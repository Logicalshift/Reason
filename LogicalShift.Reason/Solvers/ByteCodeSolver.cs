﻿using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Solver that encodes a knowledge base as bytecode in order to execute queries more quickly
    /// </summary>
    public class ByteCodeSolver : ISolver
    {
        /// <summary>
        /// The program that this solver will run
        /// </summary>
        private readonly ByteCodeProgram _program = new ByteCodeProgram();

        /// <summary>
        /// The compiled code points from the program
        /// </summary>
        private ByteCodePoint[] _compiled;

        /// <summary>
        /// The literals in the peogram
        /// </summary>
        private ILiteral[] _literals;

        /// <summary>
        /// The maximum variable index from the program
        /// </summary>
        private int _maxVariableIndex;

        /// <summary>
        /// The location of the predicates in this program
        /// </summary>
        private readonly Dictionary<ILiteral, int> _predicateLocation = new Dictionary<ILiteral,int>();

        /// <summary>
        /// Compiles a knowledge base into a bytecode program
        /// </summary>
        public async Task Compile(IKnowledgeBase knowledge)
        {
            if (knowledge == null) throw new ArgumentNullException("knowledge");

            _compiled = null;
            _literals = null;

            // Fetch the clauses and group them by their predicate
            var predicateList = (await knowledge.GetClauses())
                .GroupBy(clause => clause.Implies.UnificationKey);

            // Compile each clause in turn
            foreach (var predicate in predicateList)
            {
                // So we can call this predicate, store the location of its first instruction
                _predicateLocation[predicate.Key] = _program.Count;
                _program.Label(predicate.Key);

                var clauseList = predicate.ToArray();
                for (var clauseNum = 0; clauseNum < clauseList.Length; ++clauseNum)
                {
                    var clause = clauseList[clauseNum];

                    // Label this position
                    var thisClause = Tuple.Create(predicate.Key, clauseNum);
                    _program.Label(thisClause);

                    // Add a backtracking point if there are multiple definitions of this clause
                    if (clauseList.Length > 1)
                    {
                        var nextClause = Tuple.Create(predicate.Key, clauseNum + 1);

                        if (clauseNum == 0)
                        {
                            // First clause uses TryMeElse
                            _program.WriteWithLabel(Operation.TryMeElse, nextClause);
                        }
                        else if (clauseNum == clauseList.Length-1)
                        {
                            // Last one uses TrustMe
                            _program.Write(Operation.TrustMe);
                        }
                        else
                        {
                            // Other clauses use RetryMeElse
                            _program.WriteWithLabel(Operation.RetryMeElse, nextClause);
                        }
                    }

                    // Compile the clause itself
                    clause.Compile(_program);

                    // Final instruction is 'proceed'
                    _program.Write(Operation.Proceed);
                }
            }

            // Bind any calls to within the program that come from the same knowledgebase
            _program.BindCalls(predicate => predicate);

            _maxVariableIndex = _program.GetMaxVariableIndex();
        }

        private ByteCodePoint[] GetProgram()
        {
            if (_compiled == null)
            {
                _program.Finish();
                _compiled = _program.GetByteCode();
            }

            return _compiled;
        }

        private ILiteral[] GetLiterals()
        {
            if (_literals == null)
            {
                _literals = _program.GetLiterals();
            }

            return _literals;
        }

        public Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments)
        {
            // Create the execution environment
            var executor = new ByteCodeExecutor(GetProgram(), GetLiterals(), _maxVariableIndex+1);

            // Load the arguments
            for (int argIndex = 0; argIndex < arguments.Length; ++argIndex)
            {
                executor.Register(argIndex).SetTo(arguments[argIndex]);
            }
            
            // Run the program
            executor.Run(_predicateLocation[predicate]);

            // TODO: implement backtracking
            return () => true;
        }
    }
}

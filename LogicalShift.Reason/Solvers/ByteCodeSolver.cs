using LogicalShift.Reason.Api;
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
        /// The location of the predicates in this program
        /// </summary>
        private readonly Dictionary<ILiteral, int> _predicateLocation = new Dictionary<ILiteral,int>();

        /// <summary>
        /// Compiles a knowledge base into a bytecode program
        /// </summary>
        public async Task Compile(IKnowledgeBase knowledge)
        {
            if (knowledge == null) throw new ArgumentNullException("knowledge");

            // Fetch the clauses and group them by their predicate
            var predicateList = (await knowledge.GetClauses())
                .GroupBy(clause => clause.Implies.UnificationKey);

            // Compile each clause in turn
            foreach (var predicate in predicateList)
            {
                // So we can call this predicate, store the location of its first instruction
                _predicateLocation[predicate.Key] = _program.Count;

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
                    CompileClause(clause);

                    // Final instruction is 'proceed'
                    _program.Write(Operation.Proceed);
                }
            }
        }

        /// <summary>
        /// Compiles a clause and appends it to the program
        /// </summary>
        private void CompileClause(IClause clause)
        {
            var unifier = new ByteCodeUnifier(_program);

            // Get the assignments for this clause
            var allPredicates   = new[] { clause.Implies }.Concat(clause.If).ToArray();
            var assignmentList  = allPredicates
                .Select(predicate => new 
                {
                    Assignments = PredicateAssignmentList.FromPredicate(predicate),
                    UnificationKey = predicate.UnificationKey
                })
                .ToArray();

            // TODO: allocate space for the arguments and any permanent variables

            // Unify with the predicate first
            unifier.ProgramUnifier.Bind(assignmentList[0].Assignments.Assignments);
            unifier.ProgramUnifier.Compile(assignmentList[0].Assignments.Assignments);

            // Call the clauses
            foreach (var assignment in assignmentList.Skip(1))
            {
                unifier.QueryUnifier.Bind(assignment.Assignments.Assignments);
                unifier.QueryUnifier.Compile(assignment.Assignments.Assignments);

                _program.Write(Operation.Call, assignment.UnificationKey);
            }
        }

        public Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}

using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Unifier that writes operations to a bytecode program
    /// </summary>
    public class ByteCodeUnifier : IQueryUnifier, IProgramUnifier
    {
        /// <summary>
        /// The program that is written by this unifier
        /// </summary>
        private readonly ByteCodeProgram _program;

        /// <summary>
        /// List of variables that have been used by an instruction
        /// </summary>
        private readonly HashSet<ILiteral> _usedVariables = new HashSet<ILiteral>();

        /// <summary>
        /// The ID that a particular variable has been bound to
        /// </summary>
        private readonly Dictionary<ILiteral, int> _bindingForVariable = new Dictionary<ILiteral,int>();

        public ByteCodeUnifier(ByteCodeProgram program)
        {
            if (program == null) throw new ArgumentNullException("program");
            _program = program;
        }

        /// <summary>
        /// Removes any variables marked as 'used'
        /// </summary>
        public void ClearVariables()
        {
            _usedVariables.Clear();
            _bindingForVariable.Clear();
        }

        public bool HasVariable(ILiteral variable)
        {
            return _usedVariables.Contains(variable);
        }

        public void BindVariable(int index, ILiteral variable)
        {
            _bindingForVariable[variable] = index;
        }

        public bool PutStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            _usedVariables.Add(variable);

            _program.Write(Operation.PutStructure, termName, _bindingForVariable[variable], termLength);
            return true;
        }

        public bool SetVariable(ILiteral variable)
        {
            _usedVariables.Add(variable);

            _program.Write(Operation.SetVariable, _bindingForVariable[variable]);
            return true;
        }

        public bool SetValue(ILiteral variable)
        {
            _usedVariables.Add(variable);

            _program.Write(Operation.SetValue, _bindingForVariable[variable]);
            return true;
        }

        public bool PutVariable(ILiteral variable1, ILiteral variable2)
        {
            _usedVariables.Add(variable1);
            _usedVariables.Add(variable2);

            _program.Write(Operation.PutVariable, _bindingForVariable[variable1], _bindingForVariable[variable2]);
            return true;
        }

        public bool PutValue(ILiteral variable1, ILiteral variable2)
        {
            _usedVariables.Add(variable1);
            _usedVariables.Add(variable2);

            _program.Write(Operation.PutValue, _bindingForVariable[variable1], _bindingForVariable[variable2]);
            return true;
        }

        public bool GetStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            _usedVariables.Add(variable);

            _program.Write(Operation.GetStructure, termName, _bindingForVariable[variable], termLength);
            return true;
        }

        public bool UnifyVariable(ILiteral variable)
        {
            _usedVariables.Add(variable);

            _program.Write(Operation.UnifyVariable, _bindingForVariable[variable]);
            return true;
        }

        public bool UnifyValue(ILiteral variable)
        {
            _usedVariables.Add(variable);

            _program.Write(Operation.UnifyValue, _bindingForVariable[variable]);
            return true;
        }

        public bool GetVariable(ILiteral variable1, ILiteral variable2)
        {
            _usedVariables.Add(variable1);
            _usedVariables.Add(variable2);

            _program.Write(Operation.GetVariable, _bindingForVariable[variable1], _bindingForVariable[variable2]);
            return true;
        }

        public bool GetValue(ILiteral variable1, ILiteral variable2)
        {
            _usedVariables.Add(variable1);
            _usedVariables.Add(variable2);

            _program.Write(Operation.GetValue, _bindingForVariable[variable1], _bindingForVariable[variable2]);
            return true;
        }
    }
}

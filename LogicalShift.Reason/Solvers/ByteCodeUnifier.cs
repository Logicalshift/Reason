using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Unifier that writes operations to a bytecode program
    /// </summary>
    public class ByteCodeUnifier : IQueryUnifier, IProgramUnifier, IUnifier
    {
        /// <summary>
        /// The program that is written by this unifier
        /// </summary>
        private readonly ByteCodeProgram _program;

        /// <summary>
        /// List of variables that have been used by an instruction
        /// </summary>
        private readonly HashSet<int> _usedVariables = new HashSet<int>();

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
            var variableIndex = _bindingForVariable[variable];
            return _usedVariables.Contains(variableIndex);
        }

        public void BindVariable(int index, ILiteral variable)
        {
            _bindingForVariable[variable] = index;
        }

        public bool PutStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            var variableIndex = _bindingForVariable[variable];
            _usedVariables.Add(variableIndex);

            _program.Write(Operation.PutStructure, termName, variableIndex, termLength);
            return true;
        }

        public bool SetVariable(ILiteral variable)
        {
            var variableIndex = _bindingForVariable[variable];
            _usedVariables.Add(variableIndex);

            _program.Write(Operation.SetVariable, variableIndex);
            return true;
        }

        public bool SetValue(ILiteral variable)
        {
            var variableIndex = _bindingForVariable[variable];
            _usedVariables.Add(variableIndex);

            _program.Write(Operation.SetValue, variableIndex);
            return true;
        }

        public bool PutVariable(ILiteral variable1, ILiteral variable2)
        {
            var variableIndex1 = _bindingForVariable[variable1];
            var variableIndex2 = _bindingForVariable[variable2];

            _usedVariables.Add(variableIndex1);
            _usedVariables.Add(variableIndex2);

            _program.Write(Operation.PutVariable, variableIndex1, variableIndex2);
            return true;
        }

        public bool PutValue(ILiteral variable1, ILiteral variable2)
        {
            var variableIndex1 = _bindingForVariable[variable1];
            var variableIndex2 = _bindingForVariable[variable2];

            _usedVariables.Add(variableIndex1);
            _usedVariables.Add(variableIndex2);

            _program.Write(Operation.PutValue, variableIndex1, variableIndex2);
            return true;
        }

        public bool GetStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            var variableIndex = _bindingForVariable[variable];
            _usedVariables.Add(variableIndex);

            _program.Write(Operation.GetStructure, termName, variableIndex, termLength);
            return true;
        }

        public bool UnifyVariable(ILiteral variable)
        {
            var variableIndex = _bindingForVariable[variable];
            _usedVariables.Add(variableIndex);

            _program.Write(Operation.UnifyVariable, variableIndex);
            return true;
        }

        public bool UnifyValue(ILiteral variable)
        {
            var variableIndex = _bindingForVariable[variable];
            _usedVariables.Add(variableIndex);

            _program.Write(Operation.UnifyValue, variableIndex);
            return true;
        }

        public bool GetVariable(ILiteral variable1, ILiteral variable2)
        {
            var variableIndex1 = _bindingForVariable[variable1];
            var variableIndex2 = _bindingForVariable[variable2];

            _usedVariables.Add(variableIndex1);
            _usedVariables.Add(variableIndex2);

            _program.Write(Operation.GetVariable, variableIndex1, variableIndex2);
            return true;
        }

        public bool GetValue(ILiteral variable1, ILiteral variable2)
        {
            var variableIndex1 = _bindingForVariable[variable1];
            var variableIndex2 = _bindingForVariable[variable2];

            _usedVariables.Add(variableIndex1);
            _usedVariables.Add(variableIndex2);

            _program.Write(Operation.GetValue, variableIndex1, variableIndex2);
            return true;
        }

        public IQueryUnifier QueryUnifier
        {
            get { return this; }
        }

        public IProgramUnifier ProgramUnifier
        {
            get { return this; }
        }
    }
}

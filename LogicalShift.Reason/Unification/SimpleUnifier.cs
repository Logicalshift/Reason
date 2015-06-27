using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Basic implementation of a unifier
    /// </summary>
    public class SimpleUnifier : IQueryUnifier, IProgramUnifier
    {
        /// <summary>
        /// Set of variables that have been used before
        /// </summary>
        private readonly HashSet<ILiteral> _usedVariables = new HashSet<ILiteral>();

        /// <summary>
        /// Values for variables used by this unifier
        /// </summary>
        private readonly Dictionary<ILiteral, HeapValue> _variableValues = new Dictionary<ILiteral, HeapValue>();
        
        /// <summary>
        /// Heap used for this unifier
        /// </summary>
        private readonly List<HeapValue> _heap = new List<HeapValue>();

        /// <summary>
        /// Switches from compiling the query to compiling the program
        /// </summary>
        public void SwitchFromQueryToProgram()
        {
            _usedVariables.Clear();
        }

        public bool HasVariable(ILiteral name)
        {
            return _usedVariables.Contains(name);
        }

        public void PutStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            // Mark this variable as used
            _usedVariables.Add(variable);

            // Push to the heap
            var offset = _heap.Count;
            _heap.Add(new HeapValue() { EntryType = HeapEntryType.Structure, Offset = offset + 1 });
            _heap.Add(new HeapValue() { EntryType = HeapEntryType.Term, Offset = termLength, Value = termName });

            // Store in the variable
            _variableValues[variable] = _heap[offset];
        }

        public void SetVariable(ILiteral variable)
        {
            // Mark the variable as used
            _usedVariables.Add(variable);

            // Push to the heap
            var offset = _heap.Count;
            _heap.Add(new HeapValue() { EntryType = HeapEntryType.Reference, Offset = offset });

            // Store in the variable
            _variableValues[variable] = _heap[offset];
        }

        public void SetValue(ILiteral variable)
        {
            // Push the variable value to the heap
            _heap.Add(_variableValues[variable]);
        }

        public void GetStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            throw new NotImplementedException();
        }

        public void UnifyVariable(ILiteral variable)
        {
            throw new NotImplementedException();
        }

        public void UnifyValue(ILiteral variable)
        {
            throw new NotImplementedException();
        }
    }
}

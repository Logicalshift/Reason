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
        /// Offset representing 'null'
        /// </summary>
        const int _nullOffset = int.MinValue;

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
        /// True if running a program in write mode, false otherwise
        /// </summary>
        bool _writeMode = false;

        /// <summary>
        /// Pointer to the next value within the current structure
        /// </summary>
        private int _structurePtr = 0;

        /// <summary>
        /// Prepares to run a new program unifier using this object
        /// </summary>
        public void PrepareToRunProgram()
        {
            _usedVariables.Clear();
            _structurePtr = 0;
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
            // This variable becomes used
            _usedVariables.Add(variable);

            // Get the value of the variable
            var variableValue = _variableValues[variable];

            // Dereference it
            var dereferencedAddress = DerefHeap(variableValue.Offset);

            // Action depends on what's at that address
            var heapValue = _heap[dereferencedAddress];

            if (heapValue.EntryType == HeapEntryType.Reference && heapValue.Offset == _nullOffset)
            {
                // Variable is an unbound ref cell: bind it to a new value that we create
                var offset = _heap.Count;
                _heap.Add(new HeapValue { EntryType = HeapEntryType.Structure, Offset = offset + 1 });
                _heap.Add(new HeapValue { EntryType = HeapEntryType.Term, Value = termName, Offset = termLength });
                BindHeap(dereferencedAddress, offset);
                _writeMode = true;
            }
            else if (heapValue.EntryType == HeapEntryType.Structure)
            {
                if (Equals(_heap[heapValue.Offset].Value, termName))
                {
                    // Set the structure pointer, and use read mode
                    _structurePtr = heapValue.Offset + 1;
                    _writeMode = false;
                }
                else
                {
                    // Structure doesn't match; fail
                    throw new InvalidOperationException();
                }
            }
            else
            {
                // Fail
                throw new InvalidOperationException();
            }

            throw new NotImplementedException();
        }

        public void UnifyVariable(ILiteral variable)
        {
            // This variable becomes used
            _usedVariables.Add(variable);

            if (!_writeMode)
            {
                // Just read the value of the variable
                _variableValues[variable] = _heap[_structurePtr];
            }
            else
            {
                // Write the value of the variable
                var offset = _heap.Count;
                _heap.Add(new HeapValue { EntryType = HeapEntryType.Reference, Offset = offset });
                _variableValues[variable] = _heap[offset];
            }

            ++_structurePtr;
        }

        public void UnifyValue(ILiteral variable)
        {
            if (!_writeMode)
            {
                UnifyHeap(_variableValues[variable], _structurePtr);
            }
            else
            {
                _heap.Add(_variableValues[variable]);
            }

            ++_structurePtr;
        }

        /// <summary>
        /// Dereferences an address on the heap
        /// </summary>
        private int DerefHeap(int heapOffset)
        {
            for (;;)
            {
                var value = _heap[heapOffset];
                if (value.EntryType == HeapEntryType.Reference && value.Offset != heapOffset)
                {
                    heapOffset = value.Offset;
                }
                else
                {
                    return heapOffset;
                }
            }
        }

        /// <summary>
        /// Binds a value to the heap
        /// </summary>
        private void BindHeap(int address, int offset)
        {
            _heap[address] = new HeapValue { EntryType = HeapEntryType.Reference, Offset = offset };
        }

        /// <summary>
        /// Unifies a value on the heap
        /// </summary>
        private void UnifyHeap(int address1, int address2)
        {
            var unifyStack = new Stack<int>();

            // Push the addresses that we're going to unify
            unifyStack.Push(address1);
            unifyStack.Push(address2);

            // Iterate until the stack is empty
            while (unifyStack.Count > 0)
            {
                // Deref the values on the stack
                var offset1 = DerefHeap(address1);
                var offset2 = DerefHeap(address2);

                if (offset1 != offset2)
                {
                    var value1 = _heap[offset1];
                    var value2 = _heap[offset2];

                    if (value1.EntryType == HeapEntryType.Reference || value2.EntryType == HeapEntryType.Reference)
                    {
                        // Bind references
                        BindHeap(offset1, offset2);
                    }
                    else
                    {
                        var structure1 = _heap[value1.Offset];
                        var structure2 = _heap[value2.Offset];

                        if (Equals(structure1.Value, structure2.Value) && structure1.Offset == structure2.Offset)
                        {
                            // Process the rest of the structure
                            for (var structurePos = 1; structurePos <= structure1.Offset; ++structurePos)
                            {
                                unifyStack.Push(value1.Offset + structurePos);
                                unifyStack.Push(value2.Offset + structurePos);
                            }
                        }
                        else
                        {
                            // Structures do not match: fail
                            throw new InvalidOperationException();
                        }
                    }
                }
            }
        }
    }
}

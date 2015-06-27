using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Stores variable and heap values for unification
    /// </summary>
    public class UnificationStore
    {
        /// <summary>
        /// Offset representing the null address
        /// </summary>
        public const int NullOffset = int.MinValue;

        /// <summary>
        /// The values on the heap
        /// </summary>
        private List<HeapValue> _heap = new List<HeapValue>();

        /// <summary>
        /// The variable values
        /// </summary>
        private List<HeapValue> _variables = new List<HeapValue>();

        /// <summary>
        /// The next ID to assign to a variable
        /// </summary>
        private int _nextVariableAllocation = 0;

        /// <summary>
        /// Maps variable names to indexes into the variable arrays
        /// </summary>
        private Dictionary<ILiteral, int> _variableAddress = new Dictionary<ILiteral, int>();

        /// <summary>
        /// Returns the length of the heap
        /// </summary>
        public int HeapLength
        {
            get { return _heap.Count;}
        }

        /// <summary>
        /// Stores a value on the heap
        /// </summary>
        public void PushHeap(HeapValue newValue)
        {
            _heap.Add(newValue);
        }

        /// <summary>
        /// Reads the value at the specified address
        /// </summary>
        public HeapValue Read(int address)
        {
            if (address >= 0)
            {
                // Positive addresses are on the heap
                return ReadHeap(address);
            }
            else if (address == NullOffset)
            {
                // The null offset just contains a null reference
                return new HeapValue { EntryType = HeapEntryType.Reference, Offset = NullOffset };
            }
            else
            {
                // Negative values are variables
                return ReadVariable(-(address+1));
            }
        }

        /// <summary>
        /// Reads a variable
        /// </summary>
        private HeapValue ReadVariable(int varNum)
        {
            return _variables[varNum];
        }

        /// <summary>
        /// Reads a heap value
        /// </summary>
        private HeapValue ReadHeap(int address)
        {
            return _heap[address];
        }

        public void Write(int address, HeapValue newValue)
        {
            if (address >= 0)
            {
                WriteHeap(address, newValue);
            }
            else if (address != NullOffset)
            {
                WriteVariable(-(address+1), newValue);
            }
        }

        /// <summary>
        /// Writes to a heap address
        /// </summary>
        private void WriteHeap(int address, HeapValue newValue)
        {
            _heap[address] = newValue;
        }

        /// <summary>
        /// Writes to a variable address
        /// </summary>
        private void WriteVariable(int varNum, HeapValue newValue)
        {
            _variables[varNum] = newValue;
        }

        /// <summary>
        /// Assigns a new variable
        /// </summary>
        private int AssignNewVariable()
        {
            var result = _nextVariableAllocation;
            ++_nextVariableAllocation;
            while (_variables.Count < _nextVariableAllocation)
            {
                // New variables just refer to themselves initially
                _variables.Add(new HeapValue { EntryType = HeapEntryType.Reference, Offset = -(_variables.Count+1) });
            }
            return result;
        }

        /// <summary>
        /// Retrieves the index in the variables array for a particular variable
        /// </summary>
        private int IndexForVariable(ILiteral variable)
        {
            int result;
            if (!_variableAddress.TryGetValue(variable, out result))
            {
                result = AssignNewVariable();
                _variableAddress[variable] = result;
            }

            // Convert to a negative variable address
            return result;
        }

        /// <summary>
        /// Retrieves the address for a particular variable
        /// </summary>
        public int AddressForVariable(ILiteral variable)
        {
            var result = IndexForVariable(variable);
            return -(result + 1);
        }

        /// <summary>
        /// Reads the value of a particular variable
        /// </summary>
        public HeapValue ReadVariable(ILiteral variable)
        {
            return _variables[IndexForVariable(variable)];
        }

        /// <summary>
        /// Writes a new value to a variable
        /// </summary>
        public void WriteVariable(ILiteral variable, HeapValue newValue)
        {
            _variables[IndexForVariable(variable)] = newValue;
        }

        /// <summary>
        /// Start assigning variables to literals from address -1 again
        /// </summary>
        public void ResetVariableAssignments()
        {
            _nextVariableAllocation = 0;
        }

        /// <summary>
        /// Dereferences an address (so that it either points to a self-referential value or something that's not a reference)
        /// </summary>
        public int Dereference(int address)
        {
            for (;;)
            {
                var value = Read(address);
                if (value.EntryType == HeapEntryType.Reference && value.Offset != address)
                {
                    address = value.Offset;
                }
                else
                {
                    return address;
                }
            }
        }
    }
}

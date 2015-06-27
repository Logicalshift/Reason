using LogicalShift.Reason.Api;
using System;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Simple representation of a value stored on a unification heap
    /// </summary>
    public struct HeapValue
    {
        /// <summary>
        /// Type of this heap entry
        /// </summary>
        public HeapEntryType EntryType { get; set; }

        /// <summary>
        /// Represents a heap offset
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Value of this entry
        /// </summary>
        public ILiteral Value { get; set; }
    }
}

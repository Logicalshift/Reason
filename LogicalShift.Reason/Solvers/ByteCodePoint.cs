namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Represents a code point in a bytecode solver's program
    /// </summary>
    public struct ByteCodePoint
    {
        public ByteCodePoint(Operation op, int arg1 = 0, int arg2 = 0, int literal = 0)
        {
            Op = op;
            Arg1 = arg1;
            Arg2 = arg2;
            Literal = literal;
        }

        /// <summary>
        /// The operation to execute
        /// </summary>
        public readonly Operation Op;

        /// <summary>
        /// The first argument (generally a variable, but can be an offset, a length or a reference to a literal)
        /// </summary>
        public readonly int Arg1;

        /// <summary>
        /// The second argument (generally a variable)
        /// </summary>
        public readonly int Arg2;

        /// <summary>
        /// The identifier for the literal referenced by this code point
        /// </summary>
        public readonly int Literal;
    }
}

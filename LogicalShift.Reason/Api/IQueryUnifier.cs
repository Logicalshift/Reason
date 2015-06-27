namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Low-level operations for a query unifier
    /// </summary>
    public interface IQueryUnifier : IBaseUnifier
    {
        /// <summary>
        /// Writes a structure to the heap and stores a reference to it in a variable
        /// </summary>
        void PutStructure(ILiteral termName, int termLength, ILiteral variable);

        /// <summary>
        /// Writes a variable literal to the heap
        /// </summary>
        void SetVariable(ILiteral variable);

        /// <summary>
        /// Writes the value of a variable to the heap
        /// </summary>
        void SetValue(ILiteral variable);
    }
}

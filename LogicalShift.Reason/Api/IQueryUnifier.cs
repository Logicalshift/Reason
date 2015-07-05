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
        bool PutStructure(ILiteral termName, int termLength, ILiteral variable);

        /// <summary>
        /// Writes a variable literal to the heap
        /// </summary>
        bool SetVariable(ILiteral variable);

        /// <summary>
        /// Writes the value of a variable to the heap
        /// </summary>
        bool SetValue(ILiteral variable);

        /// <summary>
        /// Writes a variable literal to the heap and to two other variables
        /// </summary>
        bool PutVariable(ILiteral variable1, ILiteral variable2);

        /// <summary>
        /// Sets the value of variable2 to the value of variable1
        /// </summary>
        bool PutValue(ILiteral variable1, ILiteral variable2);
    }
}

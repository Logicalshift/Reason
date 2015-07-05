namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Low-level operations for a program unifier
    /// </summary>
    public interface IProgramUnifier : IBaseUnifier
    {
        /// <summary>
        /// Retrieves a structure into a variable and sets read or write mode
        /// </summary>
        bool GetStructure(ILiteral termName, int termLength, ILiteral variable);

        /// <summary>
        /// Retrieves the value of a variable that hasn't been read before
        /// </summary>
        bool UnifyVariable(ILiteral variable);

        /// <summary>
        /// Unifies a variable that has been previously read
        /// </summary>
        bool UnifyValue(ILiteral variable);

        /// <summary>
        /// Assigns variable2 to variable1
        /// </summary>
        bool GetVariable(ILiteral variable1, ILiteral variable2);

        /// <summary>
        /// Unifies variable1 and variable2
        /// </summary>
        bool GetValue(ILiteral variable1, ILiteral variable2);
    }
}

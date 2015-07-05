namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Common operations supported by unifiers has been seen before by this unifier
    /// </summary>
    public interface IBaseUnifier
    {
        /// <summary>
        /// Returns true if the variable identified has been used by SetVariable or UnifyVariable before
        /// </summary>
        /// <remarks>
        /// A variable is used if any variable bound to the same index has been encountered by an operation before.
        /// </remarks>
        bool HasVariable(ILiteral variable);

        /// <summary>
        /// Binds a variable index to a name. Multiple variable names can be bound to the same
        /// index if necessary (they will all become aliases for the same storage location).
        /// Variable names can't be used before they are bound.
        /// </summary>
        void BindVariable(int index, ILiteral variable);

        /// <summary>
        /// Retrieves the variable associated with the specified name
        /// </summary>
        IReferenceLiteral GetVariable(ILiteral variable);
    }
}

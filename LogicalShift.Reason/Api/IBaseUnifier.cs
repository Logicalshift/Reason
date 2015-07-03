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
        bool HasVariable(ILiteral variable);

        /// <summary>
        /// Binds a variable index to a name
        /// </summary>
        void BindVariable(int index, ILiteral variable);

        /// <summary>
        /// Retrieves the variable associated with the specified name
        /// </summary>
        IReferenceLiteral GetVariable(ILiteral variable);
    }
}

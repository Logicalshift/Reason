namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Common operations supported by unifiers has been seen before by this unifier
    /// </summary>
    public interface IBaseUnifier
    {
        /// <summary>
        /// Returns true if the variable identified
        /// </summary>
        bool HasVariable(ILiteral name);
    }
}

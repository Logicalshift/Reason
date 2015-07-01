namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Represents either a reference to a literal, or a literal term broken down into parameters
    /// </summary>
    public interface IReferenceLiteral
    {
        /// <summary>
        /// null if this is just a reference to another literal, otherwise a literal representing the name of the term
        /// </summary>
        ILiteral Term { get; }

        /// <summary>
        /// What this is a reference to, provided that Term is null. If term is non-null, this is a reference to the first
        /// argument. This can also be set to be self-referential to indicate that this reference should be treated as
        /// an unbound variable.
        /// </summary>
        IReferenceLiteral Reference { get; }

        /// <summary>
        /// For items that are an argument to a term, this is the pointer to the next argument
        /// </summary>
        IReferenceLiteral NextArgument { get; }

        /// <summary>
        /// Sets the value of Term and Reference to the same values as the supplied reference literal
        /// </summary>
        void SetTo(IReferenceLiteral value);
    }
}

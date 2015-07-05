namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// A trail is use to backtrack on an operation by unbinding references
    /// </summary>
    public interface ITrail
    {
        /// <summary>
        /// Records a reference that should be reset to unbound when this trail is reset
        /// </summary>
        void Record(IReferenceLiteral reference);

        /// <summary>
        /// Resets all of the recorded references back to being unbound
        /// </summary>
        void Reset();
    }
}

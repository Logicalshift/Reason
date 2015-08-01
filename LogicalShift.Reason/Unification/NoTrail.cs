using System;
using LogicalShift.Reason.Api;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Trail used when no back tracking is supported
    /// </summary>
    public class NoTrail : ITrail
    {
        public void Record(IReferenceLiteral reference)
        {
        }

        public void Reset()
        {
            throw new NotSupportedException("Backtracking is not supported");
        }
    }
}

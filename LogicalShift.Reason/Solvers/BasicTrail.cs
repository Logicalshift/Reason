using LogicalShift.Reason.Api;
using System.Collections.Generic;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Simplest possible implementation of a trail
    /// </summary>
    public class BasicTrail : ITrail
    {
        /// <summary>
        /// Items to reset when this trail is reset
        /// </summary>
        private readonly List<IReferenceLiteral> _toReset = new List<IReferenceLiteral>();

        public void Record(IReferenceLiteral reference)
        {
            _toReset.Add(reference);
        }

        public void Reset()
        {
            foreach (var val in _toReset)
            {
                val.Unbind();
            }
            _toReset.Clear();
        }
    }
}

using LogicalShift.Reason.Api;
using System;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Unification state with no bound variables
    /// </summary>
    public class EmptyUnificationState : IUnificationState
    {
        public EmptyUnificationState()
        {
        }

        public bool TryGetBindingForVariable(ILiteral variable, out ILiteral binding)
        {
            binding = null;
            return false;
        }

        public IUnificationState StateWithBinding(ILiteral variable, ILiteral value)
        {
            return new ListUnificationState(variable, value, null);
        }
    }
}

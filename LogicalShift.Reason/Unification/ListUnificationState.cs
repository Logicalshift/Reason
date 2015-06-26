using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Unification state where the bindings are just represented as a list
    /// </summary>
    public class ListUnificationState : IUnificationState
    {
        /// <summary>
        /// Next state in the list (or null for the last element)
        /// </summary>
        private readonly ListUnificationState _next;

        /// <summary>
        /// Literal representing the variable that is bound
        /// </summary>
        private readonly ILiteral _thisVariable;

        /// <summary>
        /// The value that it has been bound to
        /// </summary>
        private readonly ILiteral _boundTo;

        public ListUnificationState(ILiteral variable, ILiteral boundTo, ListUnificationState next)
        {
            if (variable == null) throw new ArgumentNullException("variable");
            if (boundTo == null) throw new ArgumentNullException("boundTo");

            _thisVariable = variable;
            _boundTo = boundTo;
            _next = next;
        }

        public bool TryGetBindingForVariable(ILiteral variable, out ILiteral binding)
        {
            for (var state = this; state != null; state = state._next)
            {
                if (Equals(variable, state._thisVariable))
                {
                    binding = state._boundTo;
                    return true;
                }
            }

            binding = null;
            return false;
        }

        public IUnificationState StateWithBinding(ILiteral variable, ILiteral value)
        {
            return new ListUnificationState(variable, value, this);
        }
    }
}

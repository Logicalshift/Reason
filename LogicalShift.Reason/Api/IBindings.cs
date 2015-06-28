using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Represents a series of variable bindings
    /// </summary>
    public interface IBindings
    {
        /// <summary>
        /// Represents the unified result
        /// </summary>
        ILiteral Result { get; }

        /// <summary>
        /// The list of variables that were bound
        /// </summary>
        IEnumerable<ILiteral> Variables { get; }

        /// <summary>
        /// Finds what the specified variable was bound to
        /// </summary>
        ILiteral GetValueForVariable(ILiteral variable);
    }
}

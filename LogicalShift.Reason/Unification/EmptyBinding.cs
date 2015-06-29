using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// A class representing no bindings
    /// </summary>
    public class EmptyBinding : IBindings
    {
        public EmptyBinding(ILiteral result)
        {
            Result = result;
        }

        public ILiteral Result 
        { 
            get; private set;
        }

        public IEnumerable<ILiteral> Variables
        {
            get { yield break; }
        }

        public ILiteral GetValueForVariable(ILiteral variable)
        {
            return null;
        }
    }
}

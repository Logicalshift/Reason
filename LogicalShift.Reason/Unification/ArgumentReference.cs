using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Represents a reference to a structure argument
    /// </summary>
    public class ArgumentReference : IReferenceLiteral
    {
        public ArgumentReference(IReferenceLiteral value)
        {
            if (value == null) value = this;

            Term = null;
            Reference = value;
            NextArgument = null;
        }

        public ArgumentReference(IReferenceLiteral value, IReferenceLiteral nextArgument)
        {
            if (value == null) value = this;

            Term = null;
            Reference = value;
            NextArgument = nextArgument;
        }

        public ILiteral Term
        {
            get; private set;
        }

        public IReferenceLiteral Reference
        {
            get; private set;
        }

        public IReferenceLiteral NextArgument
        {
            get; private set;
        }

        public void SetTo(IReferenceLiteral value)
        {
            Term = value.Term;
            Reference = value.Reference;
        }
    }
}

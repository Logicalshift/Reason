using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Basic implementation of a reference literal. This can be used to represent a simple reference
    /// </summary>
    public class SimpleReference : IReferenceLiteral
    {
        public SimpleReference()
        {
            Term = null;
            Reference = this;
        }

        public SimpleReference(IReferenceLiteral reference)
        {
            if (reference == null) reference = this;

            Term = null;
            Reference = reference;
        }

        public SimpleReference(ILiteral term, IReferenceLiteral referenceToFirstArgument)
        {
            if (referenceToFirstArgument == null) referenceToFirstArgument = this;

            Term = term;
            Reference = referenceToFirstArgument;
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
            get { return null; }
        }

        public void SetTo(IReferenceLiteral value)
        {
            Term = value.Term;
            Reference = value.Reference;
        }
    }
}

using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class UnificationState
    {
        [Test]
        public void CanResolveBinding()
        {
            var binding = Literal.NewAtom();
            var boundTo = Literal.NewAtom();

            var empty = new EmptyUnificationState();
            var withBinding = empty.StateWithBinding(binding, boundTo);

            ILiteral fetchedBinding;
            Assert.IsFalse(empty.TryGetBindingForVariable(binding, out fetchedBinding));
            Assert.IsTrue(withBinding.TryGetBindingForVariable(binding, out fetchedBinding));
            Assert.AreEqual(boundTo, fetchedBinding);
        }
    }
}

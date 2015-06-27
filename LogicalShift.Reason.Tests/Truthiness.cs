using LogicalShift.Reason.Api;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class Truthiness
    {
        [Test]
        public void TrueIsTrue()
        {
            Assert.IsTrue(Equals(Literal.True(), Literal.True()));
        }

        [Test]
        public void TrueHashesToSelf()
        {
            var dict = new Dictionary<ILiteral, bool>();
            dict[Literal.True()] = true;
            Assert.IsTrue(dict[Literal.True()]);
        }

        [Test]
        public void TrueUnifiesWithSelf()
        {
            var truthiness = Literal.True();

            Assert.AreEqual(1, truthiness.Unify(truthiness).Count());
        }

        [Test]
        public void TrueDoesNotUnifyWithDifferentAtom()
        {
            var truthiness = Literal.True();
            var otherAtom = Literal.NewAtom();

            Assert.AreEqual(0, truthiness.Unify(otherAtom).Count());
        }
    }
}

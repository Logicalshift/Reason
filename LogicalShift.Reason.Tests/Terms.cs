using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class Terms
    {
        [Test]
        public void TermEqualsSelf()
        {
            var term = Literal.NewTerm(1);
            Assert.IsTrue(Equals(term, term));
        }

        [Test]
        public void TermWithParametersEqualsSelf()
        {
            var term = Literal.NewTerm(1);
            var withParameters = term.With(Literal.NewAtom());

            Assert.IsTrue(Equals(withParameters, withParameters));
        }

        [Test]
        public void TermUnifiesWithSelf()
        {
            var term = Literal.NewTerm(1);
            var withParameters = term.With(Literal.NewAtom());

            var unified = withParameters.Unify(withParameters).ToList();

            Assert.AreEqual(1, unified.Count);
            Assert.IsTrue(withParameters.Equals(unified[0]));
        }

        [Test]
        public void TermUnifiesWithParameter()
        {
            var tA = Literal.NewTerm(1);
            var aX = Literal.NewAtom();
            var vY = Literal.NewVariable();

            var aOfX = tA.With(aX);     // a(x)
            var aOfY = tA.With(vY);     // a(Y)

            var unified = aOfY.Unify(aOfX).ToList();

            Assert.AreEqual(1, unified.Count);
            Assert.IsTrue(aOfX.Equals(unified[0]));
        }
    }
}

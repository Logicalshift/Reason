using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class Functors
    {
        [Test]
        public void FunctorEqualsSelf()
        {
            var functor = Literal.NewFunctor(1);
            Assert.IsTrue(Equals(functor, functor));
        }

        [Test]
        public void FunctorWithParametersEqualsSelf()
        {
            var functor = Literal.NewFunctor(1);
            var withParameters = functor.With(Literal.NewAtom());

            Assert.IsTrue(Equals(withParameters, withParameters));
        }

        [Test]
        public void FunctorUnifiesWithSelf()
        {
            var functor = Literal.NewFunctor(1);
            var withParameters = functor.With(Literal.NewAtom());

            var unified = withParameters.Unify(withParameters).ToList();

            Assert.AreEqual(1, unified.Count);
            Assert.IsTrue(withParameters.Equals(unified[0]));
        }

        [Test]
        public void FunctorUnifiesWithParameter()
        {
            var tA = Literal.NewFunctor(1);
            var aX = Literal.NewAtom();
            var vY = Literal.NewVariable();

            var aOfX = tA.With(aX);     // a(x)
            var aOfY = tA.With(vY);     // a(Y)

            var unified = aOfY.Unify(aOfX).ToList();

            Assert.AreEqual(1, unified.Count);
            Assert.IsTrue(aOfX.Equals(unified[0]));
        }

        [Test]
        public void MoreUnification()
        {
            var p = Literal.NewFunctor(3);
            var h = Literal.NewFunctor(2);
            var f = Literal.NewFunctor(1);
            var a = Literal.NewAtom();
            var Z = Literal.NewVariable();
            var W = Literal.NewVariable();
            var Y = Literal.NewVariable();
            var X = Literal.NewVariable();

            // p(Z, h(Z, W), f(W))
            var query = p.With(Z, h.With(Z, W), f.With(W));

            // p(f(X), h(Y, f(a)), Y)
            var program = p.With(f.With(X), h.With(Y, f.With(a)), Y);

            var result = query.Unify(program).ToList();

            Assert.AreEqual(1, result.Count);
        }
    }
}

using LogicalShift.Reason.Api;
using LogicalShift.Reason.Literals;
using LogicalShift.Reason.Unification;
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
        public void ManualUnification()
        {
            var unifier = new SimpleUnifier();
            var X = new[] { new Variable(), new Variable(), new Variable(), new Variable(), new Variable(), new Variable(), new Variable(), new Variable() };
            var h2 = new UnboundFunctor(2);
            var f1 = new UnboundFunctor(1);
            var p3 = new UnboundFunctor(3);
            var a0 = new UnboundFunctor(0);

            unifier.BindVariable(X[1]);
            unifier.BindVariable(X[2]);
            unifier.BindVariable(X[3]);
            unifier.BindVariable(X[4]);
            unifier.BindVariable(X[5]);
            unifier.BindVariable(X[6]);
            unifier.BindVariable(X[7]);

            unifier.PutStructure(h2, 2, X[3]);
            unifier.SetVariable(X[2]);
            unifier.SetVariable(X[5]);
            unifier.PutStructure(f1, 1, X[4]);
            unifier.SetValue(X[5]);
            unifier.PutStructure(p3, 3, X[1]);
            unifier.SetValue(X[2]);
            unifier.SetValue(X[3]);
            unifier.SetValue(X[4]);

            unifier.GetStructure(p3, 3, X[1]);
            unifier.UnifyVariable(X[2]);
            unifier.UnifyVariable(X[3]);
            unifier.UnifyVariable(X[4]);
            unifier.GetStructure(f1, 1, X[2]);
            unifier.UnifyVariable(X[5]);
            unifier.GetStructure(h2, 2, X[3]);
            unifier.UnifyValue(X[4]);
            unifier.UnifyVariable(X[6]);
            unifier.GetStructure(f1, 1, X[6]);
            unifier.UnifyVariable(X[7]);
            unifier.GetStructure(a0, 0, X[7]);

            var result = unifier.UnifiedValue(X[1]);
            Assert.IsNotNull(result);
        }

        private Tuple<ILiteral, ILiteral> GetQueryAndProgram1()
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

            return new Tuple<ILiteral, ILiteral>(query, program);
        }

        [Test]
        public void FlattenedQueryEliminatesDuplicates()
        {
            var queryProgram = GetQueryAndProgram1();

            var flattenedQuery = queryProgram.Item1.Flatten().ToList();

            // Should be something like:
            //   X1 = p(X2, X3, X4)
            //   X2 = Z
            //   X3 = h(X2, X5)
            //   X4 = f(X5)
            //   X5 = W
            // W and Z are duplicates so they are only introduced once
            Assert.AreEqual(5, flattenedQuery.Count);
        }

        [Test]
        public void MoreUnification()
        {
            var queryProgram = GetQueryAndProgram1();
            var query = queryProgram.Item1;
            var program = queryProgram.Item2;

            var result = query.Unify(program).ToList();

            Assert.AreEqual(1, result.Count);
        }
    }
}

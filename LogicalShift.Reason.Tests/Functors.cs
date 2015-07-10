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
        public void FindFreeVariableInFunctor()
        {
            var variable = Literal.NewVariable();
            var functor = Literal.NewFunctor(1).With(variable);
            var unifier = new SimpleUnifier();

            var freeVars = unifier.QueryUnifier.Compile(functor).ToList();
            Assert.AreEqual(1, freeVars.Count);
            Assert.AreEqual(variable, freeVars[0]);
        }

        [Test]
        public void FunctorUnifiesWithSelf()
        {
            var functor = Literal.NewFunctor(1);
            var withParameters = functor.With(Literal.NewAtom());

            var unified = withParameters.Unify(withParameters);

            Assert.IsNotNull(unified);
            Assert.IsTrue(withParameters.Equals(unified.Result));
        }

        [Test]
        public void FunctorUnifiesWithParameter1()
        {
            var tA = Literal.NewFunctor(1);
            var aX = Literal.NewAtom();
            var vY = Literal.NewVariable();

            var aOfX = tA.With(aX);     // a(x)
            var aOfY = tA.With(vY);     // a(Y)

            var unified = aOfY.Unify(aOfX);

            Assert.IsNotNull(unified);
            Assert.IsTrue(aOfX.Equals(unified.Result));
            Assert.AreEqual(aX, unified.GetValueForVariable(vY));
        }

        [Test]
        public void FunctorUnifiesWithParameter2()
        {
            var tA = Literal.NewFunctor(1);
            var aX = Literal.NewAtom();
            var vY = Literal.NewVariable();

            var aOfX = tA.With(aX);     // a(x)
            var aOfY = tA.With(vY);     // a(Y)

            var unified = aOfX.Unify(aOfY);

            Assert.IsNotNull(unified);
            Assert.IsTrue(aOfX.Equals(unified.Result));
        }

        [Test]
        public void FunctorUnifiesWithParameter3()
        {
            var tA = Literal.NewFunctor(1);
            var vX = Literal.NewVariable();
            var vY = Literal.NewVariable();

            var aOfX = tA.With(vX);     // a(x)
            var aOfY = tA.With(vY);     // a(Y)

            var unified = aOfX.Unify(aOfY);

            Assert.IsNotNull(unified);
        }

        [Test]
        public void FunctorUnifiesWithParameter4()
        {
            var tA = Literal.NewFunctor(2);
            var tB = Literal.NewFunctor(1);
            var vX = Literal.NewVariable();
            var vY = Literal.NewVariable();

            var aOfX = tA.With(tB.With(vX), vX);    // a(x)
            var aOfY = tA.With(vX, vY);             // a(Y)

            var unified = aOfX.Unify(aOfY);

            Assert.IsNotNull(unified);
        }

        [Test]
        public void FunctorUnifiesWithBindings()
        {
            var tA = Literal.NewFunctor(1);
            var vX = Literal.NewVariable();
            var vY = Literal.NewVariable();
            var aX = Literal.NewAtom();

            var aOfX = tA.With(vX);     // a(x)
            var aOfY = tA.With(vY);     // a(Y)

            var bindings = new BasicBinding(aX, new[] {
                new { var = vX, val = aX}
            }.ToDictionary(item => item.var, item => item.val));

            var unified = aOfY.Unify(aOfX, bindings);

            Assert.IsNotNull(unified);
            Assert.AreEqual(aX, unified.GetValueForVariable(vY));
        }

        [Test]
        public void ManualUnification()
        {
            var simpleUnifier = new SimpleUnifier();
            var unifier = new TraceUnifier(simpleUnifier);
            var X = new[] { new Variable(), new Variable(), new Variable(), new Variable(), new Variable(), new Variable(), new Variable(), new Variable() };
            var h2 = new UnboundFunctor(2);
            var f1 = new UnboundFunctor(1);
            var p3 = new UnboundFunctor(3);
            var a0 = new UnboundFunctor(0);

            unifier.BindVariable(0, X[1]);
            unifier.BindVariable(1, X[2]);
            unifier.BindVariable(2, X[3]);
            unifier.BindVariable(3, X[4]);
            unifier.BindVariable(4, X[5]);
            unifier.BindVariable(5, X[6]);
            unifier.BindVariable(6, X[7]);

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

            var result = simpleUnifier.UnifiedValue(X[1]);
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
        public void FlattenedQueryIsInTopDownOrder()
        {
            var p = Literal.NewFunctor(3);
            var h = Literal.NewFunctor(2);
            var f = Literal.NewFunctor(1);
            var a = Literal.NewAtom();
            var Y = Literal.NewVariable();
            var X = Literal.NewVariable();

            // p(f(X), h(Y, f(a)), Y)
            var query = p.With(f.With(X), h.With(Y, f.With(a)), Y);

            var flattened = query.Flatten().ToList();

            Assert.AreEqual(7, flattened.Count);
            Assert.AreEqual(p, flattened[0].Value.UnificationKey);
        }

        [Test]
        public void MoreUnification()
        {
            var queryProgram = GetQueryAndProgram1();
            var query = queryProgram.Item1;
            var program = queryProgram.Item2;

            var result = query.Unify(program);

            Assert.IsNotNull(result);
        }

        [Test]
        public void FindFreeVariableInMoreComplicatedFunctor()
        {
            var functor = GetQueryAndProgram1().Item1;
            var unifier = new SimpleUnifier();

            var freeVars = unifier.QueryUnifier.Compile(functor).ToList();
            Assert.AreEqual(2, freeVars.Count);
        }
    }
}

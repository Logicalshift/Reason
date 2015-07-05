using LogicalShift.Reason.Solvers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class SingleClauseSolver
    {
        [Test]
        public void SolveAsSelf()
        {
            var a = Literal.NewAtom();
            var b = Literal.NewAtom();
            var fab = Literal.NewFunctor(2).With(a, b);
            var clause = Clause.Always(fab);
            var solver = new SimpleSingleClauseSolver(clause, new NothingSolver());

            Assert.IsTrue(solver.Query(fab).Success);
        }

        [Test]
        public void SolveWithVariable()
        {
            var a = Literal.NewAtom();
            var b = Literal.NewAtom();
            var f = Literal.NewFunctor(2);
            var X = Literal.NewVariable();
            var fab = f.With(a, b);
            var fXb = f.With(X, b);
            var clause = Clause.Always(fab);
            var solver = new SimpleSingleClauseSolver(clause, new NothingSolver());

            Assert.IsTrue(solver.Query(fXb).Success);
        }

        [Test]
        public void VariablesThatCantUnifyCantBeSolved1()
        {
            // An argument of f(X, X) can't unify against f(a, b) as X can't be both b and a
            var a = Literal.NewAtom();
            var b = Literal.NewAtom();
            var f = Literal.NewFunctor(2);
            var g = Literal.NewFunctor(1);
            var X = Literal.NewVariable();
            var gfab = g.With(f.With(a, b));
            var gfXX = g.With(f.With(X, X));
            var clause = Clause.Always(gfab);
            var solver = new SimpleSingleClauseSolver(clause, new NothingSolver());

            Assert.IsFalse(solver.Query(gfXX).Success);
        }

        [Test]
        public void VariablesThatCantUnifyCantBeSolved2()
        {
            // The arguments a,b can't unify against X,X as X can't be both values
            var a = Literal.NewAtom();
            var b = Literal.NewAtom();
            var f = Literal.NewFunctor(2);
            var X = Literal.NewVariable();
            var fab = f.With(a, b);
            var fXX = f.With(X, X);
            var clause = Clause.Always(fab);
            var solver = new SimpleSingleClauseSolver(clause, new NothingSolver());

            Assert.IsFalse(solver.Query(fXX).Success);
        }

        [Test]
        public void DifferentDoesNotSolve()
        {
            var a = Literal.NewAtom();
            var b = Literal.NewAtom();
            var c = Literal.NewAtom();
            var f = Literal.NewFunctor(2);
            var fab = f.With(a, b);
            var fac = f.With(a, c);
            var clause = Clause.Always(fab);
            var solver = new SimpleSingleClauseSolver(clause, new NothingSolver());

            Assert.IsFalse(solver.Query(fac).Success);
        }
    }
}

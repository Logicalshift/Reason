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
            var clause = Clause.Always(a);
            var solver = new SimpleSingleClauseSolver(clause, new NothingSolver());

            Assert.IsTrue(solver.Query(a)());
        }
    }
}

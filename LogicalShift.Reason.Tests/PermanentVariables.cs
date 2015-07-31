using LogicalShift.Reason.Solvers;
using NUnit.Framework;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class PermanentVariables
    {
        [Test]
        public void PicksCorrectVariablesInSimpleCase()
        {
            // Predicate p(X,Y) :- q(X, Z), r(Z,Y) should have Z and Y as permanent variables
            var p = Literal.NewAtom();
            var q = Literal.NewAtom();
            var r = Literal.NewAtom();

            var X = Literal.NewVariable();
            var Y = Literal.NewVariable();
            var Z = Literal.NewVariable();

            var pXY = Literal.NewFunctor(2).With(X, Y);
            var qXZ = Literal.NewFunctor(2).With(X, Z);
            var rZY = Literal.NewFunctor(2).With(Z, Y);

            // Create the clause
            var clause = Clause.If(qXZ, rZY).Then(pXY);

            // Compile to assignments
            var allPredicates = new[] { clause.Implies }.Concat(clause.If).ToArray();
            var assignmentList = allPredicates.Select(predicate => PredicateAssignmentList.FromPredicate(predicate));

            // Get the set of permanent variables
            var permanent = PermanentVariableAssignments.PermanentVariables(assignmentList);

            // Z and Y are the only permanent variables
            Assert.IsFalse(permanent.Contains(X));
            Assert.IsTrue(permanent.Contains(Y));
            Assert.IsTrue(permanent.Contains(Z));
            Assert.AreEqual(2, permanent.Count);
        }
    }
}

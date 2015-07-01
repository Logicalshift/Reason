using LogicalShift.Reason.Api;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class SimpleResolve
    {
        [Test]
        [TestCase(SolverStyle.BackwardsChaining)]
        [TestCase(SolverStyle.ForwardsChaining)]
        public async Task ResolveASimpleGoal(SolverStyle style)
        {
            // Create some knowledge
            var knowledge = KnowledgeBase.New();
            var houseCat = Literal.NewAtom();
            var feline = Literal.NewAtom();
            var small = Literal.NewAtom();

            // Small felines are house cats and small and feline are true
            var houseCatsAreSmallFelines = Clause.If(feline, small).Then(houseCat);
            knowledge = knowledge.Assert(houseCatsAreSmallFelines);
            knowledge = knowledge.Assert(Clause.Always(feline));
            knowledge = knowledge.Assert(Clause.Always(small));

            // Solve for 'houseCat' (which should be true under this simple system)
            var solver = Solver.NewSolver(knowledge, style);

            // Should be true
            var result = await solver.Solve(houseCat);
            Assert.IsTrue(result.Success);
        }

        [Test]
        [TestCase(SolverStyle.BackwardsChaining)]
        /* [TestCase(SolverStyle.ForwardsChaining)] */ // TODO
        public async Task ResolveAMoreComplicatedGoal(SolverStyle style)
        {
            var knowledge = KnowledgeBase.New();

            var houseCat = Literal.NewFunctor(1);
            var feline = Literal.NewFunctor(1);
            var small = Literal.NewFunctor(1);
            var meows = Literal.NewFunctor(1);
            var tom = Literal.NewAtom();
            var X = Literal.NewVariable();
            var Y = Literal.NewVariable();

            // houseCat(X) :- small(X), feline(X)
            var houseCatsAreSmallFelines = Clause.If(feline.With(X), small.With(X)).Then(houseCat.With(X));
            var felinesMeow = Clause.If(meows.With(Y)).Then(feline.With(Y));
            var tomIsSmall = Clause.Always(small.With(tom));
            var tomMeows = Clause.Always(meows.With(tom));

            knowledge = knowledge
                .Assert(houseCatsAreSmallFelines)
                .Assert(tomIsSmall)
                .Assert(felinesMeow)
                .Assert(tomMeows);

            var solver = Solver.NewSolver(knowledge, style);
            var result = await solver.Solve(houseCat.With(tom));
            Assert.IsTrue(result.Success);

            var allCats = await solver.Solve(houseCat.With(X));
            Assert.IsTrue(result.Success);
            Assert.AreEqual(tom, allCats.Bindings.GetValueForVariable(X));
        }
    }
}

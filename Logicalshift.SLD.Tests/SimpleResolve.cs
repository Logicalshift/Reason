using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Logicalshift.SLD.Tests
{
    [TestFixture]
    public class SimpleResolve
    {
        [Test]
        public async Task ResolveASimpleGoal()
        {
            // Create some knowledge
            var knowledge = KnowledgeBase.New();
            var houseCat = Literal.NewAtom();
            var feline = Literal.NewAtom();
            var small = Literal.NewAtom();

            // Small felines are house cats and small and feline are true
            var houseCatsAreSmallFelines = Clause.If(feline, small).Then(houseCat);
            knowledge = knowledge.Assert(houseCatsAreSmallFelines);
            knowledge = knowledge.Assert(Clause.If(Literal.True()).Then(feline));
            knowledge = knowledge.Assert(Clause.If(Literal.True()).Then(small));

            // Solve for 'houseCat' (which should be true under this simple system)
            var solver = Solver.NewSolver(knowledge);

            // Should be true
            var result = await solver.Solve(houseCat);
            Assert.IsTrue(result.Success);
        }
    }
}

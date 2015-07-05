using LogicalShift.Reason.Api;
using LogicalShift.Reason.Solvers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class DispatchingSolver
    {
        [Test]
        public async Task SimpleChainQuery()
        {
            var knowledge = KnowledgeBase.New();

            var houseCat = Literal.NewFunctor(1);
            var feline = Literal.NewFunctor(1);
            var small = Literal.NewFunctor(1);
            var meows = Literal.NewFunctor(1);
            var tom = Literal.NewAtom();
            var jerry = Literal.NewAtom();
            var X = Literal.NewVariable();
            var Y = Literal.NewVariable();

            // houseCat(X) :- small(X), feline(X)
            var houseCatsAreSmallFelines = Clause.If(feline.With(X), small.With(X)).Then(houseCat.With(X));
            var felinesMeow = Clause.If(meows.With(Y)).Then(feline.With(Y));
            var tomIsSmall = Clause.Always(small.With(tom));
            var jerryIsSmall = Clause.Always(small.With(jerry));
            var tomMeows = Clause.Always(meows.With(tom));

            knowledge = knowledge
                .Assert(houseCatsAreSmallFelines)
                .Assert(tomIsSmall)
                .Assert(felinesMeow)
                .Assert(jerryIsSmall)
                .Assert(tomMeows);

            var solver = new SimpleDispatchingSolver();
            await solver.LoadFromKnowledgeBase(knowledge);

            // Tom is a housecat
            var result = solver.Query(houseCat.With(tom));
            Assert.IsTrue(result.Success);
            Assert.IsFalse((await result.Next()).Success);

            // Jerry is not a housecat
            result = solver.Query(houseCat.With(jerry));
            Assert.IsFalse(result.Success);
        }

        [Test]
        public async Task ManyHouseCats()
        {
            var tom = Literal.NewAtom();
            var heathcliff = Literal.NewAtom();
            var sylvester = Literal.NewAtom();
            var houseCat = Literal.NewFunctor(1);
            var any = Literal.NewVariable();

            var knowledge = KnowledgeBase.New()
                .Assert(Clause.Always(houseCat.With(tom)))
                .Assert(Clause.Always(houseCat.With(heathcliff)))
                .Assert(Clause.Always(houseCat.With(sylvester)));

            // Get all the cats
            var solver = new SimpleDispatchingSolver();
            await solver.LoadFromKnowledgeBase(knowledge);

            var allResults = new List<ILiteral>();
            for (var result = solver.Query(houseCat.With(any)); result != null && result.Success; result = await result.Next())
            {
                allResults.Add(result.Bindings.GetValueForVariable(any));
            }

            Assert.IsTrue(allResults.Contains(tom));
            Assert.IsTrue(allResults.Contains(heathcliff));
            Assert.IsTrue(allResults.Contains(sylvester));
            Assert.AreEqual(3, allResults.Count);
        }
        [Test]
        public async Task MultiChainQuery()
        {
            var knowledge = KnowledgeBase.New();

            var houseCat = Literal.NewFunctor(1);
            var feline = Literal.NewFunctor(1);
            var small = Literal.NewFunctor(1);
            var meows = Literal.NewFunctor(1);
            var tom = Literal.NewAtom();
            var heathcliff = Literal.NewAtom();
            var sylvester = Literal.NewAtom();
            var X = Literal.NewVariable();
            var Y = Literal.NewVariable();

            // houseCat(X) :- small(X), feline(X)
            var houseCatsAreSmallFelines = Clause.If(feline.With(X), small.With(X)).Then(houseCat.With(X));
            var felinesMeow = Clause.If(meows.With(Y)).Then(feline.With(Y));
            var tomIsSmall = Clause.Always(small.With(tom));
            var heathcliffIsSmall = Clause.Always(small.With(heathcliff));
            var sylvesterIsSmall = Clause.Always(small.With(sylvester));
            var tomMeows = Clause.Always(meows.With(tom));
            var heathcliffIsFeline = Clause.Always(feline.With(heathcliff));
            var sylvesterMeows = Clause.Always(meows.With(sylvester));

            knowledge = knowledge
                .Assert(houseCatsAreSmallFelines)
                .Assert(tomIsSmall)
                .Assert(felinesMeow)
                .Assert(tomMeows)
                .Assert(heathcliffIsSmall)
                .Assert(heathcliffIsFeline)
                .Assert(sylvesterIsSmall)
                .Assert(sylvesterMeows);

            var solver = new SimpleDispatchingSolver();
            await solver.LoadFromKnowledgeBase(knowledge);

            // Get all the cats
            var allResults = new List<ILiteral>();
            var any = Literal.NewVariable();
            for (var result = solver.Query(houseCat.With(any)); result != null && result.Success; result = await result.Next())
            {
                allResults.Add(result.Bindings.GetValueForVariable(any));
            }

            Assert.IsTrue(allResults.Contains(tom));
            Assert.IsTrue(allResults.Contains(heathcliff));
            Assert.IsTrue(allResults.Contains(sylvester));
            Assert.AreEqual(3, allResults.Count); 
        }
    }
}

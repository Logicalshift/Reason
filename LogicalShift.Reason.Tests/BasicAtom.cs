using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class BasicAtom
    {
        [Test]
        public void AtomEqualsSelf()
        {
            var atom = Literal.NewAtom();
            Assert.IsTrue(Equals(atom, atom));
        }

        [Test]
        public void AtomUnifiesWithSelf()
        {
            var atom = Literal.NewAtom();

            Assert.AreEqual(1, atom.Unify(atom).Count());
        }

        [Test]
        public void AtomDoesNotUnifyWithDifferentAtom()
        {
            var atom = Literal.NewAtom();
            var otherAtom = Literal.NewAtom();

            Assert.AreEqual(0, atom.Unify(otherAtom).Count());
        }

        [Test]
        public void AtomNotEqualToOtherAtom()
        {
            var atom = Literal.NewAtom();
            var otherAtom = Literal.NewAtom();
            Assert.IsFalse(Equals(atom, otherAtom));
        }

        [Test]
        public void AtomHashesToSelf()
        {
            var atom = Literal.NewAtom();
            var dict = new Dictionary<ILiteral, bool>();
            dict[atom] = true;
            Assert.IsTrue(dict[atom]);
        }

        [Test]
        public void AtomIsNotFreeVariable()
        {
            var atom = Literal.NewAtom();
            var unifier = new SimpleUnifier();

            var freeVars = unifier.QueryUnifier.Compile(atom).ToList();
            Assert.AreEqual(0, freeVars.Count);
        }
    }
}

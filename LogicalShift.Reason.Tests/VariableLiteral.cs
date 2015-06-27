using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class VariableLiteral
    {
        [Test]
        public void VariableEqualsSelf()
        {
            var variable = Literal.NewVariable();
            Assert.IsTrue(Equals(variable, variable));
        }

        [Test]
        public void VariableUnificationCreatesBinding()
        {
            var variable = Literal.NewVariable();
            var atom = Literal.NewAtom();

            var states = variable.Unify(atom).ToList();

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(atom, states[0]);
        }

        [Test]
        public void AtomUnificationCreatesBinding()
        {
            var variable = Literal.NewVariable();
            var atom = Literal.NewAtom();

            var states = atom.Unify(variable).ToList();

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(atom, states[0]);
        }

        /*
        [Test]
        public void BindVariableToVariable()
        {
            var variable1 = Literal.NewVariable();
            var variable2 = Literal.NewVariable();
            var atom = Literal.NewAtom();

            var states = variable2.Unify(variable1, new EmptyUnificationState()).ToList();
            states = variable1.Unify(atom, states[0]).ToList();

            ILiteral variableBoundTo;
            Assert.AreEqual(1, states.Count);

            Assert.IsTrue(states[0].TryGetBindingForVariable(variable1, out variableBoundTo));
            Assert.AreEqual(atom, variableBoundTo);

            Assert.IsTrue(states[0].TryGetBindingForVariable(variable2, out variableBoundTo));
            Assert.AreEqual(atom, variableBoundTo);
        }
         */
    }
}

using LogicalShift.Reason.Solvers;
using NUnit.Framework;
using System;

namespace LogicalShift.Reason.Tests
{
    [TestFixture]
    public class ByteCode
    {
        [Test]
        public void UnifierUsesIndexAndNotLiteralVariable()
        {
            var unifier = new ByteCodeUnifier(new ByteCodeProgram());

            var var1 = Literal.NewVariable();
            var var2 = Literal.NewVariable();

            unifier.BindVariable(1, var1);
            unifier.BindVariable(1, var2);

            // As var1 and var2 are bound to the same index, using var1 should mark var2 as used as well
            unifier.SetVariable(var1);
            Assert.IsTrue(unifier.HasVariable(var1));
            Assert.IsTrue(unifier.HasVariable(var2));
        }

        [Test]
        public void SimpleCompile()
        {
            var p = Literal.NewAtom();
            var q = Literal.NewAtom();
            var r = Literal.NewAtom();

            var X = Literal.NewVariable();
            var Y = Literal.NewVariable();
            var Z = Literal.NewVariable();

            var pXY = Literal.NewFunctor(2).With(X, Y);
            var qXZ = Literal.NewFunctor(2).With(X, Z);
            var rZY = Literal.NewFunctor(2).With(Z, Y);

            // p(X, Y) :- q(X, Z), r(Z, Y)
            var clause = Clause.If(qXZ, rZY).Then(pXY);

            // Compile
            ByteCodeProgram program = new ByteCodeProgram();
            clause.Compile(program);

            Console.WriteLine(program.ToString());
        }
    }
}

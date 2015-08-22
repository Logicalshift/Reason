using LogicalShift.Reason.Api;
using LogicalShift.Reason.Solvers;
using LogicalShift.Reason.Unification;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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

        [Test]
        public void AllocateReplacesVariables()
        {
            var executor = new ByteCodeExecutor(new ByteCodePoint[0], new ILiteral[0], 2);

            var someValue = Literal.NewAtom();
            executor.Register(0).SetTo(new SimpleReference(someValue, new SimpleReference()));
            Assert.AreEqual(someValue, executor.Register(0).Term, "Term is initially set");

            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 1, 0));
            Assert.AreNotEqual(someValue, executor.Register(0).Term, "Term is replaced by allocate");
        }

        [Test]
        public void AllocatePreservesArgumentsByMovingUp()
        {
            var executor = new ByteCodeExecutor(new ByteCodePoint[0], new ILiteral[0], 2);

            var someValue = Literal.NewAtom();
            executor.Register(0).SetTo(new SimpleReference(someValue, new SimpleReference()));
            Assert.AreEqual(someValue, executor.Register(0).Term, "Term is initially set");

            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 1, 1));
            Assert.AreEqual(someValue, executor.Register(1).Term, "Argument is preserved by allocate");
        }

        [Test]
        public void AllocateReplacesPermanentVariables()
        {
            var executor = new ByteCodeExecutor(new ByteCodePoint[0], new ILiteral[0], 3);

            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 2, 0));

            var someValue = Literal.NewAtom();
            executor.Register(1).SetTo(new SimpleReference(someValue, new SimpleReference()));
            Assert.AreEqual(someValue, executor.Register(1).Term, "Term is initially set");

            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 1, 0));
            Assert.AreNotEqual(someValue, executor.Register(0).Term);
            Assert.AreNotEqual(someValue, executor.Register(1).Term);
            Assert.AreNotEqual(someValue, executor.Register(2).Term);
        }

        [Test]
        public void AllocatePreservesArgumentsByMovingDown()
        {
            var executor = new ByteCodeExecutor(new ByteCodePoint[0], new ILiteral[0], 3);

            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 2, 0));

            var someValue = Literal.NewAtom();
            executor.Register(2).SetTo(new SimpleReference(someValue, new SimpleReference()));
            Assert.AreEqual(someValue, executor.Register(2).Term, "Term is initially set");

            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 1, 1));
            Assert.AreEqual(someValue, executor.Register(1).Term, "Argument is preserved by allocate");
        }

        [Test]
        public void PermanentVariablesAreRestored()
        {
            var executor = new ByteCodeExecutor(new ByteCodePoint[0], new ILiteral[0], 2);

            // Allocate a permanent variable
            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 1, 0));

            // Set it to something
            var someValue = Literal.NewAtom();
            executor.Register(0).SetTo(new SimpleReference(someValue, new SimpleReference()));
            Assert.AreEqual(someValue, executor.Register(0).Term, "Term is initially set");

            // Allocate another frame
            executor.Dispatch(new ByteCodePoint(Operation.Allocate, 1, 0));
            Assert.AreNotEqual(someValue, executor.Register(0).Term, "Term is replaced by allocate");

            // Restore the original value using deallocate
            executor.Dispatch(new ByteCodePoint(Operation.Deallocate));
            Assert.AreEqual(someValue, executor.Register(0).Term);
        }

        [Test]
        public async Task SolveSimplePredicate()
        {
            // Knowledge is a(x)
            var a = Literal.NewFunctor(1);
            var x = Literal.NewAtom();
            var aOfX = a.With(x);

            var knowledge = KnowledgeBase.New().Assert(Clause.Always(aOfX));

            // Generate a bytecode solver for this
            var solver = new ByteCodeSolver();
            await solver.Compile(knowledge);

            // Call it with the query a(Y)
            var refToY = new SimpleReference();

            var solve = solver.Call(a, refToY);

            var solved = solve();
            Assert.IsTrue(solved);
        }
    }
}

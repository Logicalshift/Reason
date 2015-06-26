using Logicalshift.SLD.Api;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logicalshift.SLD.Tests
{
    [TestFixture]
    public class Truthiness
    {
        [Test]
        public void TrueIsTrue()
        {
            Assert.IsTrue(Equals(Literal.True(), Literal.True()));
        }

        [Test]
        public void TrueHashesToSelf()
        {
            var dict = new Dictionary<ILiteral, bool>();
            dict[Literal.True()] = true;
            Assert.IsTrue(dict[Literal.True()]);
        }
    }
}

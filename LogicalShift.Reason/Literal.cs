using Logicalshift.Reason.Api;
using Logicalshift.Reason.Literals;
using System;

namespace Logicalshift.Reason
{
    /// <summary>
    /// Methods for creating and manipulating literals
    /// </summary>
    public static class Literal
    {
        /// <summary>
        /// Creates a new atomic literal
        /// </summary>
        public static ILiteral NewAtom()
        {
            return new BasicAtom();
        }

        /// <summary>
        /// The literal meaning 'true'
        /// </summary>
        public static ILiteral True()
        {
            return TrueLiteral.Value;
        }
    }
}

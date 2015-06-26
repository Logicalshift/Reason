using Logicalshift.SLD.Api;
using Logicalshift.SLD.Literals;
using System;

namespace Logicalshift.SLD
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

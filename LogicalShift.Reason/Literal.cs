using LogicalShift.Reason.Api;
using LogicalShift.Reason.Literals;
using System;

namespace LogicalShift.Reason
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

        /// <summary>
        /// Creates a new variable literal
        /// </summary>
        public static ILiteral NewVariable()
        {
            return new Variable();
        }

        /// <summary>
        /// Creates a new term with the specified number of parameters
        /// </summary>
        public static UnboundTerm NewTerm(int numParameters)
        {
            return new UnboundTerm(numParameters);
        }

        /// <summary>
        /// Applies parameters to a term
        /// </summary>
        public static BoundTerm With(this UnboundTerm target, params ILiteral[] parameters)
        {
            return new BoundTerm(target, parameters);
        }
    }
}

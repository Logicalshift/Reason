using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Low-level operations for a program unifier
    /// </summary>
    public interface IProgramUnifier
    {
        /// <summary>
        /// Retrieves a structure into a variable and sets read or write mode
        /// </summary>
        void GetStructure(ILiteral termName, int termLength, ILiteral variable);

        /// <summary>
        /// Retrieves the value of a variable that hasn't been read before
        /// </summary>
        void UnifyVariable(ILiteral variable);

        /// <summary>
        /// Unifies a variable that has been previously read
        /// </summary>
        void UnifyValue(ILiteral variable);
    }
}

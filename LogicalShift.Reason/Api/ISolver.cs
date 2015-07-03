using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Api
{
	/// <summary>
	/// Interface implemented by objects that can solve for goals
	/// </summary>
    public interface ISolver
    {
		/// <summary>
		/// Attempts to solve for a particular set of goals
		/// </summary>
        Task<IQueryResult> Solve(IEnumerable<ILiteral> goals);

        /// <summary>
        /// 'Calls' a particular predicate in this solver with the specified arguments. After the
        /// returned function is complete, the arguments are updated to their unified value.
        /// </summary>
        /// <param name="predicate">The literal representing the predicate to call</param>
        /// <param name="arguments">The arguments to unify with the predicate</param>
        /// <returns>
        /// A function that can be used to update the reference literals to the various possible results.
        /// </returns>
        Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments);
    }
}

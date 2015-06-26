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
    }
}

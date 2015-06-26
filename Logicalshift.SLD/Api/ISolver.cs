using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logicalshift.SLD.Api
{
	/// <summary>
	/// Inteface implemented by objects that can solve for goals
	/// </summary>
    public interface ISolver
    {
		/// <summary>
		/// Attempts to solve for a particular goal
		/// </summary>
        Task<IQueryResult> Solve(ILiteral goal);
    }
}

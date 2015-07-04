using LogicalShift.Reason.Api;
using System;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Solver that always returns false
    /// </summary>
    public class NothingSolver : ISolver
    {
        public Task<IQueryResult> Solve(System.Collections.Generic.IEnumerable<ILiteral> goals)
        {
            throw new System.NotImplementedException();
        }

        public Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments)
        {
            return () => false;
        }
    }
}

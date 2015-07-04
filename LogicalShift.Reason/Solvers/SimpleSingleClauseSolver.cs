using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Represents a solver that can solve a single clause
    /// </summary>
    public class SimpleSingleClauseSolver : ISolver
    {
        /// <summary>
        /// The clause that this will solve
        /// </summary>
        private readonly IClause _clause;

        /// <summary>
        /// The solver that will be used for subclauses
        /// </summary>
        private readonly ISolver _subclauseSolver;

        public SimpleSingleClauseSolver(IClause clause, ISolver subclauseSolver)
        {
            if (clause == null) throw new ArgumentNullException("clause");
            if (subclauseSolver == null) throw new ArgumentNullException("subclauseSolver");

            _clause = clause;
            _subclauseSolver = subclauseSolver;
        }

        public Task<IQueryResult> Solve(IEnumerable<ILiteral> goals)
        {
            throw new NotImplementedException();
        }

        public Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}

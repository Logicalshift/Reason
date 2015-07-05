using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Solver that maps predicates to lists of solvers that can resolve it
    /// </summary>
    public class SimpleDispatchingSolver : ISolver
    {
        /// <summary>
        /// Maps unification keys to the solver to use
        /// </summary>
        private readonly Dictionary<ILiteral, List<ISolver>> _solverForUnificationKey = new Dictionary<ILiteral,List<ISolver>>();

        public SimpleDispatchingSolver()
        {
        }

        /// <summary>
        /// Creates solvers in this object from clauses in a knowledge base
        /// </summary>
        public async Task LoadFromKnowledgeBase(IKnowledgeBase knowledge, Func<IClause, ISolver> solverForClause = null)
        {
            if (knowledge == null) throw new ArgumentNullException("knowledge");

            // By default, use the simple single clause solver
            if (solverForClause == null)
            {
                solverForClause = clause => new SimpleSingleClauseSolver(clause, this);
            }

            // Fetch the clauses from the knowledge base
            var clauses = await knowledge.GetClauses();

            // Fill the dictionary
            var solvers = clauses.Select(clause => new { Key = clause.Implies.UnificationKey, Solver = solverForClause(clause) });

            foreach (var solver in solvers)
            {
                if (solver.Key == null) continue;

                List<ISolver> solversForKey;
                if (!_solverForUnificationKey.TryGetValue(solver.Key, out solversForKey))
                {
                    solversForKey = new List<ISolver>();
                    _solverForUnificationKey.Add(solver.Key, solversForKey);
                }

                solversForKey.Add(solver.Solver);
            }
        }

        public Task<IQueryResult> Solve(IEnumerable<ILiteral> goals)
        {
            throw new NotImplementedException();
        }

        public Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments)
        {
            List<ISolver> solvers;

            // Result is nothing if there are no solvers that match this key
            if (!_solverForUnificationKey.TryGetValue(predicate, out solvers))
            {
                return () => false;
            }

            // Call each solver in turn
            IEnumerator<ISolver> currentSolver = solvers.GetEnumerator();
            Func<bool> currentResult = () => false;

            return () =>
            {
                for (;;)
                {
                    // Result is true if the current solver is still returning results
                    if (currentResult())
                    {
                        return true;
                    }

                    // Get the next result
                    if (!currentSolver.MoveNext())
                    {
                        // No more solvers to try
                        return false;
                    }

                    // Call the next solver
                    // TODO: one slight problem here is we expect the solver to backtrack the argument values before returning false
                    // (which is a performance loss for the very last solver)
                    currentResult = currentSolver.Current.Call(predicate, arguments);
                }
            };
        }
    }
}

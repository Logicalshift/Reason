using Logicalshift.SLD.Api;
using Logicalshift.SLD.Solvers;
using System;
using System.Collections.Generic;

namespace Logicalshift.SLD
{
    /// <summary>
    /// Methods for creating and altering solvers
    /// </summary>
    public static class Solver
    {
        /// <summary>
        /// Creates a goal solver that works against the specified knowledge base
        /// </summary>
        public static ISolver NewSolver(this IKnowledgeBase knowledge)
        {
            return new ForwardChainingSolver(knowledge);
        }
    }
}

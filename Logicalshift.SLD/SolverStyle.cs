using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logicalshift.SLD
{
    /// <summary>
    /// Styles of solver that can be used
    /// </summary>
    public enum SolverStyle
    {
        /// <summary>
        /// Solver that works by starting from the goal and working backwards
        /// </summary>
        BackwardsChaining,

        /// <summary>
        /// Solver that works by starting from the knowledge base and working forwards to discover if the goals are true
        /// </summary>
        ForwardsChaining
    }
}

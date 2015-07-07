using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Solver that encodes a knowledge base as bytecode in order to execute queries more quickly
    /// </summary>
    public class ByteCodeSolver : ISolver
    {
        public Func<bool> Call(ILiteral predicate, params IReferenceLiteral[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}

using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// A query result where the next result is retrieved by a function
    /// </summary>
    public class ChainedResult : IQueryResult
    {
        /// <summary>
        /// A query result that stores the results for this query
        /// </summary>
        private readonly IQueryResult _basicResult;

        /// <summary>
        /// A function that retrieves the next query result
        /// </summary>
        private readonly Func<Task<IQueryResult>> _nextResult;

        public ChainedResult(IQueryResult basicResult, Func<Task<IQueryResult>> nextResult)
        {
            if (basicResult == null) throw new ArgumentNullException("basicResult");
            if (nextResult == null) throw new ArgumentNullException("nextResult");

            _basicResult = basicResult;
            _nextResult = nextResult;
        }

        public bool Success
        {
            get { return _basicResult.Success; }
        }

        public IBindings Bindings
        {
            get { return _basicResult.Bindings; }
        }

        public async Task<IQueryResult> Next()
        {
            return new ChainedResult(await _nextResult(), _nextResult);
        }
    }
}

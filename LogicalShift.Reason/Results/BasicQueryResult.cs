using Logicalshift.Reason.Api;
using System;
using System.Threading.Tasks;

namespace Logicalshift.Reason.Results
{
    /// <summary>
    /// Represents a simple succeeded/failed query result
    /// </summary>
    public class BasicQueryResult : IQueryResult
    {
        private readonly bool _success;

        public BasicQueryResult(bool succeeded)
        {
            _success = succeeded;
        }

        public bool Success
        {
            get { return _success; }
        }

        public Task<IQueryResult> Next()
        {
            // No further results
            return Task.FromResult<IQueryResult>(null);
        }
    }
}

using System;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Represents a single result for a query
    /// </summary>
    public interface IQueryResult
    {
        /// <summary>
        /// True if a result was successfully retrieved
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Retrieves the next result
        /// </summary>
        Task<IQueryResult> Next();
    }
}

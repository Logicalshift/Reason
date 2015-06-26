using System;
using System.Threading.Tasks;

namespace Logicalshift.SLD.Api
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
        /// The result that was retrieved, or null if there are no more results
        /// </summary>
        ILiteral Result { get; }

        /// <summary>
        /// Retrieves the next result
        /// </summary>
        Task<IQueryResult> Next();
    }
}

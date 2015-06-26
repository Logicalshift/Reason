using System;
using System.Collections.Generic;

namespace Logicalshift.SLD.Api
{
    /// <summary>
    /// Represents a knowledge base
    /// </summary>
    public interface IKnowledgeBase
    {
        /// <summary>
        /// The set of clauses that make up this knowledge base
        /// </summary>
        IEnumerable<IClause> Clauses { get; }
    }
}

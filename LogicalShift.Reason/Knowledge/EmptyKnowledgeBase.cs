using Logicalshift.Reason.Api;
using System;
using System.Collections.Generic;

namespace Logicalshift.Reason.Knowledge
{
    /// <summary>
    /// A knowledge base with no information in it
    /// </summary>
    public class EmptyKnowledgeBase : IKnowledgeBase
    {
        public IEnumerable<IClause> Clauses
        {
            get { yield break; }
        }
    }
}

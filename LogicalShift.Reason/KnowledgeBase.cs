using LogicalShift.Reason.Api;
using LogicalShift.Reason.Knowledge;
using System;

namespace LogicalShift.Reason
{
    /// <summary>
    /// Static methods for creating/managing a knowledge base
    /// </summary>
    public static class KnowledgeBase
    {
        /// <summary>
        /// Creates a new knowledge base
        /// </summary>
        public static IKnowledgeBase New()
        {
            return new EmptyKnowledgeBase();
        }

        /// <summary>
        /// Generates a new knowledge base with an additional assertion
        /// </summary>
        public static IKnowledgeBase Assert(this IKnowledgeBase oldKnowledge, IClause assertion)
        {
            return new ListKnowledgeBase(assertion, oldKnowledge);
        }
    }
}

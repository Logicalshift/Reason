using Logicalshift.SLD.Api;
using System;
using System.Collections.Generic;

namespace Logicalshift.SLD.Clauses
{
    /// <summary>
    /// Represents a positive Horn clause
    /// </summary>
    public class PositiveClause : IClause
    {
        private readonly IClause _negativeClause;
        private readonly ILiteral _implies;

        public PositiveClause(IClause negativeClause, ILiteral implies)
        {
            if (negativeClause == null) throw new ArgumentNullException("negativeClause");
            if (implies == null) throw new ArgumentNullException("implies");

            _negativeClause = negativeClause;
            _implies = implies;
        }

        public IEnumerable<ILiteral> If
        {
            get { return _negativeClause.If; }
        }

        public ILiteral Implies
        {
            get { return _implies; }
        }
    }
}

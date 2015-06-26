using Logicalshift.SLD.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logicalshift.SLD.Clauses
{
    /// <summary>
    /// Represents a negative Horn clause
    /// </summary>
    public class NegativeClause : IClause
    {
        private readonly ILiteral[] _if;

        public NegativeClause(IEnumerable<ILiteral> ifParts)
        {
            if (ifParts == null) throw new ArgumentNullException("ifParts");

            _if = ifParts.ToArray();
        }

        public IEnumerable<ILiteral> If
        {
            get { return _if; }
        }

        public ILiteral Implies
        {
            get { return null; }
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            bool first = true;
            foreach (var item in If)
            {
                if (!first)
                {
                    result.Append(", ");
                }
                result.AppendFormat("({0})", item);
                first = false;
            }

            return result.ToString();
        }
    }
}

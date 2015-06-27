using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Literal representing truth
    /// </summary>
    public class TrueLiteral : ILiteral, IEquatable<TrueLiteral>
    {
        private readonly static TrueLiteral _value = new TrueLiteral();

        public static TrueLiteral Value { get { return _value; } }

        private TrueLiteral() { }

        public override int GetHashCode()
        {
            return true.GetHashCode();
        }

        public void UnifyQuery(IQueryUnifier unifier)
        {
            unifier.PutStructure(this, 0, this);
        }

        public void UnifyProgram(IProgramUnifier unifier)
        {
            unifier.GetStructure(this, 0, this);
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            // We don't have any parameters
            return this;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TrueLiteral);
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as TrueLiteral);
        }

        public bool Equals(TrueLiteral other)
        {
            if (other == null) return false;
            return true;
        }

        public override string ToString()
        {
            return "true";
        }
    }
}

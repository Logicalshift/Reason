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

        public IEnumerable<IUnificationState> Unify(ILiteral unifyWith, IUnificationState state)
        {
            if (!Equals(this, unifyWith))
            {
                // No valid unification states
                yield break;
            }
            else
            {
                // Existing state remains valid
                yield return state;
            }
        }

        public ILiteral Bind(IUnificationState state)
        {
            return this;
        }
    }
}

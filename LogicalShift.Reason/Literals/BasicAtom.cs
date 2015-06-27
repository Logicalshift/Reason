using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Represents a basic atomic literal
    /// </summary>
    public class BasicAtom : ILiteral, IEquatable<BasicAtom>
    {
        /// <summary>
        /// A unique identifier for this atom 
        /// </summary>
        private readonly long _identifier;

        /// <summary>
        /// The identifier to assign to the next atom
        /// </summary>
        private static long _nextIdentifier;

        public BasicAtom()
        {
            // Assign a unique (within this session) identifier to this atom
            _identifier = Interlocked.Increment(ref _nextIdentifier);
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

        public bool Equals(BasicAtom other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other._identifier == _identifier) return true;
            return false;
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as BasicAtom);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BasicAtom);
        }

        public override int GetHashCode()
        {
            return _identifier.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("atom_{0}", _identifier);
        }
    }
}

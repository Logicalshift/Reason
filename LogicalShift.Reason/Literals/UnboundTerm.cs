using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Represents a term with n parameters (a term with 1 parameter might be written T/1, or T(X))
    /// </summary>
    public class UnboundTerm : ILiteral, IEquatable<UnboundTerm>
    {
        /// <summary>
        /// A literal giving the name for this term
        /// </summary>
        private readonly ILiteral _name;

        /// <summary>
        /// The number of parameters in this term
        /// </summary>
        private readonly int _numParameters;

        public UnboundTerm(int numParameters)
        {
            _name = Literal.NewAtom();
            _numParameters = numParameters;
        }

        public UnboundTerm(ILiteral name, int numParameters)
        {
            if (name == null) throw new ArgumentNullException("name");

            _name = name;
            _numParameters = numParameters;
        }

        /// <summary>
        /// THe name of this term
        /// </summary>
        public ILiteral Name { get { return _name; } }

        /// <summary>
        /// The number of parameters for this term
        /// </summary>
        public int NumParameters { get { return _numParameters; } }

        public void UnifyQuery(IQueryUnifier unifier)
        {
            unifier.PutStructure(this, _numParameters, this);

            // Parameters are unbound, so just use a bunch of free variables
            for (int param = 0; param < _numParameters; ++param)
            {
                new Variable().UnifyQuery(unifier);
            }
        }

        public void UnifyProgram(IProgramUnifier unifier)
        {
            unifier.GetStructure(this, _numParameters, this);

            // Parameters are unbound, so just use a bunch of free variables
            for (int param = 0; param < _numParameters; ++param)
            {
                new Variable().UnifyProgram(unifier);
            }
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            return new BoundTerm(this, parameters);
        }

        public ILiteral UnificationKey
        {
            get 
            { 
                return this; 
            }
        }

        public bool Equals(UnboundTerm other)
        {
            if (other == null) return false;

            return Equals(_name, other._name) && _numParameters == other._numParameters;
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as UnboundTerm);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnboundTerm);
        }

        public override int GetHashCode()
        {
            var nameHash = _name.GetHashCode();
            var paramHash = _numParameters.GetHashCode();

            return nameHash * 397 + paramHash;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", _name, _numParameters);
        }
    }
}

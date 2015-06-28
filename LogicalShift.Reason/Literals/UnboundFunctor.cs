using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Represents a functor with n parameters (a functor with 1 parameter might be written T/1, or T(X))
    /// </summary>
    public class UnboundFunctor : ILiteral, IEquatable<UnboundFunctor>
    {
        /// <summary>
        /// A literal giving the name for this functor
        /// </summary>
        private readonly ILiteral _name;

        /// <summary>
        /// The number of parameters in this functor
        /// </summary>
        private readonly int _numParameters;

        public UnboundFunctor(int numParameters)
        {
            _name = Literal.NewAtom();
            _numParameters = numParameters;
        }

        public UnboundFunctor(ILiteral name, int numParameters)
        {
            if (name == null) throw new ArgumentNullException("name");

            _name = name;
            _numParameters = numParameters;
        }

        /// <summary>
        /// The name of this functor
        /// </summary>
        public ILiteral Name { get { return _name; } }

        /// <summary>
        /// The number of parameters for this functor
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
            return new BoundFunctor(this, parameters);
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get
            {
                for (int param =0; param < _numParameters; ++param)
                {
                    yield return new Variable();
                }
            }
        }

        public ILiteral UnificationKey
        {
            get 
            { 
                return this; 
            }
        }

        public IEnumerable<IAssignmentLiteral> Flatten()
        {
            var parameters = Enumerable.Range(0, _numParameters).Select(num => new Variable());
            var assign = new BoundFunctor(this, parameters);

            // Assign the parameters
            yield return new TermAssignment(new Variable(), assign);
            
            // Each parameter is a free variable
            foreach (var param in parameters)
            {
                yield return new VariableAssignment(param, new Variable());
            }
        }

        public bool Equals(UnboundFunctor other)
        {
            if (other == null) return false;

            return Equals(_name, other._name) && _numParameters == other._numParameters;
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as UnboundFunctor);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnboundFunctor);
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

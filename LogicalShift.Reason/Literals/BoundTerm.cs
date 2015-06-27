using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Literal representing a term bound to parameters
    /// </summary>
    public class BoundTerm : ILiteral, IEquatable<BoundTerm>
    {
        /// <summary>
        /// The parameters that this term is bound to
        /// </summary>
        private readonly ILiteral[] _parameters;

        /// <summary>
        /// The unbound variant of this term
        /// </summary>
        private readonly UnboundTerm _unbound;

        public BoundTerm(params ILiteral[] parameters)
        {
            _parameters = parameters;
            _unbound = new UnboundTerm(_parameters.Length);
        }

        public BoundTerm(ILiteral name, IEnumerable<ILiteral> parameters)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (parameters == null) throw new ArgumentNullException("parameters");

            _parameters = parameters.ToArray();
            _unbound = new UnboundTerm(name, _parameters.Length);
        }

        public BoundTerm(UnboundTerm unbound, IEnumerable<ILiteral> parameters)
        {
            if (unbound == null) throw new ArgumentNullException("unbound");
            if (parameters == null) throw new ArgumentNullException("parameters");

            _unbound = unbound;
            _parameters = parameters.ToArray();

            if (_parameters.Length != _unbound.NumParameters) throw new ArgumentException("Incorrect number of parameters for term", "parameters");
        }

        public void UnifyQuery(IQueryUnifier unifier)
        {
            unifier.PutStructure(_unbound, _unbound.NumParameters, _unbound);
            
            foreach (var param in _parameters)
            {
                param.UnifyQuery(unifier);
            }
        }

        public void UnifyProgram(IProgramUnifier unifier)
        {
            unifier.GetStructure(_unbound, _unbound.NumParameters, _unbound);

            foreach (var param in _parameters)
            {
                param.UnifyProgram(unifier);
            }
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            return new BoundTerm(_unbound, parameters);
        }

        public ILiteral UnificationKey
        {
            get { return _unbound; }
        }

        public bool Equals(BoundTerm other)
        {
            if (other == null) return false;

            if (!Equals(_unbound, other._unbound)) return false;

            for (int paramIndex = 0; paramIndex < _parameters.Length; ++paramIndex)
            {
                if (!Equals(_parameters[paramIndex], other._parameters[paramIndex]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as BoundTerm);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoundTerm);
        }

        public override int GetHashCode()
        {
            int result = _unbound.GetHashCode();

            foreach (var param in _parameters)
            {
                result *= 397;
                result += param.GetHashCode();
            }

            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendFormat("{0}(", _unbound.Name);

            bool first = true;
            foreach (var param in _parameters)
            {
                if (!first) result.Append(", ");

                result.Append(param.ToString());
            }
            result.Append(")");

            return result.ToString();
        }
    }
}

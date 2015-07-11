using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Literal representing a functor bound to parameters
    /// </summary>
    public class BoundFunctor : ILiteral, IEquatable<BoundFunctor>
    {
        /// <summary>
        /// The parameters that this functor is bound to
        /// </summary>
        private readonly ILiteral[] _parameters;

        /// <summary>
        /// The unbound variant of this functor
        /// </summary>
        private readonly UnboundFunctor _unbound;

        public BoundFunctor(params ILiteral[] parameters)
        {
            _parameters = parameters;
            _unbound = new UnboundFunctor(_parameters.Length);
        }

        public BoundFunctor(ILiteral name, IEnumerable<ILiteral> parameters)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (parameters == null) throw new ArgumentNullException("parameters");

            _parameters = parameters.ToArray();
            _unbound = new UnboundFunctor(name, _parameters.Length);
        }

        public BoundFunctor(UnboundFunctor unbound, IEnumerable<ILiteral> parameters)
        {
            if (unbound == null) throw new ArgumentNullException("unbound");
            if (parameters == null) throw new ArgumentNullException("parameters");

            _unbound = unbound;
            _parameters = parameters.ToArray();

            if (_parameters.Length != _unbound.NumParameters) throw new ArgumentException("Incorrect number of parameters for functor", "parameters");
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            return new BoundFunctor(_unbound, parameters);
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get
            {
                return _parameters;
            }
        }

        public ILiteral UnificationKey
        {
            get { return _unbound; }
        }

        public IEnumerable<IAssignmentLiteral> Flatten()
        {
            // Start by flattening all of the parameters
            var flattenedParameters = _parameters.Select(param => param.Flatten().ToArray()).ToArray();

            // Get the list of variables that the parameters are assigned to
            var parameterTargets = flattenedParameters.Select(list => list.First().Variable);

            // Merge any parameter assignments that assign to the same value
            var eliminator = new AssignmentEliminator(flattenedParameters.SelectMany(item => item));
            eliminator.Eliminate();
            parameterTargets = parameterTargets.Select(eliminator.MapVariable);

            // Generate the assignment for this term
            var target = new Variable();
            yield return new TermAssignment(target, new BoundFunctor(_unbound, parameterTargets));

            // Generate the list of parameter assignments
            foreach (var param in eliminator.Assignments)
            {
                yield return param;
            }
        }

        public bool Equals(BoundFunctor other)
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
            return Equals(other as BoundFunctor);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoundFunctor);
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
                first = false;
            }
            result.Append(")");

            return result.ToString();
        }
    }
}

using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Assignment
{
    /// <summary>
    /// Represents an assignment to a variable (ie, X1 = X style assignments)
    /// </summary>
    public class VariableAssignment : IAssignmentLiteral, IEquatable<VariableAssignment>
    {
        private readonly ILiteral _target;
        private readonly ILiteral _variable;

        public VariableAssignment(ILiteral target, ILiteral variable)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (variable == null) throw new ArgumentNullException("variable");

            _target = target;
            _variable = variable;
        }

        public ILiteral Variable
        {
            get { return _target; }
        }

        public ILiteral Value
        {
            get { return _variable; }
        }

        public bool CompileQuery(IQueryUnifier query)
        {
            return true;
        }

        public bool CompileProgram(IProgramUnifier program)
        {
            return true;
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            var vals = parameters.ToArray();
            return new VariableAssignment(vals[0], vals[1]);
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get 
            {
                yield return _target;
                yield return _variable; 
            }
        }

        public ILiteral UnificationKey
        {
            get { return this; }
        }

        public IEnumerable<IAssignmentLiteral> Flatten()
        {
            yield return this;
        }

        public IAssignmentLiteral Remap(Func<ILiteral, ILiteral> valueForLiteral)
        {
            return new VariableAssignment(valueForLiteral(_target), valueForLiteral(_variable));
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as VariableAssignment);
        }

        public bool Equals(VariableAssignment other)
        {
            if (other == null) return false;
            return Equals(_target, other._target) && Equals(_variable, other._variable);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VariableAssignment);
        }

        public override int GetHashCode()
        {
            return _target.GetHashCode() * 397 + _variable.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", _target, _variable);
        }
    }
}

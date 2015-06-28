using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

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

        public void CompileQuery(IQueryUnifier query)
        {
        }

        public void CompileProgram(IProgramUnifier program)
        {
        }

        public void UnifyQuery(IQueryUnifier unifier)
        {
            throw new NotImplementedException();
        }

        public void UnifyProgram(IProgramUnifier unifier)
        {
            throw new NotImplementedException();
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            return this;
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get 
            {
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

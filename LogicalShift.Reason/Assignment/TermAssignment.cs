using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Assignment
{
    /// <summary>
    /// Represents an assignment of the form X1 = t(X2, X3, X4)
    /// </summary>
    public class TermAssignment : IAssignmentLiteral, IEquatable<TermAssignment>
    {
        private readonly ILiteral _target;
        private readonly ILiteral _assignTo;

        public TermAssignment(ILiteral target, ILiteral assignTo)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (assignTo == null) throw new ArgumentNullException("assignTo");

            _target = target;
            _assignTo = assignTo;
        }

        public ILiteral Variable
        {
            get { return _target; }
        }

        public ILiteral Value
        {
            get { return _assignTo; }
        }

        public void CompileQuery(IQueryUnifier query)
        {
            // The dependencies of the assignTo value indicate the parameters of this term
            var terms = _assignTo.Dependencies.ToArray();

            // Put the structure
            query.PutStructure(_assignTo.UnificationKey, terms.Length, _target);

            // Put the variables
            foreach (var termVariable in terms)
            {
                if (query.HasVariable(termVariable))
                {
                    query.SetValue(termVariable);
                }
                else
                {
                    query.SetVariable(termVariable);
                }
            }
        }

        public void CompileProgram(IProgramUnifier program)
        {
            // The dependencies of the assignTo value indicate the parameters of this term
            var terms = _assignTo.Dependencies.ToArray();

            // Put the structure
            program.GetStructure(_assignTo.UnificationKey, terms.Length, _target);

            // Put the variables
            foreach (var termVariable in terms)
            {
                if (program.HasVariable(termVariable))
                {
                    program.UnifyValue(termVariable);
                }
                else
                {
                    program.UnifyVariable(termVariable);
                }
            }
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
                yield return _assignTo;
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
            return Equals(other as TermAssignment);
        }

        public bool Equals(TermAssignment other)
        {
            if (other == null) return false;
            return Equals(_target, other._target) && Equals(_assignTo, other._assignTo);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TermAssignment);
        }

        public override int GetHashCode()
        {
            return _target.GetHashCode() * 397 + _assignTo.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", _target, _assignTo);
        }
    }
}

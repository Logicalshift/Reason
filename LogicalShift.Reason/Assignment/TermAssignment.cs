﻿using LogicalShift.Reason.Api;
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

        public bool CompileQuery(IQueryUnifier query)
        {
            // The dependencies of the assignTo value indicate the parameters of this term
            var terms = _assignTo.Dependencies.ToArray();

            // Put the structure
            if (!query.PutStructure(_assignTo.UnificationKey, terms.Length, _target)) return false;

            // Put the variables
            foreach (var termVariable in terms)
            {
                if (query.HasVariable(termVariable))
                {
                    if (!query.SetValue(termVariable)) return false;
                }
                else
                {
                    if (!query.SetVariable(termVariable)) return false;
                }
            }

            return true;
        }

        public bool CompileProgram(IProgramUnifier program)
        {
            // The dependencies of the assignTo value indicate the parameters of this term
            var terms = _assignTo.Dependencies.ToArray();

            // Put the structure
            if (!program.GetStructure(_assignTo.UnificationKey, terms.Length, _target)) return false;

            // Put the variables
            foreach (var termVariable in terms)
            {
                if (program.HasVariable(termVariable))
                {
                    if (!program.UnifyValue(termVariable)) return false;
                }
                else
                {
                    if (!program.UnifyVariable(termVariable)) return false;
                }
            }

            return true;
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            var vals = parameters.ToArray();
            return new TermAssignment(vals[0], vals[1]);
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get 
            {
                yield return _target;
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

        public IAssignmentLiteral Remap(Func<ILiteral, ILiteral> valueForLiteral)
        {
            // Remap the dependencies
            var dependencies = _assignTo.Dependencies.Select(valueForLiteral);

            // Turn into a new assignment
            return new TermAssignment(valueForLiteral(_target), _assignTo.RebuildWithParameters(dependencies));
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

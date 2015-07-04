using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Assignment
{
    /// <summary>
    /// Represents the assignment of a variable to an argument
    /// </summary>
    public class ArgumentAssignment : IAssignmentLiteral, IEquatable<ArgumentAssignment>
    {
        private readonly ILiteral _argument;
        private readonly ILiteral _variable;

        public ArgumentAssignment(ILiteral argument, ILiteral variable)
        {
            if (argument == null) throw new ArgumentNullException("argument");
            if (variable == null) throw new ArgumentNullException("variable");

            _argument = argument;
            _variable = variable;
        }

        public ILiteral Variable
        {
            get { return _argument; }
        }

        public ILiteral Value
        {
            get { return _variable; }
        }

        public void CompileQuery(IQueryUnifier query)
        {
            if (query.HasVariable(_argument))
            {
                query.PutValue(_variable, _argument);
            }
            else
            {
                query.PutVariable(_variable, _argument);
            }
        }

        public void CompileProgram(IProgramUnifier program)
        {
            if (program.HasVariable(_argument))
            {
                program.GetValue(_variable, _argument);
            }
            else
            {
                program.GetVariable(_variable, _argument);
            }
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            var vals = parameters.ToArray();
            return new ArgumentAssignment(vals[0], vals[1]);
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get 
            {
                yield return _argument;
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
            return new VariableAssignment(valueForLiteral(_argument), valueForLiteral(_variable));
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as ArgumentAssignment);
        }

        public bool Equals(ArgumentAssignment other)
        {
            if (other == null) return false;
            return Equals(_argument, other._argument) && Equals(_variable, other._variable);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ArgumentAssignment);
        }

        public override int GetHashCode()
        {
            return _argument.GetHashCode() * 397 + _variable.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", _argument, _variable);
        }
    }
}

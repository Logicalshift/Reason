using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Represents a basic variable binding set
    /// </summary>
    public class BasicBinding : IBindings
    {
        /// <summary>
        /// The result of the binding
        /// </summary>
        private readonly ILiteral _result;

        /// <summary>
        /// The value bindings
        /// </summary>
        private readonly Dictionary<ILiteral, ILiteral> _valueForVariable;

        public BasicBinding(ILiteral result, Dictionary<ILiteral, ILiteral> valueForVariable)
        {
            _result = result;
            _valueForVariable = valueForVariable;
        }

        public ILiteral Result
        {
            get { return _result; }
        }

        public IEnumerable<ILiteral> Variables
        {
            get { return _valueForVariable.Keys; }
        }

        public ILiteral GetValueForVariable(ILiteral variable)
        {
            ILiteral result;
            if (_valueForVariable.TryGetValue(variable, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}

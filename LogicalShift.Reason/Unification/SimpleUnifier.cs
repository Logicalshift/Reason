using LogicalShift.Reason.Api;
using LogicalShift.Reason.Literals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Basic implementation of a unifier
    /// </summary>
    public class SimpleUnifier : IQueryUnifier, IProgramUnifier, IUnifier
    {
        /// <summary>
        /// Set of variables that have been used before
        /// </summary>
        private readonly HashSet<int> _usedVariables = new HashSet<int>();

        /// <summary>
        /// Returns the names assigned to particular variables
        /// </summary>
        private readonly Dictionary<ILiteral, IReferenceLiteral> _addressForName = new Dictionary<ILiteral, IReferenceLiteral>();

        /// <summary>
        /// Maps variable names to their index
        /// </summary>
        private readonly Dictionary<ILiteral, int> _indexForVariable = new Dictionary<ILiteral, int>();

        /// <summary>
        /// Variables in order
        /// </summary>
        private readonly List<IReferenceLiteral> _variables = new List<IReferenceLiteral>();

        /// <summary>
        /// True if running a program in write mode, false otherwise
        /// </summary>
        bool _writeMode = false;

        /// <summary>
        /// Pointer to the next value within the current structure
        /// </summary>
        private IReferenceLiteral _structurePtr = null;

        /// <summary>
        /// Pointer to the last built argument
        /// </summary>
        private ArgumentReference _lastArgument = null;

        /// <summary>
        /// Prepares to run a new program unifier using this object
        /// </summary>
        public void PrepareToRunProgram()
        {
            _usedVariables.Clear();
            _structurePtr = null;
        }

        /// <summary>
        /// Loads the supplied arguments into the variables numbered 0 and up. This should
        /// be called before <see cref="BindVariable"></see> (as it would otherwise detach
        /// the variable binding)
        /// </summary>
        public void LoadArguments(IEnumerable<IReferenceLiteral> arguments)
        {
            int index = 0;

            foreach (var argumentValue in arguments)
            {
                if (_variables.Count <= index)
                {
                    _variables.Add(argumentValue);
                }
                else
                {
                    _variables[index] = argumentValue;
                }

                ++index;
            }
        }

        /// <summary>
        /// Reads the first count variables
        /// </summary>
        public IReferenceLiteral[] GetArgumentVariables(int count)
        {
            return _variables.Take(count).ToArray();
        }

        public bool HasVariable(ILiteral name)
        {
            return _usedVariables.Contains(_indexForVariable[name]);
        }

        public void BindVariable(int index, ILiteral variableName)
        {
            while (_variables.Count <= index)
            {
                _variables.Add(new SimpleReference());
            }

            _addressForName[variableName] = _variables[index];
            _indexForVariable[variableName] = index;
        }

        public void PutStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            // Mark this variable as used
            _usedVariables.Add(_indexForVariable[variable]);

            // Create the structure
            ArgumentReference firstArgument = null;

            for (int argNum = 0; argNum < termLength; ++argNum)
            {
                var newArgument = new ArgumentReference(null, firstArgument);
                firstArgument = newArgument;
            }

            var structure = new SimpleReference(termName, firstArgument);

            _lastArgument = firstArgument;

            // Store in the variable
            _addressForName[variable].SetTo(structure);
        }

        public void SetVariable(ILiteral variable)
        {
            // Mark the variable as used
            _usedVariables.Add(_indexForVariable[variable]);

            // Create a new reference
            IReferenceLiteral newReference;

            // Add to the current structure
            if (_lastArgument != null)
            {
                // The last argument is the reference
                newReference = _lastArgument;
                _lastArgument = _lastArgument.NextArgument;
            }
            else
            {
                newReference = new SimpleReference();
            }

            // Store in the variable
            _addressForName[variable].SetTo(newReference);
        }

        public void SetValue(ILiteral variable)
        {
            // Read the variable
            var variableValue = _addressForName[variable];
            
            // Store to the current structure
            if (_lastArgument != null)
            {
                _lastArgument.SetTo(variableValue);
                _lastArgument = _lastArgument.NextArgument;
            }
        }

        public void PutVariable(ILiteral variable1, ILiteral variable2)
        {
            _usedVariables.Add(_indexForVariable[variable1]);
            _usedVariables.Add(_indexForVariable[variable2]);

            var newValue = new SimpleReference();

            // Store in the variables
            _addressForName[variable1].SetTo(newValue);
            _addressForName[variable2].SetTo(newValue);

            // Store to the current structure
            if (_lastArgument != null)
            {
                _lastArgument.SetTo(newValue);
                _lastArgument = _lastArgument.NextArgument;
            }
        }

        public void PutValue(ILiteral variable1, ILiteral variable2)
        {
            _addressForName[variable2].SetTo(_addressForName[variable1]);
        }

        public void GetStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            // This variable becomes used
            _usedVariables.Add(_indexForVariable[variable]);

            // Get the dereferenced address of the variable
            var heapValue = _addressForName[variable].Dereference();

            // Action depends on what's at that address
            if (heapValue.IsVariable())
            {
                // Variable is an unbound ref cell: bind it to a new value that we create
                ArgumentReference firstArgument = null;

                for (int argNum = 0; argNum < termLength; ++argNum)
                {
                    var newArgument = new ArgumentReference(null, firstArgument);
                    firstArgument = newArgument;
                }

                var newStructure = new SimpleReference(termName, firstArgument);
                _lastArgument = firstArgument;
                _structurePtr = firstArgument;

                Bind(heapValue, newStructure);
                _writeMode = true;
            }
            else if (!heapValue.IsReference())
            {
                if (Equals(heapValue.Term, termName))
                {
                    // Set the structure pointer, and use read mode
                    _structurePtr = heapValue.Reference;
                    _writeMode = false;
                }
                else
                {
                    // Structure doesn't match; fail
                    throw new InvalidOperationException();
                }
            }
            else
            {
                // Fail
                throw new InvalidOperationException();
            }
        }

        public void UnifyVariable(ILiteral variable)
        {
            // This variable becomes used
            _usedVariables.Add(_indexForVariable[variable]);

            if (!_writeMode)
            {
                // Just read the value of the variable
                _addressForName[variable].SetTo(_structurePtr);
            }
            else
            {
                // Write the value of the variable
                _addressForName[variable].SetTo(_lastArgument);
                _lastArgument = _lastArgument.NextArgument;
            }

            _structurePtr = _structurePtr.NextArgument;
        }

        public void UnifyValue(ILiteral variable)
        {
            if (!_writeMode)
            {
                Unify(_addressForName[variable], _structurePtr);
            }
            else
            {
                _lastArgument.SetTo(_addressForName[variable]);
                _lastArgument = _lastArgument.NextArgument;
            }

            _structurePtr = _structurePtr.NextArgument;
        }

        public void GetVariable(ILiteral variable1, ILiteral variable2)
        {
            _usedVariables.Add(_indexForVariable[variable1]);
            _usedVariables.Add(_indexForVariable[variable2]);

            _addressForName[variable1].SetTo(_addressForName[variable2]);
        }

        public void GetValue(ILiteral variable1, ILiteral variable2)
        {
            Unify(_addressForName[variable1], _addressForName[variable2]);
        }

        /// <summary>
        /// Binds a value to the heap
        /// </summary>
        private void Bind(IReferenceLiteral target, IReferenceLiteral value)
        {
            target.SetTo(value);
        }

        /// <summary>
        /// Unifies a value on the heap
        /// </summary>
        private void Unify(IReferenceLiteral address1, IReferenceLiteral address2)
        {
            var unifyStack = new Stack<IReferenceLiteral>();

            // Push the addresses that we're going to unify
            unifyStack.Push(address1);
            unifyStack.Push(address2);

            // Iterate until the stack is empty
            while (unifyStack.Count > 0)
            {
                // Deref the values on the stack
                var value1 = unifyStack.Pop().Dereference();
                var value2 = unifyStack.Pop().Dereference();

                if (!ReferenceEquals(value1, value2))
                {
                    if (value1.IsReference() || value2.IsReference())
                    {
                        // Bind references
                        Bind(value1, value2);
                    }
                    else
                    {
                        if (Equals(value1.Term, value2.Term))
                        {
                            // Process the rest of the structure
                            var structurePos1 = value1.Reference;
                            var structurePos2 = value2.Reference;

                            while (structurePos1 != null)
                            {
                                unifyStack.Push(structurePos1);
                                unifyStack.Push(structurePos2);

                                structurePos1 = structurePos1.NextArgument;
                                structurePos2 = structurePos2.NextArgument;
                            }
                        }
                        else
                        {
                            // Structures do not match: fail
                            throw new InvalidOperationException();
                        }
                    }
                }
            }
        }

        public IQueryUnifier QueryUnifier
        {
            get { return this; }
        }

        public IProgramUnifier ProgramUnifier
        {
            get { return this; }
        }

        public ILiteral UnifiedValue(ILiteral name)
        {
            return _addressForName[name].Freeze();
        }

        public IReferenceLiteral GetVariable(ILiteral name)
        {
            return _addressForName[name];
        }
    }
}

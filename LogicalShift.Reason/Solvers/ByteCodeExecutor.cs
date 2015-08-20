using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using System;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Executes a bytecode program
    /// </summary>
    public class ByteCodeExecutor
    {
        /// <summary>
        /// The program that this will run
        /// </summary>
        private readonly ByteCodePoint[] _program;

        /// <summary>
        /// Maps literal identifiers to literal objects
        /// </summary>
        private readonly ILiteral[] _literals;

        /// <summary>
        /// The address of the instruction that is being executed
        /// </summary>
        private int _programCounter;

        /// <summary>
        /// The environment for the program
        /// </summary>
        private ByteCodeEnvironment _environment;

        /// <summary>
        /// The variable registers
        /// </summary>
        private SimpleReference[] _registers;

        /// <summary>
        /// True if we're in 'write mode'
        /// </summary>
        private bool _writeMode;

        /// <summary>
        /// Pointer to the last built argument
        /// </summary>
        private IReferenceLiteral _lastArgument;

        /// <summary>
        /// Pointer to the next value within the current structure
        /// </summary>
        private IReferenceLiteral _structurePtr;

        /// <summary>
        /// The trail for the current backtracking operation
        /// </summary>
        private NoTrail _trail = new NoTrail();

        public ByteCodeExecutor(ByteCodePoint[] program, ILiteral[] literals, int maxVariableIndex)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));

            _program        = program;
            _literals       = literals;
            _programCounter = 0;
            _environment    = new ByteCodeEnvironment(0, 0, null);
            _registers      = new SimpleReference[maxVariableIndex];

            for (var registerIndex = 0; registerIndex<_registers.Length; ++registerIndex)
            {
                _registers[registerIndex] = new SimpleReference();
            }
        }

        /// <summary>
        /// Executes a single instruction
        /// </summary>
        public void Step()
        {
            // Fetch the next opcode
            var address = _programCounter;

            // Advance the program counter
            ++_programCounter;

            // Dispatch the instruction
            Dispatch(_program[address]);
        }

        /// <summary>
        /// Returns the contents of register
        /// </summary>
        public SimpleReference Register(int register)
        {
            return _registers[register];
        }

        /// <summary>
        /// Dispatches a particular code point
        /// </summary>
        public void Dispatch(ByteCodePoint codePoint)
        {
            // Action depends on the opcode
            switch (codePoint.Op)
            {
                case Operation.Nothing:
                    break;

                case Operation.GetStructure:
                    GetStructure(codePoint.Literal, codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.GetVariable:
                    GetVariable(codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.GetValue:
                    GetValue(codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.PutStructure:
                    PutStructure(codePoint.Literal, codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.PutVariable:
                    PutVariable(codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.PutValue:
                    PutValue(codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.SetVariable:
                    SetVariable(codePoint.Arg1);
                    break;

                case Operation.SetValue:
                    SetValue(codePoint.Arg1);
                    break;

                case Operation.UnifyVariable:
                    UnifyVariable(codePoint.Arg1);
                    break;

                case Operation.UnifyValue:
                    UnifyValue(codePoint.Arg1);
                    break;

                case Operation.Allocate:
                    Allocate(codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.Deallocate:
                    Deallocate();
                    break;

                case Operation.Proceed:
                    Proceed();
                    break;

                case Operation.CallAddress:
                    CallAddress(codePoint.Arg1, codePoint.Arg2);
                    break;

                case Operation.TryMeElse:
                case Operation.RetryMeElse:
                case Operation.TrustMe:
                    throw new NotImplementedException("Opcode not implemented yet");

                case Operation.Call:
                    throw new NotImplementedException("External calls not yet supported");

                default:
                    throw new NotImplementedException("Unknown opcode");
            }
        }

        /// <summary>
        /// Calls a routine at a particular address
        /// </summary>
        private void CallAddress(int address, int numArguments)
        {
            _environment.ContinuationPointer = _programCounter;
            _programCounter = address;
        }

        /// <summary>
        /// Continues execution using the continuation pointer
        /// </summary>
        private void Proceed()
        {
            _programCounter = _environment.ContinuationPointer;
        }

        /// <summary>
        /// Deallocates the last allocated block, restoring the permanent variables from the preceding environment
        /// </summary>
        private void Deallocate()
        {
            var oldEnvironment = _environment.ContinuationEnvironment;
            if (oldEnvironment == null) return;

            // Restore any permanent variables from the new environment
            for (var varIndex = 0; varIndex < oldEnvironment.Variables.Length; ++varIndex)
            {
                _registers[varIndex] = oldEnvironment.Variables[varIndex];
            }

            // Reallocate any temporary variables that have newly appeared
            for (var varIndex = oldEnvironment.Variables.Length; varIndex < _environment.Variables.Length; ++varIndex)
            {
                _registers[varIndex] = new SimpleReference();
            }

            // Finally, restore the environment
            _environment = oldEnvironment;
        }

        /// <summary>
        /// Allocates space for a number of permanent and argument variables.
        /// </summary>
        /// <remarks>
        /// Permanent variables are stored in the environment and are numbered from 0.
        /// Arguments occur after the temporary variables and have their values preserved
        /// from the previous state by this call.
        /// </remarks>
        private void Allocate(int numPermanent, int numArguments)
        {
            // Allocate a new environment
            var newEnvironment = new ByteCodeEnvironment(numPermanent, numArguments, _environment);
            newEnvironment.ContinuationEnvironment = _environment;

            // Make sure that we don't overwrite arguments in the previous environment by replacing them with new temporary variables
            if (numPermanent + numArguments < _environment.Variables.Length)
            {
                for (int oldPermanent = numPermanent + numArguments; oldPermanent < _environment.Variables.Length; ++oldPermanent)
                {
                    _registers[oldPermanent] = new SimpleReference();
                }
            }

            // Move arguments to their new position
            int oldArgStart = _environment.Variables.Length;

            if (oldArgStart > numPermanent)
            {
                // Moving arguments 'down', leaving a hole
                for (int argument = 0; argument < numArguments; ++argument)
                {
                    _registers[argument + numPermanent] = _registers[argument + oldArgStart];
                    _registers[argument + oldArgStart] = new SimpleReference();
                }
            }
            else if (oldArgStart < numPermanent)
            {
                // Moving arguments 'up', old locations will be overwritten by new permanent variables
                for (int argument = numArguments-1; argument >=0; --argument)
                {
                    _registers[argument + numPermanent] = _registers[argument + oldArgStart];
                }
            }

            // Copy in the new permanent variables
            for (int permanent = 0; permanent < numPermanent; ++permanent)
            {
                _registers[permanent] = newEnvironment.Variables[permanent];
            }

            _environment = newEnvironment;
        }

        /// <summary>
        /// In write mode, writes the value of a variable to the current structure. In read mode,
        /// unifies the value in the current structure with the value of a variable.
        /// </summary>
        private bool UnifyValue(int variable)
        {
            if (!_writeMode)
            {
                if (!_registers[variable].Unify(_structurePtr, _trail))
                {
                    return false;
                }
            }
            else
            {
                _lastArgument.SetTo(_registers[variable]);
                _lastArgument = _lastArgument.NextArgument;
            }

            _structurePtr = _structurePtr.NextArgument;

            return true;
        }

        /// <summary>
        /// Stores the value of a variable in the current structure
        /// </summary>
        private void SetValue(int variable)
        {
            var variableValue = _registers[variable];

            // Store to the current structure
            if (_lastArgument != null)
            {
                _lastArgument.SetTo(variableValue);
                _lastArgument = _lastArgument.NextArgument;
            }
        }

        /// <summary>
        /// Cretes a new reference and stores it in the current structure and a particular variable
        /// </summary>
        private void SetVariable(int variable)
        {
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
            _registers[variable].SetTo(newReference);
        }

        /// <summary>
        /// Stores a new reference in two variables and the current structure
        /// </summary>
        private void PutVariable(int variable, int argument)
        {
            var newValue = new SimpleReference();

            // Store in the variables
            _registers[variable].SetTo(newValue);
            _registers[argument].SetTo(newValue);

            // Store to the current structure
            if (_lastArgument != null)
            {
                _lastArgument.SetTo(newValue);
                _lastArgument = _lastArgument.NextArgument;
            }
        }

        /// <summary>
        /// Generates a new empty structure of a particular length on the heap and stores a reference
        /// to it in a variable
        /// </summary>
        private void PutStructure(int termLiteral, int variable, int termLength)
        {
            var termName = _literals[termLiteral];

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
            _registers[variable].SetTo(structure);
        }

        /// <summary>
        /// Unifies two variables
        /// </summary>
        private void GetValue(int variable, int argument)
        {
            _registers[variable].Unify(_registers[argument], _trail);
        }

        /// <summary>
        /// Sets variable to the value of argument
        /// </summary>
        private void GetVariable(int variable, int argument)
        {
            _registers[variable].SetTo(_registers[argument]);
        }

        /// <summary>
        /// Sets variable to the value of argument
        /// </summary>
        private void PutValue(int variable, int argument)
        {
            _registers[argument].SetTo(_registers[variable]);
        }

        /// <summary>
        /// Either begins unifying against an existing structure (if the variable is bound) or
        /// begins writing a new structure to an unbound variable.
        /// </summary>
        private bool GetStructure(int literal, int variable, int termLength)
        {
            var termName = _literals[literal];

            // Get the dereferenced address of the variable
            var heapValue = _registers[variable].Dereference();

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
                    return false;
                }
            }
            else
            {
                // Fail
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unifies a variable
        /// </summary>
        private void UnifyVariable(int variable)
        {
            if (!_writeMode)
            {
                // Just read the value of the variable into the variable
                _registers[variable].SetTo(_structurePtr);
            }
            else
            {
                // Write the value of the variable
                _registers[variable].SetTo(_lastArgument);
                _lastArgument = _lastArgument.NextArgument;
            }

            _structurePtr = _structurePtr.NextArgument;
        }

        /// <summary>
        /// Binds one value to anoteher
        /// </summary>
        private void Bind(IReferenceLiteral target, IReferenceLiteral value)
        {
            _trail.Record(target);
            target.SetTo(value);
        }
    }
}

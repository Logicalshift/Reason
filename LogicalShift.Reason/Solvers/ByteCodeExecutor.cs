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
            _environment    = new ByteCodeEnvironment(0, null);
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

            // Action depends on the opcode
            switch (_program[address].Op)
            {
                case Operation.Nothing:
                    break;

                case Operation.GetStructure:
                    GetStructure(_program[address].Literal, _program[address].Arg1, _program[address].Arg2);
                    break;

                case Operation.GetVariable:
                    GetVariable(_program[address].Arg1, _program[address].Arg2);
                    break;

                case Operation.GetValue:
                    GetValue(_program[address].Arg1, _program[address].Arg2);
                    break;

                case Operation.PutStructure:
                case Operation.PutVariable:
                case Operation.PutValue:
                case Operation.SetVariable:
                case Operation.SetValue:
                    throw new NotImplementedException("Opcode not implemented yet");

                case Operation.UnifyVariable:
                    UnifyVariable(_program[address].Arg1);
                    break;

                case Operation.UnifyValue:
                case Operation.Allocate:
                case Operation.Deallocate:
                case Operation.Proceed:
                case Operation.CallAddress:
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
        /// Unifies two variables
        /// </summary>
        private void GetValue(int arg1, int arg2)
        {
            _registers[arg1].Unify(_registers[arg2], _trail);
        }

        /// <summary>
        /// Sets var1 to the value of var2
        /// </summary>
        private void GetVariable(int var1, int var2)
        {
            _registers[var1].SetTo(_registers[var2]);
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
                // Just read the value of the variable
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

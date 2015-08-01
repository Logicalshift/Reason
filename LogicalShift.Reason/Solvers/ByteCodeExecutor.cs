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

        public ByteCodeExecutor(ByteCodePoint[] program, int maxVariableIndex)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));

            _program        = program;
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
                case Operation.GetVariable:
                case Operation.GetValue:
                case Operation.PutStructure:
                case Operation.PutVariable:
                case Operation.PutValue:
                case Operation.SetVariable:
                case Operation.SetValue:
                case Operation.UnifyVariable:
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
    }
}

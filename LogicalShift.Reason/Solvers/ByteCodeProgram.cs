using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Represents a bytecode program
    /// </summary>
    public class ByteCodeProgram
    {
        /// <summary>
        /// The operations making up the program
        /// </summary>
        private readonly List<ByteCodePoint> _program = new List<ByteCodePoint>();

        /// <summary>
        /// Maps indexes to literals (eg, for GetStructure)
        /// </summary>
        private readonly List<ILiteral> _literals = new List<ILiteral>();

        /// <summary>
        /// Maps literals to their identifiers
        /// </summary>
        private readonly Dictionary<ILiteral, int> _literalIdentifier = new Dictionary<ILiteral, int>();

        /// <summary>
        /// Maps instruction pointers to the instructions that reference a particular label
        /// </summary>
        private readonly Dictionary<object, List<int>> _ipsReferencingLabel = new Dictionary<object, List<int>>();

        /// <summary>
        /// Maps labels to the instruction point that they are attached to
        /// </summary>
        private readonly Dictionary<object, int> _ipsWithLabel = new Dictionary<object, int>();

        /// <summary>
        /// The maximum bound variable index
        /// </summary>
        private int _maxVariableIndex = -1;

        /// <summary>
        /// Returns the index of the highest variable value that this program can use
        /// </summary>
        public int GetMaxVariableIndex()
        {
            if (_maxVariableIndex < 0)
            {
                // Read the program to find the max variable index
                foreach (var codepoint in _program)
                {
                    switch (codepoint.Op)
                    {
                        case Operation.PutStructure:
                        case Operation.SetVariable:
                        case Operation.SetValue:
                        case Operation.GetStructure:
                        case Operation.UnifyVariable:
                        case Operation.UnifyValue:
                            if (codepoint.Arg1 > _maxVariableIndex) _maxVariableIndex = codepoint.Arg1;
                            break;

                        case Operation.PutVariable:
                        case Operation.PutValue:
                        case Operation.GetVariable:
                        case Operation.GetValue:
                            if (codepoint.Arg1 > _maxVariableIndex) _maxVariableIndex = codepoint.Arg1;
                            if (codepoint.Arg2 > _maxVariableIndex) _maxVariableIndex = codepoint.Arg2;
                            break;

                        default:
                            // Doesn't use variables
                            break;
                    }
                }
            }

            return _maxVariableIndex;
        }

        /// <summary>
        /// Retrieves the code point at a particular instruction offset
        /// </summary>
        public ByteCodePoint this[int ip]
        {
            get { return _program[ip]; }
        }

        /// <summary>
        /// The current length of the program
        /// </summary>
        public int Count
        {
            get { return _program.Count; }
        }

        /// <summary>
        /// Retrieves the literal for a particular integer value
        /// </summary>
        public ILiteral LiteralAtIndex(int index)
        {
            return _literals[index];
        }

        /// <summary>
        /// Writes a new code point to this program
        /// </summary>
        public int Write(Operation op, int arg1 = 0, int arg2 = 0)
        {
            _maxVariableIndex = -1;
            _program.Add(new ByteCodePoint(op, arg1, arg2));
            return _program.Count - 1;
        }

        /// <summary>
        /// Writes a new code point to this program
        /// </summary>
        public int Write(Operation op, ILiteral literal, int arg1 = 0, int arg2 = 0)
        {
            int literalValue;
            if (!_literalIdentifier.TryGetValue(literal, out literalValue))
            {
                literalValue = _literalIdentifier[literal] = _literals.Count;
                _literals.Add(literal);
            }

            _program.Add(new ByteCodePoint(op, arg1, arg2, literalValue));
            _maxVariableIndex = -1;
            return _program.Count - 1;
        }

        /// <summary>
        /// Writes an operation where the first argument is the IP of the specified label
        /// </summary>
        public int WriteWithLabel(Operation op, object label)
        {
            int labelIp;
            int currentIp = _program.Count;
            if (_ipsWithLabel.TryGetValue(label, out labelIp))
            {
                // Label already encountered
                Write(op, labelIp-currentIp);
            }
            else
            {
                // Label not encountered
                List<int> labelInstructions;
                if (!_ipsReferencingLabel.TryGetValue(label, out labelInstructions))
                {
                    labelInstructions = _ipsReferencingLabel[label] = new List<int>();
                }

                labelInstructions.Add(_program.Count);
                Write(op, -1);
            }

            return _program.Count - 1;
        }

        /// <summary>
        /// Sets a label referencing the next instruction to be written
        /// </summary>
        public int Label(object label)
        {
            // Write the label
            var labelIp = _program.Count;
            _ipsWithLabel[label] = labelIp;
            
            // Update any instructions that reference the label
            List<int> existingReferences;
            if (_ipsReferencingLabel.TryGetValue(label, out existingReferences))
            {
                foreach (var refIp in existingReferences)
                {
                    var old = _program[refIp];
                    _program[refIp] = new ByteCodePoint(old.Op, labelIp-refIp, old.Arg2);
                }

                existingReferences.Clear();
            }

            return labelIp;
        }

        /// <summary>
        /// Clears any data structures used while writing this program, leaving only those needed to
        /// run it.
        /// </summary>
        public void Finish()
        {
            // Don't need to map labels any more
            _ipsWithLabel.Clear();
            _ipsReferencingLabel.Clear();

            // Don't need to map literals to values any more
            _literalIdentifier.Clear();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (var codePoint in _program)
            {
                if (result.Length > 0) result.Append('\n');
                result.AppendFormat("{0}/{1} {2}, {3}", codePoint.Op, codePoint.Literal, codePoint.Arg1, codePoint.Arg2);
            }

            return result.ToString();
        }
    }
}

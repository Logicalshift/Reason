using LogicalShift.Reason.Unification;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Represents an execution environment for a running bytecode program
    /// </summary>
    public class ByteCodeEnvironment
    {
        public ByteCodeEnvironment(int numVariables, int numArguments, ByteCodeEnvironment continuationEnvironment)
        {
            Variables = new SimpleReference[numVariables];
            NumberOfArguments = numArguments;
            for (var x=0; x<numVariables; ++x)
            {
                Variables[x] = new SimpleReference();
            }
            ContinuationEnvironment = continuationEnvironment;
            ContinuationPointer = -1;
        }

        /// <summary>
        /// Address of the instruction to return to during the next proceed
        /// </summary>
        public int ContinuationPointer;

        /// <summary>
        /// The environment that should be used once this environment has been finished with
        /// </summary>
        public ByteCodeEnvironment ContinuationEnvironment;

        /// <summary>
        /// The variables in this environment
        /// </summary>
        public readonly SimpleReference[] Variables;

        /// <summary>
        /// The number of arguments in this environment
        /// </summary>
        public int NumberOfArguments { get; private set; }
    }
}

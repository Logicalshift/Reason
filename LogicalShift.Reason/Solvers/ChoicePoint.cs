using LogicalShift.Reason.Api;
using LogicalShift.Reason.Unification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// Representation of a choice point
    /// </summary>
    public class ChoicePoint
    {
        /// <summary>
        /// Previous choice point on the current stack
        /// </summary>
        private readonly ChoicePoint _previousChoicePoint;

        /// <summary>
        /// The values of the argument registers when this choice point was created
        /// </summary>
        private readonly SimpleReference[] _arguments;

        /// <summary>
        /// The trail for this choice point
        /// </summary>
        private readonly ITrail _trail;

        /// <summary>
        /// The location of the next clause
        /// </summary>
        private readonly int _nextClause;

        /// <summary>
        /// The environment in use when this choice point was created
        /// </summary>
        private readonly ByteCodeEnvironment _environment;

        public ChoicePoint(ChoicePoint previousChoice, ByteCodeEnvironment environment, IEnumerable<IReferenceLiteral> arguments, ITrail trail, int nextClause)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (trail == null) throw new ArgumentNullException(nameof(trail));

            _previousChoicePoint    = previousChoice;
            _arguments              = arguments.Select(arg => new SimpleReference(arg)).ToArray();
            _trail                  = trail;
            _nextClause             = nextClause;
            _environment            = environment;
        }

        /// <summary>
        /// null, or the choice point that preceeds this one in the stack
        /// </summary>
        public ChoicePoint PreviousChoicePoint
        {
            get { return _previousChoicePoint; }
        }

        /// <summary>
        /// The values of the arguments when this choice point was made
        /// </summary>
        public IEnumerable<SimpleReference> Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// The trail that must be reset when this choice point is restored
        /// </summary>
        public ITrail Trail
        {
            get { return _trail; }
        }

        /// <summary>
        /// The next clause for this choice point
        /// </summary>
        public int NextClause
        {
            get { return _nextClause; }
        }

        /// <summary>
        /// The environment in use when this choice point was created
        /// </summary>
        public ByteCodeEnvironment Environment
        {
            get { return _environment; }
        }
    }
}

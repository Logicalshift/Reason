﻿using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Literal representing a variable clause
    /// </summary>
    public class Variable : ILiteral, IEquatable<Variable>
    {
        /// <summary>
        /// The identifier for this variable
        /// </summary>
        private readonly long _identifier;

        /// <summary>
        /// The identifier to assign to the next variable created
        /// </summary>
        private static long _nextIdentifier = 0;

        public Variable()
        {
            _identifier = Interlocked.Increment(ref _nextIdentifier);
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            // We don't have any parameters
            return this;
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get
            {
                yield break;
            }
        }

        public ILiteral UnificationKey
        {
            get
            {
                // Variables can be unified with anything, so they don't have a unification key
                return null;
            }
        }

        public IEnumerable<IAssignmentLiteral> Flatten()
        {
            yield return new VariableAssignment(new Variable(), this);
        }

        public bool Equals(Variable other)
        {
            if (other == null) return false;

            return other._identifier == _identifier;
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as Variable);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Variable);
        }

        public override int GetHashCode()
        {
            return _identifier.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("var_{0}", _identifier);
        }
    }
}

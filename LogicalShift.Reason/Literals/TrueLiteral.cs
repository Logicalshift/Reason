﻿using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Literal representing truth
    /// </summary>
    public class TrueLiteral : ILiteral, IEquatable<TrueLiteral>
    {
        private readonly static TrueLiteral _value = new TrueLiteral();

        public static TrueLiteral Value { get { return _value; } }

        private TrueLiteral() { }

        public override int GetHashCode()
        {
            return true.GetHashCode();
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
                // Simple atoms can be unified only with themselves
                return this;
            }
        }

        public IEnumerable<IAssignmentLiteral> Flatten()
        {
            yield return new TermAssignment(new Variable(), this);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TrueLiteral);
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as TrueLiteral);
        }

        public bool Equals(TrueLiteral other)
        {
            if (other == null) return false;
            return true;
        }

        public override string ToString()
        {
            return "true";
        }
    }
}

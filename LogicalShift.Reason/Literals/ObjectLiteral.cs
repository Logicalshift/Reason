using LogicalShift.Reason.Api;
using LogicalShift.Reason.Assignment;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Literals
{
    /// <summary>
    /// Literal made from a .NET object
    /// </summary>
    public class ObjectLiteral<TObjectType> : ILiteral, IEquatable<ObjectLiteral<TObjectType>>
    {
        /// <summary>
        /// The object that defines this literal
        /// </summary>
        private readonly TObjectType _object;

        public ObjectLiteral(TObjectType obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            _object = obj;
        }

        /// <summary>
        /// The object representing the value of this literal
        /// </summary>
        public TObjectType Value
        {
            get { return _object; }
        }

        public IEnumerable<IAssignmentLiteral> Flatten()
        {
            yield return new TermAssignment(new Variable(), this);
        }

        public ILiteral RebuildWithParameters(IEnumerable<ILiteral> parameters)
        {
            return this;
        }

        public IEnumerable<ILiteral> Dependencies
        {
            get { yield break; }
        }

        public ILiteral UnificationKey
        {
            get { return this; }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ObjectLiteral<TObjectType>);
        }

        public bool Equals(ILiteral other)
        {
            return Equals(other as ObjectLiteral<TObjectType>);
        }

        public bool Equals(ObjectLiteral<TObjectType> other)
        {
            if (other == null) return false;

            return Equals(_object, other._object);
        }

        public override int GetHashCode()
        {
            return _object.GetHashCode();
        }

        public override string ToString()
        {
            return _object.ToString();
        }
    }
}

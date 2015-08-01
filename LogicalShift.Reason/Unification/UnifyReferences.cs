using LogicalShift.Reason.Api;
using System.Collections.Generic;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Provides support for unifying reference literals
    /// </summary>
    public static class UnifyReferences
    {

        /// <summary>
        /// Unifies a value on the heap
        /// </summary>
        public static bool Unify(this IReferenceLiteral address1, IReferenceLiteral address2, ITrail trail)
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
                    if (value1.IsReference())
                    {
                        // Bind references
                        trail.Record(value1);
                        value1.SetTo(value2);
                    }
                    else if (value2.IsReference())
                    {
                        trail.Record(value2);
                        value2.SetTo(value1);
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
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}

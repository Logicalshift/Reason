using LogicalShift.Reason.Api;
using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Unification
{
    /// <summary>
    /// Unifier that writes what's happening to the console
    /// </summary>
    public class TraceUnifier : IQueryUnifier, IProgramUnifier, IUnifier
    {
        private readonly IUnifier _unifier;

        public TraceUnifier(IUnifier unifier)
        {
            _unifier = unifier;
        }

        public void PutStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            Console.WriteLine("Q: put_structure {0}/{1} -> {2}", termName, termLength, variable);
            _unifier.QueryUnifier.PutStructure(termName, termLength, variable);
        }

        public void SetVariable(ILiteral variable)
        {
            Console.WriteLine("Q: set_variable {0}", variable);
            _unifier.QueryUnifier.SetVariable(variable);
        }

        public void SetValue(ILiteral variable)
        {
            Console.WriteLine("Q: set_value {0}", variable);
            _unifier.QueryUnifier.SetValue(variable);
        }

        public bool HasVariable(ILiteral name)
        {
            // TODO: we really should choose the implementation based on the type of unifier that's being called
            return _unifier.QueryUnifier.HasVariable(name) || _unifier.ProgramUnifier.HasVariable(name);
        }

        public void BindVariable(int index, ILiteral variableName)
        {
            // TODO: we really should choose the implementation based on the type of unifier that's being called
            _unifier.QueryUnifier.BindVariable(index, variableName);
            _unifier.ProgramUnifier.BindVariable(index, variableName);
        }

        public void GetStructure(ILiteral termName, int termLength, ILiteral variable)
        {
            Console.WriteLine("P: get_structure {0}/{1} -> {2}", termName, termLength, variable);
            _unifier.ProgramUnifier.GetStructure(termName, termLength, variable);
        }

        public void UnifyVariable(ILiteral variable)
        {
            Console.WriteLine("P: unify_variable {0}", variable);
            _unifier.ProgramUnifier.UnifyVariable(variable);
        }

        public void UnifyValue(ILiteral variable)
        {
            Console.WriteLine("P: unify_value {0}", variable);
            _unifier.ProgramUnifier.UnifyValue(variable);
        }

        public IQueryUnifier QueryUnifier
        {
            get { return this; }
        }

        public IProgramUnifier ProgramUnifier
        {
            get { return this; }
        }

        public ILiteral UnifiedValue(ILiteral variable)
        {
            return _unifier.UnifiedValue(variable);
        }

        public IReferenceLiteral GetVariable(int index)
        {
            // TODO: need to separate out the program unifier from the query unifier
            return _unifier.QueryUnifier.GetVariable(index);
        }
    }
}

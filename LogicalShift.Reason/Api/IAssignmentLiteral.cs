using System;
namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Literal representing an expression of the form VAR = EXPR
    /// </summary>
    /// <remarks>
    /// This is most commonly used when flattening an expression. For example, the expression f(g(X)) can be
    /// flattened to
    /// 
    ///   X1 = f(X2)
    ///   X2 = g(X3)
    ///   X3 = X
    /// 
    /// These assignments can then be compiled in the correct order
    /// </remarks>
    public interface IAssignmentLiteral : ILiteral
    {
        /// <summary>
        /// Literal representing the name of the variable being assigned to
        /// </summary>
        ILiteral Variable { get; }

        /// <summary>
        /// Literal representing the value being assigned
        /// </summary>
        ILiteral Value { get; }

        /// <summary>
        /// Compiles this assignment as a query
        /// </summary>
        bool CompileQuery(IQueryUnifier query);

        /// <summary>
        /// Compiles this assignment as a program
        /// </summary>
        bool CompileProgram(IProgramUnifier program);

        /// <summary>
        /// Remaps the variables within this assignment
        /// </summary>
        IAssignmentLiteral Remap(Func<ILiteral, ILiteral> valueForLiteral);
    }
}

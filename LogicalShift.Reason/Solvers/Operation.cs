namespace LogicalShift.Reason.Solvers
{
    /// <summary>
    /// An operation that can be performed by a bytecode solver
    /// </summary>
    public enum Operation : byte
    {
        /// <summary>
        /// The base no-op operation
        /// </summary>
        Nothing,

        // WAM instructions for writing a query

        /// <summary>
        /// Begins writing a structure, storing a reference to it in a variable
        /// </summary>
        PutStructure,
        
        /// <summary>
        /// Creates a new unbound reference and stores it in a variable and writes it to the current structure
        /// </summary>
        SetVariable,

        /// <summary>
        /// Writes the current value of a variable to the current structure
        /// </summary>
        SetValue,

        /// <summary>
        /// Creates a new unbound reference and writes it to the current structure, as well as storing it in
        /// the two supplied variables.
        /// </summary>
        PutVariable,

        /// <summary>
        /// Stores the value of one variable in another
        /// </summary>
        PutValue,

        // WAM instructions for unifying a predicate with a query

        /// <summary>
        /// Performs an action depending on the value pointed to by dereferencing a variable.
        /// 
        /// If it's an unbound reference, we begin building a new structure and enter write mode.
        /// If it's a structure, we check that it matches the structure referred to in the supplied variable and leave write mode.
        /// </summary>
        GetStructure,

        /// <summary>
        /// Continues writing/reading the structure from GetStructure
        /// 
        /// In write mode: creates a new unbound reference and adds it to the structure, and stores a reference to it in the variable
        /// In read mode: reads the next value from the structure into the variable
        /// </summary>
        UnifyVariable,

        /// <summary>
        /// Continues writing/reading the structure from GetStructure
        /// 
        /// In write mode: writes the value of the supplied variable to the structure.
        /// In read mode: unifies the value of the variable with the next value from the structure
        /// </summary>
        UnifyValue,

        /// <summary>
        /// Stores the value of one variable into another
        /// </summary>
        GetVariable,

        /// <summary>
        /// Unifies two variables
        /// </summary>
        GetValue,

        // WAM control instructions

        /// <summary>
        /// Calls a predicate and sets the continuation pointer to the next instruction
        /// </summary>
        Call,

        /// <summary>
        /// Calls an address in the current program
        /// </summary>
        CallAddress,

        /// <summary>
        /// Sets the current instruction to the continuation pointer
        /// </summary>
        Proceed,

        /// <summary>
        /// Creates space for a number of variables and stores the current continuation pointer
        /// </summary>
        Allocate,

        /// <summary>
        /// Discards the variables allocated by the last allocate instruction and restores the values that were
        /// there before.
        /// </summary>
        Deallocate,

        /// <summary>
        /// Creates a choice point between the next instruction and a particular code point
        /// </summary>
        TryMeElse,

        /// <summary>
        /// Backtrack to the last choice point and try the alternative path
        /// </summary>
        RetryMeElse,

        /// <summary>
        /// Backtrack to the last choice point and discard it
        /// </summary>
        TrustMe
    }
}

﻿using System;
using System.Collections.Generic;

namespace LogicalShift.Reason.Api
{
    /// <summary>
    /// Interface implemented by objects representing a unifier
    /// </summary>
    public interface IUnifier
    {
        /// <summary>
        /// Returns the unifier used to build the query
        /// </summary>
        IQueryUnifier QueryUnifier { get; }

        /// <summary>
        /// Returns the unifier used to build the program
        /// </summary>
        IProgramUnifier ProgramUnifier { get; }
    }
}

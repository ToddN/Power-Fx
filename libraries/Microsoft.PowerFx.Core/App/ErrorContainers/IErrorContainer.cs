﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AppMagic.Common;
using System.Collections.Generic;

namespace Microsoft.AppMagic.Authoring.Texl
{
    internal interface IErrorContainer
    {
        /// <summary>
        /// The severity to use in the default EnsureError and Error functions. Is not
        /// used in the Errors function.
        /// </summary>
        DocumentErrorSeverity DefaultSeverity { get; }

        /// <summary>
        /// Only adds and returns the error if its severity is equal to or higher
        /// than the existing errors for the node in the container.
        ///
        /// Severity is defaulted to critical.
        /// </summary>
        TexlError EnsureError(TexlNode node, StringResources.ErrorResourceKey errKey, params object[] args);

        /// <summary>
        /// Adds an error to the container and returns the composed error value
        /// that was inserted.
        ///
        /// Severity is defaulted to critical.
        /// </summary>
        TexlError Error(TexlNode node, StringResources.ErrorResourceKey errKey, params object[] args);

        /// <summary>
        /// Only adds and returns the error if its severity is equal to or higher
        /// than the existing errors for the node in the container.
        /// </summary>
        TexlError EnsureError(DocumentErrorSeverity severity, TexlNode node, StringResources.ErrorResourceKey errKey, params object[] args);

        /// <summary>
        /// Adds an error to the container and returns the composed error value
        /// that was inserted.
        /// </summary>
        TexlError Error(DocumentErrorSeverity severity, TexlNode node, StringResources.ErrorResourceKey errKey, params object[] args);

        /// <summary>
        /// Used to apply errors due to differing type schemas. Use schemaDifferenceType = DType.Invalid to indicate
        /// that the schema difference is due to a missing member.
        /// </summary>
        void Errors(TexlNode node, DType nodeType, KeyValuePair<string, DType> schemaDifference, DType schemaDifferenceType);
    }
}

﻿// ------------------------------------------------------------------------------
//  <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------

namespace Microsoft.PowerFx.LanguageServerProtocol.Protocol
{
    public class CompletionParams: TextDocumentPositionParams
    {
        public CompletionParams()
        {
            Context = new CompletionContext();
        }

        /// <summary>
        /// The completion context. This is only available if the client specifies
        /// to send this using the client capability
        /// `completion.contextSupport === true`
        /// </summary>
        public CompletionContext Context { get; set; }
    }
}

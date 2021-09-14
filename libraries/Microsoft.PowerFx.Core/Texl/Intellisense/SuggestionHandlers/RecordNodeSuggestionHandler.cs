﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.AppMagic.Authoring.Texl
{
    internal partial class Intellisense
    {
        /// <summary>
        /// Suggests operators that can be used on a value of type record or table.  E.g. In, As 
        /// </summary>
        internal sealed class RecordNodeSuggestionHandler : NodeKindSuggestionHandler
        {
            public RecordNodeSuggestionHandler() 
                : base(NodeKind.Record) 
            { }

            internal override bool TryAddSuggestionsForNodeKind(IntellisenseData intellisenseData)
            {
                Contracts.AssertValue(intellisenseData);

                TexlNode curNode = intellisenseData.CurNode;
                int cursorPos = intellisenseData.CursorPos;

                var tokenSpan = curNode.Token.Span;

                // Only suggest after record nodes
                if (cursorPos <= tokenSpan.Lim)
                    return true;

                if (IntellisenseHelper.CanSuggestAfterValue(cursorPos, intellisenseData.Script))
                {
                    // Verify that cursor is after a space after the current node's token.
                    // Suggest binary keywords.
                    IntellisenseHelper.AddSuggestionsForAfterValue(intellisenseData, DType.EmptyTable);
                }

                return true;
            }
        }
    }
}
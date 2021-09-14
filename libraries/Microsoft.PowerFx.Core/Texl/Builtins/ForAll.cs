﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // ForAll(source:*, formula)
    internal sealed class ForAllFunction : FunctionWithTableInput
    {
        public override bool SkipScopeForInlineRecords => true;
        public override bool IsSelfContained => true;
        public override bool RequiresErrorContext => true;

        public ForAllFunction()
            : base("ForAll", TexlStrings.AboutForAll, FunctionCategories.Table, DType.Unknown, 0x2, 2, 2, DType.EmptyTable)
        {
            ScopeInfo = new FunctionScopeInfo(this);
        }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.ForAllArg1, TexlStrings.ForAllArg2 };
        }

        public override bool CheckInvocation(TexlNode[] args, DType[] argTypes, IErrorContainer errors, out DType returnType)
        {
            Contracts.AssertValue(args);
            Contracts.AssertAllValues(args);
            Contracts.AssertValue(argTypes);
            Contracts.Assert(args.Length == argTypes.Length);
            Contracts.AssertValue(errors);

            bool fArgsValid = base.CheckInvocation(args, argTypes, errors, out returnType);

            if (argTypes[1].IsRecord)
            {
                returnType = argTypes[1].ToTable();
            }
            else if (argTypes[1].IsPrimitive || argTypes[1].IsTable)
            {
                returnType = DType.CreateTable(new TypedName(argTypes[1], ColumnName_Value));
            }
            else
            {
                returnType = DType.Error;
                fArgsValid = false;
            }

            return fArgsValid;
        }

        public override bool HasSuggestionsForParam(int index)
        {
            Contracts.Assert(0 <= index);

            return index == 0;
        }

        public override string GetUniqueTexlRuntimeName(bool isPrefetching = false)
        {
            return GetUniqueTexlRuntimeName(suffix: isPrefetching ? "_ParallelPrefetching" : "");
        }
    }
}
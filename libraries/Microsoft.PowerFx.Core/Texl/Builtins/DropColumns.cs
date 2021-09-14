﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // DropColumns(source:*[...], name:s, name:s, ...)
    internal sealed class DropColumnsFunction : FunctionWithTableInput
    {
        public override bool IsSelfContained => true;

        public DropColumnsFunction()
            : base("DropColumns", TexlStrings.AboutDropColumns, FunctionCategories.Table, DType.EmptyTable, 0, 2, int.MaxValue, DType.EmptyTable)
        { }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.DropColumnsArg1, TexlStrings.DropColumnsArg2 };
            yield return new [] { TexlStrings.DropColumnsArg1, TexlStrings.DropColumnsArg2, TexlStrings.DropColumnsArg2 };
            yield return new [] { TexlStrings.DropColumnsArg1, TexlStrings.DropColumnsArg2, TexlStrings.DropColumnsArg2, TexlStrings.DropColumnsArg2 };
        }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures(int arity)
        {
            if (arity > 2)
                return GetGenericSignatures(arity, TexlStrings.DropColumnsArg1, TexlStrings.DropColumnsArg2);
            return base.GetSignatures(arity);
        }

        public override bool CheckInvocation(TexlNode[] args, DType[] argTypes, IErrorContainer errors, out DType returnType)
        {
            Contracts.AssertValue(args);
            Contracts.AssertValue(argTypes);
            Contracts.Assert(args.Length == argTypes.Length);
            Contracts.AssertValue(errors);
            Contracts.Assert(MinArity <= args.Length && args.Length <= MaxArity);

            bool fArgsValid = base.CheckInvocation(args, argTypes, errors, out returnType);
            Contracts.Assert(returnType.IsTable);

            if (!argTypes[0].IsTable)
            {
                fArgsValid = false;
                errors.EnsureError(DocumentErrorSeverity.Severe, args[0], TexlStrings.ErrNeedTable_Func, Name);
            }
            else
                returnType = argTypes[0];

            // The result type has N fewer columns, as specified by (args[1],args[2],args[3],...)
            int count = args.Length;
            for (var i = 1; i < count; i++)
            {
                TexlNode nameArg = args[i];
                DType nameArgType = argTypes[i];

                // Verify we have a string literal for the column name. Accd to spec, we don't support
                // arbitrary expressions that evaluate to string values, because these values contribute to
                // type analysis, so they need to be known upfront (before DropColumns executes).
                StrLitNode nameNode;
                if (nameArgType.Kind != DKind.String || (nameNode = nameArg.AsStrLit()) == null)
                {
                    fArgsValid = false;
                    errors.EnsureError(DocumentErrorSeverity.Severe, nameArg, TexlStrings.ErrExpectedStringLiteralArg_Name, nameArg.ToString());
                    continue;
                }

                // Verify that the name is valid.
                if (!DName.IsValidDName(nameNode.Value))
                {
                    fArgsValid = false;
                    errors.EnsureError(DocumentErrorSeverity.Severe, nameArg, TexlStrings.ErrArgNotAValidIdentifier_Name, nameNode.Value);
                    continue;
                }

                DName columnName = new DName(nameNode.Value);

                // Verify that the name exists.
                DType columnType;
                if (!returnType.TryGetType(columnName, out columnType))
                {
                    fArgsValid = false;
                    returnType.ReportNonExistingName(FieldNameKind.Logical, errors, columnName, nameArg);
                    continue;
                }

                // Drop the specified column from the result type.
                bool fError = false;
                returnType = returnType.Drop(ref fError, DPath.Root, columnName);
                Contracts.Assert(!fError);
            }

            return fArgsValid;
        }

        // This method returns true if there are special suggestions for a particular parameter of the function.
        public override bool HasSuggestionsForParam(int argumentIndex)
        {
            Contracts.Assert(0 <= argumentIndex);

            return argumentIndex >= 0;
        }
    }
}

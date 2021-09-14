﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // Text(arg:n|s)
    // Text(arg:n|s, format:s)
    // Text(arg:n|s, format:s, language:s)
    // Corresponding DAX functions: Format, Fixed
    internal sealed class TextFunction : BuiltinFunction
    {
        public override bool SupportsParamCoercion => true;
        public override bool RequiresErrorContext => true;
        public override bool IsSelfContained => true;

        public TextFunction()
            : base("Text", TexlStrings.AboutText, FunctionCategories.Table | FunctionCategories.Text | FunctionCategories.DateTime, DType.String, 0, 1, 3, DType.Number, DType.String, DType.String)
        { }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.TextArg1, TexlStrings.TextArg2 };
            yield return new [] { TexlStrings.TextArg1, TexlStrings.TextArg2, TexlStrings.TextArg3 };
        }

        public override bool CheckInvocation(TexlNode[] args, DType[] argTypes, IErrorContainer errors, out DType returnType, out Dictionary<TexlNode, DType> nodeToCoercedTypeMap)
        {
            Contracts.AssertValue(args);
            Contracts.AssertAllValues(args);
            Contracts.AssertValue(argTypes);
            Contracts.Assert(args.Length == argTypes.Length);
            Contracts.AssertValue(errors);
            Contracts.Assert(MinArity <= args.Length && args.Length <= MaxArity);

            bool isValid = true;
            returnType = DType.String;
            nodeToCoercedTypeMap = null;

            TexlNode arg0 = args[0];
            DType arg0Type = argTypes[0];

            bool matchedWithCoercion;
            bool isValidString = true;
            bool isValidNumber = CheckType(arg0, arg0Type, DType.Number, DefaultErrorContainer, out matchedWithCoercion);
            DType arg0CoercedType = matchedWithCoercion ? DType.Number : DType.Invalid;

            if (!isValidNumber || matchedWithCoercion)
            {
                isValidString = CheckType(arg0, arg0Type, DType.String, DefaultErrorContainer, out matchedWithCoercion);

                if (isValidString)
                {
                    if (matchedWithCoercion)
                    {
                        // If both the matches were with coercion, we pick string over number.
                        // For instance Text(true) returns true in the case of EXCEL. If we picked
                        // number coercion, then we would return 1 and it will not match EXCEL behavior.
                        arg0CoercedType = DType.String;
                    }
                    else
                    {
                        arg0CoercedType = DType.Invalid;
                    }
                }
            }

            if (!isValidNumber && !isValidString)
            {
                errors.EnsureError(DocumentErrorSeverity.Severe, args[0], TexlStrings.ErrNumberOrStringExpected);
                isValid = false;
            }

            if (args.Length < 2)
            {
                if (isValid && arg0CoercedType.IsValid)
                {
                    CollectionUtils.Add(ref nodeToCoercedTypeMap, arg0, arg0CoercedType);
                    return true;
                }

                return isValid;
            }

            StrLitNode formatNode;
            if (!DType.String.Accepts(argTypes[1]))
            {
                errors.EnsureError(DocumentErrorSeverity.Severe, args[1], TexlStrings.ErrStringExpected);
                isValid = false;
            }
            else if ((formatNode = args[1].AsStrLit()) != null)
            {
                // Verify statically that the format string doesn't contain BOTH numeric and date/time
                // format specifiers. If it does, that's an error accd to Excel and our spec.
                string fmt = formatNode.Value;

                // But firstly skip any locale-prefix
                if (fmt.StartsWith("[$-"))
                {
                    int end = fmt.IndexOf(']', 3);
                    if (end > 0)
                        fmt = fmt.Substring(end + 1);
                }
                bool hasDateTimeFmt = fmt.IndexOfAny(new char[] { 'm', 'd', 'y', 'h', 'H', 's', 'a', 'A', 'p', 'P' }) >= 0;
                bool hasNumericFmt = fmt.IndexOfAny(new char[] { '0', '#' }) >= 0;
                if (hasDateTimeFmt && hasNumericFmt)
                {
                    errors.EnsureError(DocumentErrorSeverity.Moderate, formatNode, TexlStrings.ErrIncorrectFormat_Func, Name);
                    isValid = false;
                }
            }

            if (args.Length > 2)
            {
                DType argType = argTypes[2];
                if (!DType.String.Accepts(argType))
                {
                    errors.EnsureError(DocumentErrorSeverity.Severe, args[2], TexlStrings.ErrStringExpected);
                    isValid = false;
                }
            }

            if (isValid)
            {
                if (arg0CoercedType.IsValid)
                {
                    CollectionUtils.Add(ref nodeToCoercedTypeMap, arg0, arg0CoercedType);
                    return true;
                }
            }
            else
            {
                nodeToCoercedTypeMap = null;
            }

            return isValid;
        }

        // This method returns true if there are special suggestions for a particular parameter of the function.
        public override bool HasSuggestionsForParam(int argumentIndex)
        {
            Contracts.Assert(0 <= argumentIndex);

            return argumentIndex == 1 || argumentIndex == 2;
        }
    }
}

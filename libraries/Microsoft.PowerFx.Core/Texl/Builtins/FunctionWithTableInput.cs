﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // Abstract base class for all Texl builtin functions which take table as the first argument.
    internal abstract class FunctionWithTableInput : BuiltinFunction
    {
        public FunctionWithTableInput(DPath theNamespace, string name, TexlStrings.StringGetter description, FunctionCategories fc, DType returnType, BigInteger maskLambdas, int arityMin, int arityMax, params DType[] paramTypes)
            : base(theNamespace, name, description, fc, returnType, maskLambdas, arityMin, arityMax, paramTypes)
        { }

        public FunctionWithTableInput(string name, TexlStrings.StringGetter description, FunctionCategories fc, DType returnType, BigInteger maskLambdas, int arityMin, int arityMax, params DType[] paramTypes)
            : this(DPath.Root, name, description, fc, returnType, maskLambdas, arityMin, arityMax, paramTypes)
        { }

        public override bool SupportCoercionForArg(int argIndex)
        {
            if (!base.SupportCoercionForArg(argIndex))
                return false;

            // For first arg we don't support coercion.
            return argIndex != 0;
        }

        // This method returns true if there are special suggestions for a particular parameter of the function.
        public override bool HasSuggestionsForParam(int argumentIndex)
        {
            Contracts.Assert(0 <= argumentIndex);

            return argumentIndex == 0;
        }
    }
}


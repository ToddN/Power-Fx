﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // Abstract base class for all 1-arg math functions that return numeric values.
    internal abstract class MathOneArgFunction : BuiltinFunction
    {
        public override bool SupportsParamCoercion => true;
        public override bool IsSelfContained => true;

        public MathOneArgFunction(string name, TexlStrings.StringGetter description, FunctionCategories fc)
            : base(name, description, fc, DType.Number, 0, 1, 1, DType.Number)
        { }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.MathFuncArg1 };
        }
    }

    internal abstract class MathOneArgTableFunction : BuiltinFunction
    {
        public override bool SupportsParamCoercion => true;
        public override bool IsSelfContained => true;

        public MathOneArgTableFunction(string name, TexlStrings.StringGetter description, FunctionCategories fc)
            : base(name, description, fc, DType.EmptyTable, 0, 1, 1, DType.EmptyTable)
        { }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.MathTFuncArg1 };
        }

        public override string GetUniqueTexlRuntimeName(bool isPrefetching = false)
        {
            return GetUniqueTexlRuntimeName(suffix: "_T");
        }

        public override bool CheckInvocation(TexlNode[] args, DType[] argTypes, IErrorContainer errors, out DType returnType, out Dictionary<TexlNode, DType> nodeToCoercedTypeMap)
        {
            Contracts.AssertValue(args);
            Contracts.AssertAllValues(args);
            Contracts.AssertValue(argTypes);
            Contracts.Assert(args.Length == argTypes.Length);
            Contracts.Assert(args.Length == 1);
            Contracts.AssertValue(errors);

            bool fValid = base.CheckInvocation(args, argTypes, errors, out returnType, out nodeToCoercedTypeMap);
            Contracts.Assert(returnType.IsTable);

            // Typecheck the input table
            bool matchedWithCoercion;

            var arg = args[0];
            var argType = argTypes[0];
            fValid &= CheckNumericColumnType(argType, arg, errors, out matchedWithCoercion);

            if (matchedWithCoercion)
            {
                // Now set the coerced type to a table with numeric column type with the same name as in the argument.
                var columns = argType.GetNames(DPath.Root);
                Contracts.Assert(columns.Count() == 1);

                DType coercedColumnType = DType.CreateTable(new TypedName(DType.Number, columns.Single().Name));
                CollectionUtils.Add(ref nodeToCoercedTypeMap, arg, coercedColumnType);
                returnType = coercedColumnType;
            }
            else
            {
                returnType = argType;
            }

            if (!fValid)
                nodeToCoercedTypeMap = null;

            return fValid;
        }
    }
}
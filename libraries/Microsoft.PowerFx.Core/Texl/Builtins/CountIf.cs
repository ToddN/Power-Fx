﻿//------------------------------------------------------------------------------
// <copyright file="CountIf.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.AppMagic.Authoring.Texl
{
    // CountIf(source:*, predicate:b [, predicate:b, ...])
    // Corresponding DAX function: CountAX, CountX
    internal sealed class CountIfFunction : FilterFunctionBase
    {
        public override bool RequiresErrorContext => true;

        public override DelegationCapability FunctionDelegationCapability { get { return DelegationCapability.Filter | DelegationCapability.Count; } }
        public CountIfFunction()
            : base("CountIf", TexlStrings.AboutCountIf, FunctionCategories.Table | FunctionCategories.MathAndStat, DType.Number, -2, 2, int.MaxValue, DType.EmptyTable, DType.Boolean)
        {
            ScopeInfo = new FunctionScopeInfo(this, usesAllFieldsInScope: false);
        }

        public override bool SupportsPaging(CallNode callNode, TexlBinding binding) { return false; }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new [] { TexlStrings.CountIfArg1, TexlStrings.CountIfArg2 };
            yield return new [] { TexlStrings.CountIfArg1, TexlStrings.CountIfArg2, TexlStrings.CountIfArg2 };
            yield return new [] { TexlStrings.CountIfArg1, TexlStrings.CountIfArg2, TexlStrings.CountIfArg2, TexlStrings.CountIfArg2 };
        }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures(int arity)
        {
            if (arity > 2)
                return GetGenericSignatures(arity, TexlStrings.CountIfArg1, TexlStrings.CountIfArg2);
            return base.GetSignatures(arity);
        }

        public override bool CheckInvocation(TexlNode[] args, DType[] argTypes, IErrorContainer errors, out DType returnType)
        {
            Contracts.AssertValue(args);
            Contracts.AssertValue(argTypes);
            Contracts.Assert(args.Length == argTypes.Length);
            Contracts.AssertValue(errors);

            bool fValid = base.CheckInvocation(args, argTypes, errors, out returnType);
            Contracts.Assert(returnType == DType.Number);

            // Ensure that all the args starting at index 1 are booleans.
            for (int i = 1; i < args.Length; i++)
            {
                if (!DType.Boolean.Accepts(argTypes[i]))
                {
                    errors.EnsureError(DocumentErrorSeverity.Severe, args[i], TexlStrings.ErrBooleanExpected);
                    fValid = false;
                }
            }

            return fValid;
        }

        public override bool IsServerDelegatable(CallNode callNode, TexlBinding binding)
        {
            Contracts.AssertValue(callNode);
            Contracts.AssertValue(binding);

            if (!CheckArgsCount(callNode, binding))
                return false;

            IExternalDataSource dataSource = null;
            // We ensure Document is available because some tests run with a null Document.
            if ((binding.Document != null && !binding.Document.Properties.EnabledFeatures.IsEnhancedDelegationEnabled) || !TryGetValidDataSourceForDelegation(callNode, binding, FunctionDelegationCapability, out dataSource))
            {
                if (dataSource != null && dataSource.IsDelegatable)
                    binding.ErrorContainer.EnsureError(DocumentErrorSeverity.Warning, callNode, TexlStrings.OpNotSupportedByServiceSuggestionMessage_OpNotSupportedByService, Name);

                return false;
            }

            TexlNode[] args = callNode.Args.Children.VerifyValue();

            if (args.Length == 0)
                return false;

            // Don't delegate 1-N/N-N counts
            // TASK 9966488: Enable CountRows/CountIf delegation for table relationships
            if (binding.GetType(args[0]).HasExpandInfo)
            {
                SuggestDelegationHint(callNode, binding);
                return false;
            }

            FilterOpMetadata metadata = dataSource.DelegationMetadata.FilterDelegationMetadata;
            // Validate for each predicate node.
            for (int i = 1; i < args.Length; i++)
            {
                if (!IsValidDelegatableFilterPredicateNode(args[i], binding, metadata))
                {
                    SuggestDelegationHint(callNode, binding);
                    return false;
                }
            }

            return true;
        }
    }
}

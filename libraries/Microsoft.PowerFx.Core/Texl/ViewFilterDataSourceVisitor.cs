﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.PowerFx.Core.App;

namespace Microsoft.AppMagic.Authoring.Texl
{
    /// <summary>
    /// This visitor is used to walkthrough the first node of a filter to get the datasource name and
    /// whether or not there is any other filter sub expression that uses a view.
    /// </summary>
    internal sealed class ViewFilterDataSourceVisitor : TexlVisitor
    {
        const string FilterFunctionName = "Filter";
        private TexlBinding _txb;
        public IExternalCdsDataSource cdsDataSourceInfo { get; private set; }
        public bool ContainsViewFilter { get; private set; }

        public ViewFilterDataSourceVisitor(TexlBinding binding)
        {
            Contracts.AssertValue(binding);

            _txb = binding;
        }

        public override void Visit(FirstNameNode node)
        {
            if (_txb.EntityScope.TryGetDataSource(node, out var info) && info.Kind == DataSourceKind.CdsNative)
            {
                cdsDataSourceInfo = info as IExternalCdsDataSource;
            }            
        }

        public override void PostVisit(CallNode node)
        {
            // Check if there is a filter node using view.
            if (node?.Head?.Name.Value == FilterFunctionName)
            {
                foreach (var arg in node.Args.Children)
                {
                    var argType = _txb.GetType(arg);
                    if (argType.Kind == DKind.ViewValue)
                    {
                        ContainsViewFilter = true;
                    }
                }
            }
        }

        public override void PostVisit(DottedNameNode node) { }
        public override void PostVisit(VariadicOpNode node) { }
        public override void PostVisit(RecordNode node) { }
        public override void PostVisit(ListNode node) { }
        public override void PostVisit(BinaryOpNode node) { }
        public override void PostVisit(UnaryOpNode node) { }
        public override void PostVisit(TableNode node) { }
        public override void PostVisit(AsNode node) { }
        public override void Visit(ParentNode node) { }
        public override void Visit(NumLitNode node) { }
        public override void Visit(ReplaceableNode node) { }
        public override void Visit(StrLitNode node) { }
        public override void Visit(BoolLitNode node) { }
        public override void Visit(BlankNode node) { }
        public override void Visit(ErrorNode node) { }
        public override void Visit(SelfNode node) { }
    }
}

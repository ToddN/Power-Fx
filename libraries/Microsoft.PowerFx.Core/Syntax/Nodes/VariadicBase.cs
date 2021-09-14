//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Linq;
using Microsoft.AppMagic.Authoring.Texl.SourceInformation;

namespace Microsoft.AppMagic.Authoring.Texl
{
    /// Base for all variadic nodes.
    internal abstract class VariadicBase : TexlNode
    {
        public readonly TexlNode[] Children;

        // Takes ownership of the array.
        protected VariadicBase(ref int idNext, Token primaryToken, SourceList sourceList, TexlNode[] children)
            : base(ref idNext, primaryToken, sourceList)
        {
            Contracts.AssertValue(children);
            Children = children;

            int maxDepth = 0;

            foreach (TexlNode child in children)
            {
                Contracts.AssertValue(child);
                child.Parent = this;
                if (maxDepth < child.Depth)
                    maxDepth = child.Depth;

                if (MinChildID > child.MinChildID)
                    MinChildID = child.MinChildID;
            }

            _depth = maxDepth + 1;
        }

        public TexlNode[] CloneChildren(ref int idNext, Span ts)
        {
            TexlNode[] clones = new TexlNode[Children.Length];
            for (int x = 0; x < clones.Length; x++)
            {
                clones[x] = Children[x].Clone(ref idNext, ts);
            }
            return clones;
        }

        public static Token[] Clone(Token[] toks, Span ts)
        {
            Contracts.AssertValueOrNull(toks);
            if (toks == null)
                return null;
            Token[] newToks = new Token[toks.Length];
            for (int x = 0; x < toks.Length; x++)
                newToks[x] = toks[x].Clone(ts);
            return newToks;
        }

        public int Count { get { return Children.Length; } }

        public void AcceptChildren(TexlVisitor visitor)
        {
            Contracts.AssertValue(visitor);
            foreach (var child in Children)
            {
                Contracts.AssertValue(child);
                child.Accept(visitor);
            }
        }

        public override Span GetCompleteSpan()
        {
            if (this.Children.Count() == 0)
                return new Span(this.Token.VerifyValue().Span.Min, this.Token.VerifyValue().Span.Lim);

            return new Span(this.Token.VerifyValue().Span.Min, this.Children.VerifyValue().Last().VerifyValue().GetCompleteSpan().Lim);
        }
    }
}
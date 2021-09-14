//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.AppMagic.Transport;

namespace Microsoft.AppMagic.Authoring.Texl
{
    [Flags]
    [TransportType(TransportKind.Enum)]
    public enum FunctionCategories : uint
    {
        None = 0x0,
        Text = 0x1,
        Logical = 0x2,
        Table = 0x4,
        Behavior = 0x8,
        DateTime = 0x10,
        MathAndStat = 0x20,
        Information = 0x40,
        Color = 0x80,
        REST = 0x100,
        Component = 0x200,
    }
}
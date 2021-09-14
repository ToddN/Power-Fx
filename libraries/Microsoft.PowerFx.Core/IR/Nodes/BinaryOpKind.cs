﻿//------------------------------------------------------------------------------
// <copyright file company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerFx.Core.IR
{
    public enum BinaryOpKind
    {
        InText,
        ExactInText,
        InScalarTable,
        ExactInScalarTable,
        InRecordTable,

        AddNumbers,
        AddDateAndTime, // Date + Time
        AddDateAndDay, // Date + Number (Days)
        AddDateTimeAndDay,
        AddTimeAndMilliseconds, // Time + Number (Milliseconds)

        DateDifference,
        TimeDifference,

        MulNumbers,

        DivNumbers,

        EqNumbers,
        EqBoolean,
        EqText,
        EqDate,
        EqTime,
        EqDateTime,
        EqHyperlink,
        EqCurrency,
        EqImage,
        EqColor,
        EqMedia,
        EqBlob,
        EqGuid,
        EqOptionSetValue,
        EqNull,

        NeqNumbers,
        NeqBoolean,
        NeqText,
        NeqDate,
        NeqTime,
        NeqDateTime,
        NeqHyperlink,
        NeqCurrency,
        NeqImage,
        NeqColor,
        NeqMedia,
        NeqBlob,
        NeqGuid,
        NeqOptionSetValue,
        NeqNull,

        LtNumbers,
        LeqNumbers,
        GtNumbers,
        GeqNumbers,

        LtDateTime,
        LeqDateTime,
        GtDateTime,
        GeqDateTime,

        LtDate,
        LeqDate,
        GtDate,
        GeqDate,

        LtTime,
        LeqTime,
        GtTime,
        GeqTime,

        // And, Or, Pow, Concatenate get represented as FunctionNodes with lambdas to handle short-circuiting
    }
}

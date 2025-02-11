﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.PowerFx.Core.UtilityDataStructures;
using Microsoft.PowerFx.Core.Utils;

namespace Microsoft.PowerFx.Core
{
    internal abstract class DisplayNameProvider
    {
        public abstract bool TryGetLogicalName(DName displayName, out DName logicalName);

        public abstract bool TryGetDisplayName(DName logicalName, out DName displayName);
    }
}

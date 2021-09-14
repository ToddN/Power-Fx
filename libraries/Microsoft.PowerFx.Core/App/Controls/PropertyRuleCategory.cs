﻿//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.AppMagic.Transport;

namespace Microsoft.AppMagic.Authoring
{
    /// <summary>
    /// Categories of rules and properties, e.g. data, design, behavior.
    /// When published, this enum is not available to JavaScript;
    /// Please keep these values in sync with src/AppMagic/js/AppMagic.Controls/Constants.ts
    /// </summary>
    [TransportType(TransportKind.Enum)]
    public enum PropertyRuleCategory
    {
        Data = 0,
        Design = 1,
        Behavior = 2,
        ConstantData = 3,
        OnDemandData = 4,
        Scope = 5,
        /// <summary>
        /// Represents a missing property category when deserializing
        /// Should be cleaned up by document converter, only occurs if 
        /// control template is invalid
        /// </summary>
        Unknown = 6,
    }

    /// <summary>
    /// Rule provider types. These are primarily used by the components.
    /// System - Set on all rules on on component definition.
    /// User - Set when any customization on property rules in component instance. 
    /// Unknown - Unknown when provider is not known.
    /// </summary>
    internal enum RuleProviderType
    {
        Unknown,
        System,
        User,
    }

    internal static class PropertyRuleCategoryExtensions
    {
        internal static bool IsValid(this PropertyRuleCategory category) =>
            PropertyRuleCategory.Data <= category && category <= PropertyRuleCategory.Unknown;
        internal static bool IsBehavioral(this PropertyRuleCategory category) => 
            category == PropertyRuleCategory.Behavior || category == PropertyRuleCategory.OnDemandData;
    }

    public static class PropertyRuleCategoryHelper
    {
        public static bool IsValidPropertyRuleCategory(string category)
        {
            PropertyRuleCategory _;
            return Enum.TryParse(category, ignoreCase: true, result: out _);
        }

        public static bool TryParsePropertyCategory(string category, out PropertyRuleCategory categoryEnum)
        {
            Contracts.CheckNonEmpty(category, "category");

            // Enum.TryParse uses a bunch of reflection and boxing. If this becomes an issue, we can
            // use plain-old switch statement.
            return Enum.TryParse(category, ignoreCase: true, result: out categoryEnum);
        }
    }
}

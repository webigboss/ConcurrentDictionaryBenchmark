// <copyright file="Actor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using MessagePack;
    using Microsoft.M365.Substrate.Sdk.SubstrateItem.DataModel.Signals;

    /// <summary>
    /// Signal - Actor
    /// </summary>
    [ExcludeFromCodeCoverage]
    [MessagePackObject]
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct Actor
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// AAD Tenant Id. Optional.
        /// </summary>
        [Key(0)]
        public Guid? AadTenantId { get; set; }

        /// <summary>
        /// Actor Id Type
        /// </summary>
        [Key(1)]
        public ActorIdType ActorIdType { get; set; }

        /// <summary>
        /// Actor Id
        /// </summary>
        [Key(2)]
        public string ActorId { get; set; }

        /// <summary>
        /// "User" or "System"
        /// </summary>
        [Key(3)]
        public ActorType ActorType { get; set; }
    }
}

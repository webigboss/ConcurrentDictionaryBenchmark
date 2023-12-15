// <copyright file="Item.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using MessagePack;

    /// <summary>
    /// The Item a Signal is about
    /// </summary>
    [ExcludeFromCodeCoverage]
    [MessagePackObject]
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct Item
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// Item Id
        /// </summary>
        [Key(0)]
        public string ItemId { get; set; }

        /// <summary>
        /// Item Type
        /// </summary>
        [Key(1)]
        public string ItemType { get; set; }

        /// <summary>
        /// Item Container
        /// </summary>
        [Key(2)]
        public string ContainerId { get; set; }

        /// <summary>
        /// Item Container Type
        /// </summary>
        [Key(3)]
        public string ContainerType { get; set; }
    }
}

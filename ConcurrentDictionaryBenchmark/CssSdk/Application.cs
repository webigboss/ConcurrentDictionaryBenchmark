// <copyright file="Application.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using MessagePack;

    /// <summary>
    /// Struct that defines Signal Application
    /// </summary>
    [ExcludeFromCodeCoverage]
    [MessagePackObject]
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct Application
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// Application Id
        /// </summary>
        [Key(0)]
        public Guid AadAppId { get; set; }

        /// <summary>
        /// Application Name
        /// </summary>
        [Key(1)]
        public string AppName { get; set; }

        /// <summary>
        /// Application Version
        /// </summary>
        [Key(2)]
        public string AppVer { get; set; }

        /// <summary>
        /// Application Workload
        /// </summary>
        [Key(3)]
        public string Workload { get; set; }
    }
}

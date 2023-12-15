// <copyright file="Device.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals
{
    using System.Diagnostics.CodeAnalysis;
    using MessagePack;

    /// <summary>
    /// Device Type for a signal
    /// </summary>
    [ExcludeFromCodeCoverage]
    [MessagePackObject]
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct Device
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// Device Id
        /// </summary>
        [Key(0)]
        public string DeviceId { get; set; }

        /// <summary>
        /// Device User Agent
        /// </summary>
        [Key(1)]
        public string UserAgent { get; set; }

        /// <summary>
        /// Device Client IP
        /// </summary>
        [Key(2)]
        public string ClientIp { get; set; }

        /// <summary>
        /// Operating System
        /// </summary>
        [Key(3)]
        public string Os { get; set; }

        /// <summary>
        /// Operating System version.
        /// </summary>
        [Key(4)]
        public string OsVer { get; set; }
    }
}

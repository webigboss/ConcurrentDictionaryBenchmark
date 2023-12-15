// <copyright file="CssSdkRequestContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;
    using Microsoft.Substrate.CompactSignalStore.Sdk.Contract;

    /// <summary>
    /// implementation of <see cref="ICssSdkRequestContext"/>
    /// </summary>
    public class CssSdkRequestContext : ICssSdkRequestContext
    {
        /// <inheritdoc/>
        public ShardKey ShardKey { get; init; }

        /// <inheritdoc/>
        public string Hostname { get; init; }

        /// <inheritdoc/>
        public string Token { get; init; }

        /// <inheritdoc/>
        public HttpClient HttpClient { get; init; }

        /// <inheritdoc/>
        public ILogger Logger { get; init; }

        /// <inheritdoc/>
        public string DeploymentRing { get; init; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, string> AdditionalHeaders { get; init; }

        /// <inheritdoc/>
        public IReadOnlyCollection<SignalType> SignalTypesToSigs { get; init; }

        /// <inheritdoc/>
        public void Validate() 
        {
#if DEBUG
            if (this.ShardKey == default)
            {
                throw new ArgumentException($"{nameof(ShardKey)} cannot be default value");
            }
#endif

            DebugValidator.ThrowIfNull(nameof(Hostname), Hostname);
            DebugValidator.ThrowIfNull(nameof(Token), Token);
            DebugValidator.ThrowIfNull(nameof(HttpClient), HttpClient);
            DebugValidator.ThrowIfNull(nameof(Logger), Logger);
            DebugValidator.ThrowIfNull(nameof(DeploymentRing), DeploymentRing);
        }
    }
}

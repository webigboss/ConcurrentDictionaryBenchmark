// <copyright file="ICssSdkRequestContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Contract
{
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;

    /// <summary>
    /// Css Sdk Request Context
    /// </summary>
    public interface ICssSdkRequestContext
    {
        /// <summary>
        /// Gets the Shard Key
        /// </summary>
        public ShardKey ShardKey { get; }

        /// <summary>
        /// Gets the Substrate Url Context
        /// </summary>
        public string Hostname { get; }

        /// <summary>
        /// Gets the Deployment Ring
        /// </summary>
        public string DeploymentRing { get; }

        /// <summary>
        /// Gets the Authorization Token
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// An HTTP client which can send requests. it should be passed by consumer of css client library as partner will have full control of life cycle of http client.
        /// The suggested guideline for using HttpClient : https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines?source=recommendations 
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// An logger which will log client events, exceptions, errors etc.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Optional Additional headers for the HttpRequestMessage
        /// </summary>
        public IReadOnlyDictionary<string, string> AdditionalHeaders { get; }

        /// <summary>
        /// Optional: A collection of signal types to route to Sigs
        /// </summary>
        public IReadOnlyCollection<SignalType> SignalTypesToSigs { get; }

        /// <summary>
        /// Validate the request context
        /// </summary>
        public void Validate();
    }
}

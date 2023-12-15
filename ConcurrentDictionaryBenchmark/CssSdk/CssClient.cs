// <copyright file="CssClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Implementation
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;
    using Microsoft.Substrate.CompactSignalStore.Item.DataModels;
    using Microsoft.Substrate.CompactSignalStore.Sdk.Contract;

    /// <summary>
    /// Css client
    /// </summary>
    internal class CssClient : ISignalClient
    {
        /// <summary>
        /// Css client config
        /// </summary>
        private readonly ICssSdkClientConfig clientConfig;

        /// <summary>
        /// Css client constructor
        /// </summary>
        /// <param name="clientConfig"></param>
        internal CssClient (ICssSdkClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
        }

        /// <inheritdoc/>
        public ValueTask<ISignal> GetById(ICssSdkRequestContext requestContext, string id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ISignal> QueryItems(
            ICssSdkRequestContext requestContext, 
            string filter, 
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var url = BuildEndpointUrl(requestContext, filter);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            AddHeaders(requestContext, requestMessage);

            cancellationToken.ThrowIfCancellationRequested();
            using var response = await requestContext.HttpClient
                .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            await SdkHelper.EnsureSuccessStatusCodeAndThrowHeaderAndContent(response, cancellationToken);

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await foreach (var signal in JsonSerializer.DeserializeAsyncEnumerable<Signal>(utf8Json: stream, cancellationToken: cancellationToken))
            {
                if (signal != null)
                {
                    yield return signal;
                }
            }
        }

        private string BuildEndpointUrl(ICssSdkRequestContext requestContext, string filter)
        {
            return $"https://{requestContext.Hostname}/userknowledgebase/v0.1/users('{requestContext.GetRouteKey()}')/css/signals?$filter={filter}";
        }

        private void AddHeaders(ICssSdkRequestContext requestContext, HttpRequestMessage requestMessage)
        {
            SdkHelper.AddCommonHeaders(requestContext, requestMessage);
            requestMessage.Headers.Add(SdkConstants.RoutingSessionHeaderKey, requestContext.ShardKey.MailboxGuid.ToString());
            requestMessage.Headers.Add(SdkConstants.CallingServiceAppIdHeaderKey, this.clientConfig.ApplicationId.ToString());
        }
    }
}

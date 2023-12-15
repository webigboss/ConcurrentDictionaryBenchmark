// <copyright file="SigsClient.cs" company="Microsoft">
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
    internal class SigsClient : ISignalClient
    {
        private readonly ICssSdkClientConfig clientConfig;

        private readonly int top;

        private readonly string orderby;

        /// <summary>
        /// Css client constructor
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <param name="top"></param>
        /// <param name="orderby"></param>
        internal SigsClient(ICssSdkClientConfig clientConfig, int top, string orderby)
        {
            this.clientConfig = clientConfig;
            this.top = top;
            this.orderby = orderby;
        }

        /// <inheritdoc/>
        public async ValueTask<ISignal> GetById(ICssSdkRequestContext requestContext, string id, CancellationToken cancellationToken)
        {
            var url = BuildGetByIdUrl(requestContext, id);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            AddHeaders(requestContext, requestMessage);

            using var response = await requestContext.HttpClient
                .SendAsync(requestMessage, cancellationToken)
                .ConfigureAwait(false);

            await SdkHelper.EnsureSuccessStatusCodeAndThrowHeaderAndContent(response, cancellationToken);

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<Signal>(utf8Json: stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ISignal> QueryItems(
            ICssSdkRequestContext requestContext, 
            string filter, 
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var url = BuildQueryUrl(requestContext, filter, this.top, this.orderby);

            do
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                AddHeaders(requestContext, requestMessage);

                cancellationToken.ThrowIfCancellationRequested();
                using var response = await requestContext.HttpClient
                    .SendAsync(requestMessage, cancellationToken)
                    .ConfigureAwait(false);

                await SdkHelper.EnsureSuccessStatusCodeAndThrowHeaderAndContent(response, cancellationToken);

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                var sigsResponse = await JsonSerializer.DeserializeAsync<SigsQueryResponse>(utf8Json: stream, cancellationToken: cancellationToken).ConfigureAwait(false);

                if (sigsResponse?.Value != null)
                {
                    foreach (var signal in sigsResponse.Value)
                    {
                        yield return signal;
                    }
                }

                url = sigsResponse?.NextLink;
            }
            while (!string.IsNullOrWhiteSpace(url));
        }

        private static string BuildBaseUrl(ICssSdkRequestContext requestContext)
        {
            return $"https://{requestContext.Hostname}/api/v2.0/ItemContainers('{requestContext.GetRouteKey()}')";
        }

        private static string BuildQueryUrl(ICssSdkRequestContext requestContext, string filter, int top, string orderby)
        {
            return $"{BuildBaseUrl(requestContext)}/signals?$filter={filter}&$top={top}&$orderby={orderby}";
        }

        private static string BuildGetByIdUrl(ICssSdkRequestContext requestContext, string id)
        {
            return $"{BuildBaseUrl(requestContext)}/signals('{id}')";
        }

        private void AddHeaders(ICssSdkRequestContext requestContext, HttpRequestMessage requestMessage)
        {
            SdkHelper.AddCommonHeaders(requestContext, requestMessage);
            requestMessage.Headers.Add(SdkConstants.PreferHeaderKey, SdkConstants.PreferHeaderValue);
        }
    }
}

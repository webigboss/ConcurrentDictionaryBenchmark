// <copyright file="SignalClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Implementation
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;
    using Microsoft.Substrate.CompactSignalStore.Item.DataModels;
    using Microsoft.Substrate.CompactSignalStore.Sdk.Contract;

    /// <summary>
    /// Css SDK client exposes to the consumers, has the routing logic to validate the filter 
    /// and route the request to CSS if it is supported by CSS, otherwise route to the SIGS.
    /// </summary>
    public class SignalClient : ISignalClient
    {
        private const int DefaultSigsClientTop = 100;

        private const string DefaultSigsOrderBy = "SignalType,EndTime,StartTime";

        /// <summary>
        /// css client
        /// </summary>
        private readonly ISignalClient cssClient;

        /// <summary>
        /// sigs client
        /// </summary>
        private readonly ISignalClient sigsClient;

        /// <summary>
        /// request router
        /// </summary>
        private readonly ICssSdkRequestRouter requestRouter;

        /// <summary>
        /// constructor of CssSdkClient with default router implementation.
        /// </summary>
        /// <param name="clientConfig"></param>
        public SignalClient(ICssSdkClientConfig clientConfig) : this(clientConfig, new SignalClientRequestRouter())
        {
        }

        /// <summary>
        /// constructor of CssSdkClient with custom router implementation.
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <param name="requestRouter"></param>
        public SignalClient(ICssSdkClientConfig clientConfig, ICssSdkRequestRouter requestRouter)
        {
            clientConfig.Validate();
            this.cssClient = new CssClient(clientConfig);
            this.sigsClient = new SigsClient(clientConfig, DefaultSigsClientTop, DefaultSigsOrderBy);
            this.requestRouter = requestRouter;
        }

        /// <inheritdoc/>
        public ValueTask<ISignal> GetById(ICssSdkRequestContext requestContext, string id, CancellationToken cancellationToken)
        {
            requestContext.Validate();
            return this.sigsClient.GetById(requestContext, id, cancellationToken);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ISignal> QueryItems(ICssSdkRequestContext requestContext, string filter, CancellationToken cancellationToken)
        {
            requestContext.Validate();
            var routingResult = this.requestRouter.Route(filter, requestContext);
            requestContext.Logger.LogInformation(routingResult.ToString());
            
            return routingResult.To switch
            {
                ClientType.Css => this.cssClient.QueryItems(requestContext, filter, cancellationToken),
                ClientType.Sigs => this.sigsClient.QueryItems(requestContext, filter, cancellationToken),
                ClientType.Css | ClientType.Sigs => this.QueryItemsFromCssAndSigs(requestContext, routingResult, cancellationToken),
                _ => throw new System.NotImplementedException(),
            };
        }

        /// <summary>
        /// Make calls respectively to CSS and SIGS and merge the results.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="routingResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async IAsyncEnumerable<ISignal> QueryItemsFromCssAndSigs(ICssSdkRequestContext requestContext, RoutingResult routingResult, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var signal in this.cssClient.QueryItems(requestContext, routingResult.SplitFilters[ClientType.Css], cancellationToken))
            {
                yield return signal;
            }

            await foreach (var signal in this.sigsClient.QueryItems(requestContext, routingResult.SplitFilters[ClientType.Sigs], cancellationToken))
            {
                yield return signal;
            }
        }
    }
}

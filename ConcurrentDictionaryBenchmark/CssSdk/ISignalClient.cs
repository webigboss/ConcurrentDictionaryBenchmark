// <copyright file="ISignalClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;

    /// <summary>
    /// CssSdkClient interface
    /// </summary>
    public interface ISignalClient
    {
        /// <summary>
        /// Query the item from the store as an async enumerable stream of <see cref="ISignal"/>
        /// </summary>
        /// <param name="requestContext">the query filter</param>
        /// <param name="filter">the query filter</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>An async enumerable stream of <see cref="ISignal"/></returns>
        /// <exception cref="OperationCanceledException">
        /// The cancellation token signaled a cancel. OR on .NET Core when the request has timed out.
        /// </exception>
        IAsyncEnumerable<ISignal> QueryItems(ICssSdkRequestContext requestContext, string filter, CancellationToken cancellationToken);

        /// <summary>
        /// Get signal by id
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<ISignal> GetById(ICssSdkRequestContext requestContext, string id, CancellationToken cancellationToken);
    }
}

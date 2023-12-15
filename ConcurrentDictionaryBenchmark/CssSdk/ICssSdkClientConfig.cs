// <copyright file="ICssSdkClientConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Contract
{
    using System;

    /// <summary>
    /// Interface of CSS SDK request router
    /// </summary>
    public interface ICssSdkClientConfig
    {
        /// <summary>
        /// Your application ID.
        /// </summary>
        Guid ApplicationId { get; }

        /// <summary>
        /// Validate the request context
        /// </summary>
        internal void Validate();
    }
}

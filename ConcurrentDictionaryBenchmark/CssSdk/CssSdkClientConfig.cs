// <copyright file="CssSdkClientConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Implementation
{
    using System;
    using Microsoft.Substrate.CompactSignalStore.Sdk.Contract;

    /// <summary>
    /// CSS SDK client config
    /// </summary>
    public class CssSdkClientConfig : ICssSdkClientConfig
    {
        /// <inheritdoc/>
        public Guid ApplicationId { get; init; }

        /// <inheritdoc/>
        void ICssSdkClientConfig.Validate()
        {
#if DEBUG
            if (this.ApplicationId == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(ApplicationId)} is empty");
            }
#endif
        }
    }
}

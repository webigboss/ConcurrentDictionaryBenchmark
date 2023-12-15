// <copyright file="SignalComparer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.DataModels
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Equality comparer for signals
    /// </summary>
    public class SignalComparer : IEqualityComparer<Signal>
    {
        /// <inheritdoc/>
        public bool Equals(Signal x, Signal y) => x.Id.Equals(y.Id, StringComparison.Ordinal);

        /// <inheritdoc/>
        public int GetHashCode(Signal signal) => signal.Id.GetHashCode(StringComparison.Ordinal);
    }
}

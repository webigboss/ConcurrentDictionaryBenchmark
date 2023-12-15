// <copyright file="SignalExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.DataModels
{
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;

    /// <summary>
    /// Extension methods for Signal
    /// </summary>
    public static class SignalExtensions
    {
        /// <summary>
        /// Gets the Id of the Signal
        /// </summary>
        /// <param name="cssItem"></param>
        /// <returns></returns>
        public static string GetSignalId(this ICssItem cssItem) =>
            (cssItem is Signal && cssItem.SignalType == SignalType.InstantMessage) ?
            ((Signal)cssItem).Item?.ItemId ?? cssItem.Id : cssItem.Id;
    }
}

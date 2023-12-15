// <copyright file="CssUser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts.Store.DsApi
{
    using System;
    using System.Text.Json.Serialization;
    using Microsoft.M365.Substrate.Sdk.ProcessorContracts.Contracts.Exchange;

    /// <summary>
    /// Represnets a user entity resolved through DsApi
    /// </summary>
    public record struct CssUser
    {
        /// <summary>
        /// Smtp address
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string SmtpAddress { get; init; }

        /// <summary>
        /// The tenant id
        /// </summary>
        [JsonPropertyName("TId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid TenantId { get; init; }

        /// <summary>
        /// The external directory object id
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid OId { get; init; }

        /// <summary>
        /// The recipient type details for a given user, ex: UserMailbox, GroupMailbox, etc 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public RecipientTypeDetails? RecipientTypeDetails { get; init; }

        /// <summary>
        /// A unix timestamp in seconds, as a long value, indicating when the user was resolved by the system
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long IdentityResolutionTimeStamp { get; init; }

        /// <summary>
        /// Indicates wheter the user is external or not
        /// </summary>
        public bool IdentityResolutionSuccess { get; init; }
    }
}

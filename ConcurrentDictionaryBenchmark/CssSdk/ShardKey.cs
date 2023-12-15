// <copyright file="ShardKey.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.Contracts
{
    using System;
    using System.Text.Json.Serialization;
    using MessagePack;
    using Microsoft.M365.DataPrivacy.DataTagging;

    /// <summary>
    /// The combination of values that uniquely identify a Shard.
    /// This value will be used as a Key and will also be applied to Data as a Tag.
    /// The Tag will help us ensure privacy and prevent any data intermingling issues.
    /// </summary>
    [MessagePackObject]
    public readonly struct ShardKey : IShardTag, IEquatable<ShardKey>
    {
        /// <summary>
        /// External Directory Object Id
        /// </summary>
        [Key(0)]
        [JsonPropertyName("OId")]
        public Guid ObjectId { get; init; }

        /// <summary>
        /// Mailbox Guid
        /// </summary>
        [Key(1)]
        public Guid MailboxGuid { get; init; }

        /// <summary>
        /// Tenant Id
        /// </summary>
        [Key(2)]
        [JsonPropertyName("TId")]
        public Guid TenantId { get; init; }

        /// <inheritdoc />
        public bool Equals(ShardKey shardKey) =>
            (ObjectId, MailboxGuid, TenantId) == (shardKey.ObjectId, shardKey.MailboxGuid, shardKey.TenantId);

        /// <inheritdoc />
        public override bool Equals(object obj) => 
            obj is ShardKey shardKey && Equals(shardKey);

        /// <inheritdoc />
        public static bool operator ==(ShardKey left, ShardKey right) => Equals(left, right);

        /// <inheritdoc />
        public static bool operator !=(ShardKey left, ShardKey right) => !Equals(left, right);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(ObjectId, MailboxGuid, TenantId);

        /// <inheritdoc />
        public override string ToString() => $"OId:{ObjectId}, MailboxGuid:{MailboxGuid}, TenantId:{TenantId}";
    }
}

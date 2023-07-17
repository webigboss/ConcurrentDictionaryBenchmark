using System.Text;
using System.Text.Json.Serialization;

namespace ConcurrentDictionaryBenchmark
{
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

    [Flags]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RecipientTypeDetails : long
    {
        None = 0x0L,
        UserMailbox = 0x1L,
        LinkedMailbox = 0x2L,
        SharedMailbox = 0x4L,
        LegacyMailbox = 0x8L,
        RoomMailbox = 0x10L,
        EquipmentMailbox = 0x20L,
        MailContact = 0x40L,
        MailUser = 0x80L,
        MailUniversalDistributionGroup = 0x100L,
        MailNonUniversalGroup = 0x200L,
        MailUniversalSecurityGroup = 0x400L,
        DynamicDistributionGroup = 0x800L,
        PublicFolder = 0x1000L,
        SystemAttendantMailbox = 0x2000L,
        SystemMailbox = 0x4000L,
        MailForestContact = 0x8000L,
        User = 0x10000L,
        Contact = 0x20000L,
        UniversalDistributionGroup = 0x40000L,
        UniversalSecurityGroup = 0x80000L,
        NonUniversalGroup = 0x100000L,
        DisabledUser = 0x200000L,
        MicrosoftExchange = 0x400000L,
        ArbitrationMailbox = 0x800000L,
        MailboxPlan = 0x1000000L,
        LinkedUser = 0x2000000L,
        RoomList = 0x10000000L,
        DiscoveryMailbox = 0x20000000L,
        RoleGroup = 0x40000000L,
        RemoteUserMailbox = 0x80000000L,
        Computer = 0x100000000L,
        RemoteRoomMailbox = 0x200000000L,
        RemoteEquipmentMailbox = 0x400000000L,
        RemoteSharedMailbox = 0x800000000L,
        PublicFolderMailbox = 0x1000000000L,
        TeamMailbox = 0x2000000000L,
        RemoteTeamMailbox = 0x4000000000L,
        MonitoringMailbox = 0x8000000000L,
        GroupMailbox = 0x10000000000L,
        LinkedRoomMailbox = 0x20000000000L,
        AuditLogMailbox = 0x40000000000L,
        RemoteGroupMailbox = 0x80000000000L,
        SchedulingMailbox = 0x100000000000L,
        GuestMailUser = 0x200000000000L,
        AuxAuditLogMailbox = 0x400000000000L,
        SupervisoryReviewPolicyMailbox = 0x800000000000L,
        ExchangeSecurityGroup = 0x1000000000000L,
        SubstrateGroup = 0x2000000000000L,
        SubstrateADGroup = 0x4000000000000L,
        WorkspaceMailbox = 0x8000000000000L,
        SharedWithMailUser = 0x10000000000000L,
        ServicePrinciple = 0x20000000000000L,
        BlobShard = 0x40000000000000L,
        AllUniqueRecipientTypes = 0x7FFFFFFFFFFFFFL
    }
}

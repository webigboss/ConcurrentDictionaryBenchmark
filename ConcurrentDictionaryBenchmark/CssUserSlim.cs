using System.Text;
using System.Text.Json.Serialization;

namespace ConcurrentDictionaryBenchmark
{
    public record struct CssUserSlim
    {
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

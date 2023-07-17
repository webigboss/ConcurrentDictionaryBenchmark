namespace ConcurrentDictionaryBenchmark
{
    public record struct UserCacheKey : IEquatable<UserCacheKey>
    {
        public Guid TenantId { get; set; }
        
        public string SmtpAddress { get; set; }

        public bool Equals(UserCacheKey other)
        => TenantId.Equals(other.TenantId) && SmtpAddress.Equals(other.SmtpAddress, StringComparison.OrdinalIgnoreCase);
    }
}

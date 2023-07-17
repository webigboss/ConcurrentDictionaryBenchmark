namespace ConcurrentDictionaryBenchmark
{
    internal class Utils
    {
        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateRandomSmtpAddress()
        {
            return $"{RandomString(10)}@{RandomString(10)}.com";
        }

        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

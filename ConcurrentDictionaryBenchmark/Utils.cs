using Microsoft.Diagnostics.Runtime;

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


        /// <summary>
        /// Get a random normal index given a length and percentage of indexes within 1 standard deviation (68%).
        /// 
        /// +-1 standardDeviation ~= 68%
        /// +-2 standardDeviation ~= 95%
        /// +-3 standardDeviation ~= 99.7%
        /// </summary>
        /// <param name="length"></param>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        public static int GetNormalIndex(int length, double percent)
        {
            int index = -1;
            var standardDeviation = length * percent / 2;
            while (index < 0 || index >= length)
            {
                index = (int)SimpleRNG.GetNormal(length / 2, standardDeviation);
            }

            return index;
        }
    }
}

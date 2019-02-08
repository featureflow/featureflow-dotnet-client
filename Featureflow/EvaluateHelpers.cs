using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Featureflow.Client
{
    internal class EvaluateHelpers
    {
        private const string ANONYMOUS = "anonymous";

        internal string GetHash(string contextKey, string featureKey, string salt)
        {
            var sha = SHA1.Create();
            string str = (salt + ":" + featureKey + ":" + contextKey).Substring(0, 15);
            byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
            return data.ToString();
        }

        internal long GetVariantValue(string hash)
        {
            long longVal = long.Parse(hash, NumberStyles.HexNumber);
            return (longVal % 100) + 1;
        }
    }
}
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Featureflow.Client

{
    public class EvaluateHelpers
    {
        private readonly string ANONYMOUS = "anonymous";
      /*  public bool matchRule(Rule rule, FeatureflowUser user){
            
            return audience==null || audience.matches(user);
        }*/

      /*  public string getVariantSplitKey(string contextKey, string featureKey, string salt){
            if(contextKey==null)contextKey= ANONYMOUS;
            long variantValue = getVariantValue(getHash(contextKey, featureKey, salt));
            return getSplitKey(variantValue);
        }*/

      /*  public string getSplitKey(long variantValue){
            long percent = 0;
            foreach (VariantSplit variantSplit in variantSplits) {
                percent += variantSplit.getSplit();
                if(percent >= variantValue)return variantSplit.getVariantKey();
            }
            return null;
        }*/
        
        public string getHash(string contextKey, string featureKey, string salt){
            var sha = SHA1.Create();
            string str = (salt + ":" + featureKey + ":" + contextKey).Substring(0, 15);
            byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
            return data.ToString();
        }

        public long getVariantValue(string hash) {
            long longVal = long.Parse(hash, NumberStyles.HexNumber);
            return (longVal % 100) + 1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Featureflow.Client
{
    public class Evaluate
    {
        private const string Salt = "1";

        private readonly string evaluateResult;

        public Evaluate(FeatureControl feature, User user, string failoverVariant)
        {
            evaluateResult = feature == null ? failoverVariant : CalculateVariant(feature, user);
        }

        public bool IsOn()
        {
            return Is(Variant.On);
        }

        public bool IsOff()
        {
            return Is(Variant.Off);
        }

        public bool Is(string variant)
        {
            if (evaluateResult == null)
            {
                return variant == null;
            }

            return evaluateResult.Equals(variant);
        }

        public string Value()
        {
            return evaluateResult;
        }

        private string CalculateVariant(FeatureControl featureControl, User user)
        {
            if (!featureControl.Enabled)
            {
                return featureControl.OffVariantKey;
            }

            AddAdditionalAttributes(user);

            foreach (var rule in featureControl.Rules)
            {
                // if rule matches then return the variant
                var userBucketKey = user.BucketKey ?? user.Id;

                // if the audience is null then it is the default rule (always last in the list) - all will match
                if (rule.Audience == null || AudienceMatches(user, rule.Audience))
                {
                    return GetVariant(userBucketKey, featureControl.Key, Salt, rule.VariantSplits);
                }
            }

            // at least one of hte rules should have matched above as the default rule has no matchers, if not return null to invoke the default rule
            return null;
        }

        private bool AudienceMatches(User user, Audience audience)
        {
            if (audience.Conditions == null || audience.Conditions.Count == 0)
            {
                return true;
            }

            foreach (var condition in audience.Conditions)
            {
                if (condition.Matches(user))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetVariant(string userBucketKey, string featureControlKey, string salt, List<VariantSplit> variantSplits)
        {
            var variantBucket = GetBucket(userBucketKey, featureControlKey, salt);
            var percent = 0f;
            foreach (var split in variantSplits)
            {
                percent += split.Split;
                if (variantBucket < percent)
                {
                    return split.VariantKey;
                }
            }

            return null;
        }

        private float GetBucket(string userBucketKey, string featureControlKey, string salt)
        {
            var hash = SHA1HashStringForUTF8String(string.Format("{0}:{1}:{2}", salt, featureControlKey, userBucketKey)).Substring(0, 15);
            var longValue = long.Parse(hash, NumberStyles.HexNumber);
            return (longValue % 100) + 1;
        }

        private static void AddAdditionalAttributes(User user)
        {
            user.SessionAttributes["featureflow.user.id"] = new List<object> { user.Id };
            user.SessionAttributes["featureflow.hourofday"] = new List<object> { DateTime.Now.Hour };
            user.SessionAttributes["featureflow.date"] = new List<object> { DateTime.Now };
        }

        private string SHA1HashStringForUTF8String(string str)
        {
            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(str)); 
            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    public class Condition
    {
        /// <summary>
        /// name, age, date
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }

        /// <summary>
        /// operator value
        /// </summary>
        [JsonProperty("operator")]
        public string Operator { get; set; }

        /// <summary>
        /// some value 1,2,dave,timestamp,2016-01-11-10:10:10:0000UTC
        /// </summary>
        [JsonProperty("values")]
        public List<object> Values { get; set; } = new List<object>();

        public bool Matches(User user)
        {
            // evaluate this condition against the given user. If ther are no values in the user to match then this is false as it is trying to match at least something
            if (user == null || (user.Attributes == null && user.SessionAttributes == null))
            {
                return false;
            }

            // combine session and user attributes for evaluation
            var combined = new Dictionary<string, List<object>>();
            if (user.SessionAttributes != null)
            {
                user.SessionAttributes.ToList().ForEach(x => combined[x.Key] = x.Value);
            }

            if (user.Attributes != null)
            {
                user.Attributes.ToList().ForEach(x => combined[x.Key] = x.Value);
            }

            // for each of the user attributes
            foreach (var key in combined.Keys)
            {
                if (key.Equals(Target))
                {
                    if (combined.TryGetValue(key, out List<object> attributeValues))
                    {
                        foreach (var attributeValue in attributeValues)
                        {
                            if (Evaluate(attributeValue, Operator, Values))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool Evaluate(object userAttributeValue, string op, List<object> conditionValues)
        {
            switch (op)
            {
                case "equals":
                    return userAttributeValue.Equals(conditionValues[0]);

                case "lessThan":
                    return Convert.ToInt64(userAttributeValue) < (long)conditionValues[0];

                case "greaterThan":
                    return Convert.ToInt64(userAttributeValue) > (long)conditionValues[0];

                case "lessThanOrEqual":
                    return Convert.ToInt64(userAttributeValue) <= (long)conditionValues[0];

                case "greaterThanOrEqual":
                    return Convert.ToInt64(userAttributeValue) >= (long)conditionValues[0];

                case "startsWith":
                    return userAttributeValue.ToString().StartsWith((string)conditionValues[0]);

                case "endsWith":
                    return userAttributeValue.ToString().EndsWith((string)conditionValues[0]);

                case "matches":
                    return Regex.IsMatch(userAttributeValue.ToString(), (string)conditionValues[0]);

                case "in":
                    return conditionValues.Exists(val => val.ToString().Equals(userAttributeValue.ToString()));

                case "notIn":
                    return !conditionValues.Exists(val => val.ToString().Equals(userAttributeValue.ToString()));

                case "contains":
                    return conditionValues[0].ToString().Contains(userAttributeValue.ToString());

                case "before":
                    return Convert.ToDateTime(userAttributeValue) < Convert.ToDateTime(conditionValues[0]);

                case "after":
                    return Convert.ToDateTime(userAttributeValue) > Convert.ToDateTime(conditionValues[0]);

                default:
                    return false;
            }
        }
    }
}
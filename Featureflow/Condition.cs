using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


namespace Featureflow.Client
{
    public class Condition
    {
        

        [JsonProperty("target")]
        public string Target  { get; set; }    //name, age, date
        [JsonProperty("operator")]
        public string Operator { get; set; } // = < > like in out
        [JsonProperty("values")]
        public List<object> Values  { get; set; } = new List<object>(); //some value 1,2,dave,timestamp,2016-01-11-10:10:10:0000UTC   

        public bool matches(User user)
        {
            //evaluate this condition against the given user. If ther are no values in the user to match then this is false as it is trying to match at least something
            if (user == null || user.Attributes == null && user.SessionAttributes == null) return false;
            //combine session and user attributes for evaluation
            var combined = new Dictionary<string, List<object>>();
            if (user.SessionAttributes != null)
            {
                user.SessionAttributes.ToList().ForEach(x => combined[x.Key] = x.Value);
            }
            
            if (user.Attributes != null)
            {
                user.Attributes.ToList().ForEach(x => combined[x.Key] = x.Value);    
            }
            
            //for each of the user attributes
            foreach (var key in combined.Keys)
            {
                if (key.Equals(Target))
                {
                    List<object> attributeValues;
                    combined.TryGetValue(key, out attributeValues);
                    foreach (var attributeValue in attributeValues)
                    {
                        if (Evaluate(attributeValue, Operator, Values))
                        {
                            return true;
                        }
                    }                    
                }
            }

            return false;
        }

        public static bool Evaluate(object userAttributeValue, string op, List<object> conditionValues)
        {
            long longVal;
            switch (op)
            {
                case "equals":
                    if (userAttributeValue.Equals(conditionValues[0])) return true;                    
                    return false;
                case "lessThan":
                    longVal = Convert.ToInt64(userAttributeValue);
                    if (longVal < (long) conditionValues[0]) return true;
                    return false;
                case "greaterThan":
                    longVal = Convert.ToInt64(userAttributeValue);
                    if (longVal > (long) conditionValues[0]) return true;
                    return false;
                case "lessThanOrEqual":
                    longVal = Convert.ToInt64(userAttributeValue);
                    if (longVal <= (long) conditionValues[0]) return true;
                    return false;                
                case "greaterThanOrEqual":
                    longVal = Convert.ToInt64(userAttributeValue);
                    if (longVal >= (long) conditionValues[0]) return true;
                    return false;
                case "startsWith":
                    if (userAttributeValue.ToString().StartsWith((string) conditionValues[0])) return true;                    
                    return false;
                case "endsWith":
                    if (userAttributeValue.ToString().EndsWith((string) conditionValues[0])) return true;                    
                    return false;
                case "matches":
                    if  (Regex.IsMatch(userAttributeValue.ToString(), (string) conditionValues[0])) return true;                    
                    return false;
                case "in":
                    if (conditionValues.Exists(val => val.ToString().Equals(userAttributeValue.ToString())))
                    {
                        return true;
                    }
                    return false;                        
                case "notIn":
                    if (conditionValues.Exists(val => val.ToString().Equals(userAttributeValue.ToString())))
                    {
                        return false;
                    }
                    return true;               
                case "contains":
                    if (conditionValues[0].ToString().Contains(userAttributeValue.ToString())) return true;                    
                    return false;
                case "before":
                    if(Convert.ToDateTime(userAttributeValue) < Convert.ToDateTime(conditionValues[0]))
                    //if (ConvertToDateTime(userAttributeValue) < ConvertToDateTime(conditionValues[0]))
                    {
                        return true;
                    }

                    return false;
                case "after":
                    if(Convert.ToDateTime(userAttributeValue) > Convert.ToDateTime(conditionValues[0]))
                    //if (ConvertToDateTime(userAttributeValue) > ConvertToDateTime(conditionValues[0]))
                    {
                        return true;
                    }

                    return false;
                default:
                    return false; 
            }            
        }        
    }
    

}
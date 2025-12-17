using UnityEngine;
using System.Collections.Generic;

public static class JsonHelper
{
    /// <summary>
    /// Simple JSON parser for WebSocket messages
    /// </summary>
    public static Dictionary<string, object> ParseJson(string json)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        
        // Remove surrounding braces and trim
        json = json.Trim();
        if (json.StartsWith("{") && json.EndsWith("}"))
        {
            json = json.Substring(1, json.Length - 2);
        }
        
        // Split by commas
        string[] pairs = json.Split(',');
        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split(':');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim().Trim('"');
                string value = keyValue[1].Trim().Trim('"');
                result[key] = value;
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Simple JSON serializer for WebSocket messages
    /// </summary>
    public static string SerializeToJson(Dictionary<string, object> data)
    {
        List<string> pairs = new List<string>();
        
        foreach (var kvp in data)
        {
            string valueStr = kvp.Value.ToString();
            // If the value looks like a string, wrap it in quotes
            if (!IsNumeric(valueStr) && valueStr != "true" && valueStr != "false")
            {
                valueStr = "\"" + valueStr + "\"";
            }
            pairs.Add("\"" + kvp.Key + "\":" + valueStr);
        }
        
        return "{" + string.Join(",", pairs) + "}";
    }
    
    /// <summary>
    /// Check if a string represents a numeric value
    /// </summary>
    private static bool IsNumeric(string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        
        // Check if it's an integer or float
        return int.TryParse(value, out _) || float.TryParse(value, out _);
    }
    
    /// <summary>
    /// Extract target from SignalR message
    /// </summary>
    public static string ExtractTarget(string message)
    {
        // Simple extraction of target from SignalR message
        if (message.Contains("\"target\":\""))
        {
            int startIndex = message.IndexOf("\"target\":\"") + 9;
            int endIndex = message.IndexOf("\"", startIndex);
            if (startIndex > 0 && endIndex > startIndex)
            {
                return message.Substring(startIndex, endIndex - startIndex);
            }
        }
        return "";
    }
    
    /// <summary>
    /// Extract arguments from SignalR message
    /// </summary>
    public static List<string> ExtractArguments(string message)
    {
        List<string> arguments = new List<string>();
        
        if (message.Contains("\"arguments\":"))
        {
            int startIndex = message.IndexOf("\"arguments\":[") + 12;
            int endIndex = message.IndexOf("]", startIndex);
            if (startIndex > 0 && endIndex > startIndex)
            {
                string argsString = message.Substring(startIndex, endIndex - startIndex);
                // Split by comma and clean up
                string[] args = argsString.Split(',');
                foreach (string arg in args)
                {
                    string cleanArg = arg.Trim().Trim('"');
                    arguments.Add(cleanArg);
                }
            }
        }
        
        return arguments;
    }
}
using System.Text.RegularExpressions;
using Core.Entites;

namespace Infrastructure.helpers;

public static class ValidatorHelper
{
    
    private static readonly string[] SqlInjectionPatterns =
    {
        @"--",                       
        @"\b(SELECT|INSERT|UPDATE|DELETE|DROP|EXEC)\b", 
        @"\b(UNION|ALTER|GRANT|REVOKE)\b", 
        @"[;']",                     
        @"(\bOR\b.*=)",              
        @"(\bAND\b.*=)"             
    };

    
    public static bool IsSqlInjectionSafe(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true; 

        foreach (var pattern in SqlInjectionPatterns)
        {
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                return false; 
        }

        return true;
    }

    
    public static string SanitizeSqlInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return input.Replace("'", "''");
    }
    
}
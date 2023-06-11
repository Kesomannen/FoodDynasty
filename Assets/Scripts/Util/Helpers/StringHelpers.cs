using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringHelpers {
    public static string FormatCamelCase(string input) {
        var formattedString = Regex.Replace(input, "([A-Z])", " $1").Trim();
        formattedString = char.ToUpper(formattedString[0]) + formattedString[1..];
        return formattedString;
    }

    public static string FormatMoney(double value) {
        switch (value) {
            case 0: return "$0";
            case < 0: return $"-${FormatMoney(-value)}";
        }

        var magnitude = (int)Math.Log10(value);
        var adjustedMagnitude = magnitude - magnitude % 3;
        
        var unit = adjustedMagnitude switch {
            < 3 => "",
            < 6 => "K",
            < 9 => "M",
            < 12 => "B",
            < 15 => "T",
            _ => "?"
        };
        return $"${value / Math.Pow(10, adjustedMagnitude):0.##}{unit}";
    }
}
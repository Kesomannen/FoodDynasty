using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dynasty.Library.Helpers {

public static class StringHelpers {
    public static string FormatCamelCase(string input) {
        var str = Regex.Replace(input, "([A-Z])", " $1").Trim();
        str = char.ToUpper(str[0]) + str[1..];
        return str;
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
            < 18 => "Qa",
            < 21 => "Qi",
            < 24 => "Sx",
            < 27 => "Sp",
            < 30 => "Oc",
            < 33 => "No",
            < 36 => "Dc",
            < 39 => "Ud",
            < 42 => "Dd",
            < 45 => "Td",
            < 48 => "Qad",
            < 51 => "Qid",
            < 54 => "Sxd",
            < 57 => "Spd",
            < 60 => "Ocd",
            < 63 => "Nod",
            < 66 => "Vg",
            < 69 => "Uvg",
            _ => "?"
        };
        return $"${value / Math.Pow(10, adjustedMagnitude):0.##}{unit}";
    }
}

}
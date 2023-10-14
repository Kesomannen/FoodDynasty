using System;
using Dynasty.Library.Helpers;
using UnityEngine;

namespace Dynasty.Core.Tooltip {

public struct PopupAction {
    public string Label;
    public PopupActionKind Kind;
    public Action Action;

    public static PopupAction Positive(string label, Action action = null) => new() {
        Label = label,
        Kind = PopupActionKind.Positive,
        Action = action
    };
    
    public static PopupAction Neutral(string label, Action action = null) => new() {
        Label = label,
        Kind = PopupActionKind.Neutral,
        Action = action
    };
    
    public static PopupAction Negative(string label, Action action = null) => new() {
        Label = label,
        Kind = PopupActionKind.Negative,
        Action = action
    };
    
    public Color Color => Kind switch {
        PopupActionKind.Positive => Colors.Positive,
        PopupActionKind.Neutral => Colors.Neutral,
        PopupActionKind.Negative => Colors.Negative,
        _ => throw new ArgumentOutOfRangeException()
    };
}

public struct PopupData {
    public string Header;
    public string Body;
    public PopupAction[] Actions;
}

public enum PopupActionKind {
    Positive,
    Neutral,
    Negative
}

}
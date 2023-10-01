using System;
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
        PopupActionKind.Positive => new Color(0.2588235f, 0.5660378f, 0.264472f),
        PopupActionKind.Neutral => new Color(0.1248665f, 0.2824448f, 0.4339623f),
        PopupActionKind.Negative => new Color(0.5660378f, 0.258989f, 0.264472f),
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
using System;
using UnityEngine;

namespace Dynasty.UI.Displays {

public struct PopupData {
    public string Header;
    public string Body;
    public PopupAction[] Actions;
}

public struct PopupAction {
    public string Label;
    public PopupActionKind Kind;
    public Action Action;

    public static PopupAction Accept(string label = "Accept", Action action = null) => new() {
        Label = label,
        Kind = PopupActionKind.Accept,
        Action = action
    };
    
    public static PopupAction Confirm(string label = "Confirm", Action action = null) => new() {
        Label = label,
        Kind = PopupActionKind.Confirm,
        Action = action
    };
    
    public static PopupAction Cancel(string label = "Cancel", Action action = null) => new() {
        Label = label,
        Kind = PopupActionKind.Cancel,
        Action = action
    };
    
    public Color Color => Kind switch {
        PopupActionKind.Accept => new Color(0.2588235f, 0.5660378f, 0.264472f),
        PopupActionKind.Confirm => new Color(0.1248665f, 0.2824448f, 0.4339623f),
        PopupActionKind.Cancel => new Color(0.5660378f, 0.258989f, 0.264472f),
        _ => throw new ArgumentOutOfRangeException()
    };
}

public enum PopupActionKind {
    Accept,
    Confirm,
    Cancel
}

}
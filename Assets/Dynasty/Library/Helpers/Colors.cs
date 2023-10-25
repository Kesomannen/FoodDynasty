using UnityEngine;

namespace Dynasty.Library {

public static class Colors {
    public static readonly Color Positive = new(0.2588235f, 0.5660378f, 0.264472f);
    public static readonly Color Neutral = new(0.1248665f, 0.2824448f, 0.4339623f);
    public static readonly Color Negative = new(0.5660378f, 0.258989f, 0.264472f);
    
    public static readonly Color PositiveText = new(0.6313726f, 0.8431373f, 0.6078432f);
    public static readonly Color WarningText = new(0.8431373f, 0.8431373f, 0.6078432f);
    public static readonly Color NegativeText = new(0.8962264f, 0.3438145f, 0.3086062f);
    public static readonly Color NeutralText = Color.white;
}

}
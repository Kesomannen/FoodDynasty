using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.UI.Tweening {

[CreateAssetMenu(menuName = "UI/Tweening Profile")]
public class UITweenProfile : ScriptableObject {
    [SerializeReference] List<TweenData> _onEnable;
    
    public List<TweenData> EnableTweens => _onEnable;
}

}
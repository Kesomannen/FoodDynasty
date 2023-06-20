using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Tweening Profile")]
public class UITweenProfile : ScriptableObject {
    [SerializeReference] List<UITweenData> _onEnable;
    
    public List<UITweenData> EnableTweens => _onEnable;
}
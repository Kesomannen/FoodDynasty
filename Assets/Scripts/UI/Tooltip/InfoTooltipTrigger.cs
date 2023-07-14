using UnityEngine;

public class InfoTooltipTrigger : TooltipTrigger<EntityInfo> {
    [SerializeField] string _title;
    [SerializeField] string _description;
    [SerializeField] Sprite _icon;
    
    protected override EntityInfo GetData() => new(_title, _description, _icon);
}
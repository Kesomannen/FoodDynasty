using UnityEngine;
using UnityEngine.EventSystems;

public abstract class TooltipTrigger<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] TooltipData<T> _params;
    
    protected abstract T GetData();
    
    public void OnPointerEnter(PointerEventData eventData) {
        _params.Show(GetData());
    }

    public void OnPointerExit(PointerEventData eventData) {
        _params.Hide();
    }
}
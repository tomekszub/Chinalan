using UnityEngine;
using UnityEngine.EventSystems;
public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]int skillIndex = 0;
    [SerializeField] UIManager UIManager;
    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        UIManager.FillToolTip(skillIndex);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        UIManager.HideToolTip();
    }
}

using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
            return;

        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
            return;

        base.OnDrag(eventData);
    }
}
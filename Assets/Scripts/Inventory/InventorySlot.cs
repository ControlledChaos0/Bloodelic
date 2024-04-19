using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class InventorySlot : MonoBehaviour
{
    private SmallHoldableObject heldObject;
    [SerializeField]
    private Image thisSlotImage;

    private bool hovered;
    private Color transparent = new Color(1,1,1,0);
    private Color opaque = new Color(1, 1, 1, 1);

    void Start() {
        InitializeSlot();
    }

    public void InitializeSlot()
    {;
        Debug.Log($"Image: {thisSlotImage}");
    }

    public SmallHoldableObject GetObject()
    {
        return heldObject;
    }

    public void SetObject(SmallHoldableObject curObject)
    {
        if (curObject == null)
        {
            thisSlotImage.sprite = null;
            thisSlotImage.color = transparent;
        } else
        {
            heldObject = curObject;
            Debug.Log($"Come onnnn workkkkk: {curObject}");
            thisSlotImage.sprite = curObject.icon;
            thisSlotImage.color = opaque;
        }
        
    }

}

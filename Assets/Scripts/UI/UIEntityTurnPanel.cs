using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIEntityTurnPanel : MonoBehaviour
{
    #region Properties
    [SerializeField] private Image entityIcon;
    [SerializeField] private Image background;
    [ReadOnly] public string entityName; // For ease of debug

    [SerializeField]
    private Color activeColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField]
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    #endregion

    public void SetEntityIcon(Sprite icon)
    {
        if (icon != null)
        {
            entityIcon.sprite = icon;
        }
    }
    
    public void SetActive()
    {
        entityIcon.color = activeColor;
        background.color = activeColor;
    }
    
    public void SetInactive()
    {
        entityIcon.color = inactiveColor;
        background.color = inactiveColor;
    }
    
}

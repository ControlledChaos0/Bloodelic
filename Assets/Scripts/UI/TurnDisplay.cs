using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnDisplay : MonoBehaviour
{
    [SerializeField] private GameObject entityTurnDisplayPrefab;
    private List<UIEntityTurnPanel> entityTurnPanels = new List<UIEntityTurnPanel>();

    [SerializeField] Button endTurnButton;
    
    void Awake()
    {
        // displays = new List<Image>();
        // foreach (Transform child in transform) {
        //     Image img = child.gameObject.GetComponent<Image>();
        //     if (img != null) {
        //         displays.Add(img);
        //     }
        // }
    }

    public void InitializeEntitiesTurnDisplays(List<Entity> allEntities)
    {
        foreach (var e in allEntities)
        {
            UIEntityTurnPanel newPanel = Instantiate(entityTurnDisplayPrefab).GetComponent<UIEntityTurnPanel>();
            entityTurnPanels.Add(newPanel);
            newPanel.SetEntityIcon(e.icon);
            newPanel.SetInactive();
            newPanel.entityName = e.name;

            // Attach to this transform as child
            newPanel.transform.SetParent(transform);
        }
    }
    
    public void UpdateDisplays(List<Entity> turnOrder, int currentIdx) {
        for (int i = 0; i < entityTurnPanels.Count; i++) 
        {
            // Collapse unused panels
            if (i >= turnOrder.Count)
            {
                entityTurnPanels[i].gameObject.SetActive(false);
                continue;
            }
            
            if (i != currentIdx)
            {
                entityTurnPanels[i].SetInactive();
            }
            else
            {
                entityTurnPanels[i].SetActive();
            }
        }
        
        // Update 
        UpdateDebugText(entityTurnPanels[currentIdx].entityName);
    }

    public void UpdateEndTurnButton(bool isActive)
    {
        endTurnButton.interactable = isActive;
    }
    
    #region Debugstuff
    [SerializeField] private TMP_Text debugText;

    void UpdateDebugText(string entityName)
    {
        debugText.text = "Current Turn: " + entityName;
    }
    
    #endregion
}

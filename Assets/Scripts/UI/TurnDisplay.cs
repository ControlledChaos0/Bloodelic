using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnDisplay : MonoBehaviour
{
    private List<Image> displays;
    void Awake()
    {
        displays = new List<Image>();
        foreach (Transform child in transform) {
            Image img = child.gameObject.GetComponent<Image>();
            if (img != null) {
                displays.Add(img);
            }
        }
    }

    public void UpdateDisplays(Entity[] turnOrder, int currentIdx) {
        for (int i = 0; i < displays.Count; i++) {
            displays[i].sprite = turnOrder[(i + currentIdx) % turnOrder.Length].icon;
        }
    }
}

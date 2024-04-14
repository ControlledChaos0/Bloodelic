using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectHelper
{
    public static GameObject GetParentGameObject(GameObject gameObject) {
        Transform parent = gameObject.transform.parent;
        if (parent != null) {
            return GetParentGameObject(parent.gameObject);
        } else {
            return gameObject;
        }
    }

    public static Selectable GetSelectableObject(GameObject gameObject) {
        Selectable selectable = gameObject.GetComponent<Selectable>();
        if (selectable == null) {
            Transform parent = gameObject.transform.parent;
            if (parent != null) {
                return GetSelectableObject(parent.gameObject);
            }
        }
        return selectable;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    public void Select(GameObject gameObject);
    public void HoverSelect(GameObject gameObject);
}

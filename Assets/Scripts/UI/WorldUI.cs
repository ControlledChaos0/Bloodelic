using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    private Camera m_Camera;
    protected void Awake()
    {
        //gameObject.SetActive(false);
        m_Camera = Camera.main;
        Selectable.SelectedUI += PresentWorldUI;
    }

    protected void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
    }

    public void PresentWorldUI(GameObject gameObject)
    {
        gameObject.SetActive(true);
        transform.position = gameObject.transform.position + new Vector3(0, 2f, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    private GameObject m_Parent;
    private Selectable m_Selectable;
    private Camera m_Camera;

    //Set on Selectable start
    public Selectable ObjSelect {
        get => m_Selectable;
        set => m_Selectable = value;
    }
    protected void Start()
    {
        m_Camera = CameraController.Instance.MainCamera;

        //World UI needs to operate without worrying about rotation of parent gameobject
        //Therefore, needs to disconnect from parent on runtime to operate not in local worldspace
        m_Parent = transform.parent.gameObject;
        transform.parent = null;
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

    public bool CheckIfUIObject(GameObject gameObject) {
        if (gameObject == null) {
            return false;
        }
        if (gameObject.Equals(this.gameObject)) {
            return true;
        }
        Transform parent = gameObject.transform.parent;
        if (parent == null) {
            return false;
        }
        return CheckIfUIObject(parent.gameObject);
    }
}

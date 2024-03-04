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

        m_Parent = transform.parent.gameObject;
        ObjSelect = m_Parent.GetComponent<Selectable>();

        Deactivate();
        //World UI needs to operate without worrying about rotation of parent gameobject
        //Therefore, needs to disconnect from parent on runtime to operate not in local worldspace
        Disconnect();
    }

    protected void LateUpdate()
    {
        //transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
    }

    public void Activate() {
        Debug.Log("Activate UI");
        gameObject.SetActive(true);
        ObjSelect.ClickAction += Click;
    }
    public void Deactivate() {
        gameObject.SetActive(false);
        ObjSelect.ClickAction -= Click;
    }

    private void Disconnect() {
        Vector3 parentPos = m_Parent.transform.position;
        Vector3 curPos = transform.position;
        transform.parent = null;
        transform.position = parentPos + curPos;
    }

    public void PresentWorldUI(GameObject gameObject)
    {
        gameObject.SetActive(true);
        transform.position = gameObject.transform.position + new Vector3(0, 2f, 0);
    }

    public void Click(GameObject gO) {
        if (!CheckIfUIObject(gO)) {
            ObjSelect.Deactivate();
            return;
        }
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

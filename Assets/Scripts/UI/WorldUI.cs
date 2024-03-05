using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    private GameObject m_Parent;
    private GameObject m_Canvas;
    private Selectable m_Selectable;
    private Camera m_Camera {
        get {
            if (CameraController.Instance != null) {
                return CameraController.Instance.MainCamera;
            } else {
                return null;
            }
        }
    }

    //Set on Selectable start
    public Selectable ObjSelect {
        get => m_Selectable;
        set => m_Selectable = value;
    }
    protected void Start()
    {
        m_Canvas = transform.GetChild(0).gameObject;
        m_Parent = transform.parent.gameObject;
        ObjSelect = m_Parent.GetComponent<Selectable>();

        Deactivate();
        //World UI needs to operate without worrying about rotation of parent gameobject
        //Therefore, needs to disconnect from parent on runtime to operate not in local worldspace
        Disconnect();
    }

    protected void Update() {
        transform.position = m_Parent.transform.position;
    }

    protected void LateUpdate()
    {
        //Rotate the 'arm' of the UI to the proper place
        //Not perfect, for some reason fucks up when reaching specific angles and positions
        //Look into why that's happening
        Vector3 lookVec = m_Camera.transform.position - transform.position;
        transform.right = Vector3.Cross(Vector3.Normalize(lookVec), m_Camera.transform.up);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        //Rotate the 'face'/canvas of the UI to look at the camera
        //For some reason it faces backwards if it looks at the camera, so I've flipped where it's supposed to point
        m_Canvas.transform.LookAt((-lookVec * 2) + m_Camera.transform.position, m_Camera.transform.up);
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
        Debug.Log("Do you work :D");
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

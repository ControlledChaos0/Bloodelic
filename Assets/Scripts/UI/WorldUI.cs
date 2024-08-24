using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorldUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _button;
    [SerializeField]
    private Vector2 _sizeCanvas;

    private GameObject m_Parent;
    private GameObject m_Canvas;
    private GameObject m_OptionsContainer;
    private RectTransform m_CanvasTransform;
    [SerializeField]
    private RectTransform m_OptionsTransform;
    private BoxCollider m_CanvasCollider;
    private Selectable m_Selectable;
    private bool m_AddButton;
    public GameObject Button {
        get => _button;
    }
    private Camera m_Camera {
        get {
            if (CameraController.Instance != null) {
                return CameraController.Instance.MainCamera;
            } else {
                return null;
            }
        }
    }
    protected void Start()
    {
        m_Canvas = transform.GetChild(0).gameObject;
        m_CanvasTransform = m_Canvas.GetComponent<RectTransform>();
        m_CanvasCollider = m_Canvas.GetComponent<BoxCollider>();
        m_CanvasTransform.sizeDelta = _sizeCanvas;
        m_Parent = transform.parent.gameObject;
        m_OptionsContainer = m_Canvas.transform.GetChild(0).gameObject;
        //m_OptionsTransform = m_OptionsContainer.GetComponent<RectTransform>();

        gameObject.SetActive(false);
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

    private void Disconnect() {
        Vector3 parentPos = m_Parent.transform.position;
        Vector3 curPos = transform.position;
        transform.parent = null;
        transform.position = parentPos + curPos;
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

    public void AddButtons(List<Behavior> behaviors) {
        if (m_AddButton)
        {
            return;
        }

        foreach (Behavior behavior in behaviors) 
        {
            if (behavior == null) {
                continue;
            }

            WorldUIButtonPool buttonPool = WorldUIButtonPool.Instance;
            GameObject buttonRealObj = buttonPool.GetButton();
            buttonRealObj.transform.SetParent(m_OptionsTransform);
            buttonRealObj.transform.localScale = Vector3.one;
            buttonRealObj.transform.localRotation = Quaternion.identity;
            buttonRealObj.transform.localPosition = Vector3.zero;
            buttonRealObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(behavior.Name);
            Button button = buttonRealObj.GetComponent<Button>();
            behavior.BehaviorButton = button;
            button.onClick.AddListener(behavior.StartBehaviorAction);
        }

        m_AddButton = true;
    }

    public void RemoveButtons() {
        for (int i = m_OptionsTransform.transform.childCount - 1; i >= 0 ; i-- )
        {
            Button button = m_OptionsTransform.transform.GetChild(i).gameObject.GetComponent<Button>(); 
            button.onClick.RemoveAllListeners();
            WorldUIButtonPool.Instance.ResetButton(m_OptionsTransform.transform.GetChild(i).gameObject);
        }
        m_AddButton = false;
    }
}

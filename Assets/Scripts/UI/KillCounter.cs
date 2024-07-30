using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_HumanCounterGUI;

    private int m_HumanCount;
    private int m_HumanKilled;
    private int m_HumanExited;
    private int HumansLeft
    {
        get {
            return m_HumanCount - m_HumanKilled - m_HumanExited;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_HumanCount = transform.childCount;

        m_HumanKilled = 0;
        m_HumanExited = 0;

        UpdateCounter();

        Human.DeathEvent += HandleDeathEvent;
    }

    void HandleDeathEvent()
    {
        m_HumanKilled++;
        UpdateCounter();
    }

    void UpdateCounter()
    {
        m_HumanCounterGUI.text = HumansLeft.ToString();

        if (HumansLeft == 0)
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }
    }

    
}

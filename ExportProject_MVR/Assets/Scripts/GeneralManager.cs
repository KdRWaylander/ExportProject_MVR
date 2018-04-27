using System.Collections;
using UnityEngine;

public class GeneralManager : MonoBehaviour {
    PlaySequenceManager     m_PSM;
    OutputWriter            m_OutputWriter;

    [SerializeField] GameObject m_shotgun;
    [SerializeField] GameObject m_crosshair;

    private int m_yInputCount;

    /* INITIALIZATION */
    void Start ()
    {
        m_PSM = GetComponent<PlaySequenceManager>();
        m_OutputWriter = GetComponent<OutputWriter>();
        m_yInputCount = 0;

        LoadCrossHair();
    }

    void Update()
    {
        // Start sequence: Y_manette ou A_clavier
        if ((Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.A)) && !m_PSM.GetIsRunning())
        {
            if (m_OutputWriter.GetRecord())
                m_OutputWriter.CreateFile();

            LoadGun();
            StartCoroutine(m_PSM.FirstStep());
        }

        // Quitter: Back_manette ou Echap_clavier
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton6))
        {
            Application.Quit();
        }
    }

    void LoadGun()
    {
        Transform gunNode = GameObject.Find("GunNode").transform;

        GameObject shotGun = Instantiate(m_shotgun, gunNode);
        shotGun.transform.parent = gunNode;
        shotGun.transform.localPosition = new Vector3(0, 0, 0);
        shotGun.transform.localRotation = Quaternion.identity;

    }

    void LoadCrossHair()
    {
        Transform gunNode = GameObject.Find("GunNode").transform;

        GameObject crossHair = Instantiate(m_crosshair, gunNode);
        crossHair.transform.parent = gunNode;
        crossHair.transform.localPosition = new Vector3(0, 0, 0);
        crossHair.transform.localRotation = Quaternion.identity;
    }
}
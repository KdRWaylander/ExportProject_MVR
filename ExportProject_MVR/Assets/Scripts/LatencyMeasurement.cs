using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LatencyMeasurement : MonoBehaviour {
    GameObject m_trackedDevice;
    Renderer m_outputImage;

    float m_Y;
    bool m_Run;
    Color m_transpRed, m_transpGreen;

    void Start()
    {
        m_trackedDevice = null;
        m_outputImage = GetComponent<Renderer>();
        m_outputImage.material.color = Color.black;

        m_transpRed = Color.red;
        m_transpGreen = Color.green;

        m_Run = false;

        Invoke("SetUp", 10f);
    }

    void Update ()
    {
        if (m_Run && m_trackedDevice.transform.position.y >= m_Y)
        {
            m_outputImage.material.color = m_transpGreen;
        }
        else if (m_Run && m_trackedDevice.transform.position.y < m_Y)
        {
            m_outputImage.material.color = m_transpRed;
        }
        else
        {
            return;
        }

        m_Y = m_trackedDevice.transform.position.y;
    }

    void SetUp()
    {
        m_trackedDevice = GameObject.Find("HeadNode");
        m_Y = m_trackedDevice.transform.position.y;

        m_outputImage.material.color = Color.blue;
        m_Run = true;
    }
}
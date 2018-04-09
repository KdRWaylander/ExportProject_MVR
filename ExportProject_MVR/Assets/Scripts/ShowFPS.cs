using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour {
    Text m_text;
    float m_fps, m_add;
    List<float> m_fpsList;

	void Start () {
        m_text = GetComponent<Text>();
        m_fpsList = new List<float>();
	}
	
	void Update () {
        m_fpsList.Add(Mathf.Floor(1f / Time.deltaTime));

        m_add = 0f;
        foreach (float f in m_fpsList) {
            m_add += f;
        }
        m_fps = Mathf.Floor(m_add / m_fpsList.Count);

        m_text.text = m_fps.ToString();
	}
}

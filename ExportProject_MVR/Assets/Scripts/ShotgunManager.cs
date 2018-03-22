using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunManager : MonoBehaviour {
    TimeManager m_timer;
    OutputWriter m_outputWriter;
    TargetManager m_targetManager;
    PlaySequenceManager m_playSequenceManager;
    HUDManager m_HUD;
    MatrixCalculation m_matrixCalculation;

    void Start () {
        m_timer                 = GameObject.Find("GeneralManager").GetComponent<TimeManager>();
        m_outputWriter          = GameObject.Find("GeneralManager").GetComponent<OutputWriter>();
        m_playSequenceManager   = GameObject.Find("GeneralManager").GetComponent<PlaySequenceManager>();
        m_matrixCalculation     = GameObject.Find("GeneralManager").GetComponent<MatrixCalculation>();
        m_targetManager         = GameObject.Find("TARGETS").GetComponent<TargetManager>();
        m_HUD                   = GameObject.Find("HUD").GetComponent<HUDManager>();
    }

    void Update () {
        // Left click or A button
        if (m_timer.GetIsRunning() && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0))) {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Target")
            {
                // Hide indication
                m_HUD.DisplayTexture(0);

                // Re-lock targets so there can only be one shoot
                m_targetManager.LockTargets();

                // Get info
                string targetName = hit.transform.parent.name;
                Vector3 targetCenter = hit.transform.position;
                Vector3 targetSize = hit.transform.gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
                Vector3 targetScale = hit.transform.lossyScale;
                Quaternion targetRotation = hit.transform.rotation;

                //Vector3 results = m_matrixCalculation.ComputeResult(hit, MatrixCalculation.ComputationMethod.Euler);
                Vector3 results = m_matrixCalculation.ComputeResult(hit, MatrixCalculation.ComputationMethod.Quaternion);

                // Output info in .txt file
                if (m_outputWriter.GetRecord())
                {
                    //m_outputWriter.AddTrialResults(targetName, targetCenter, targetRotation, targetSize, targetScale, hit.point, m_timer.Lap());
                    m_outputWriter.AddRelativeResults(targetName, results, m_timer.Lap());
                }

                // Launch next step or stop and reset timer
                StopAllCoroutines();

                if (m_playSequenceManager.GetLastIteration())
                {
                    // Sequence ending
                    m_timer.Stop();
                    m_timer.Reset();
                    m_playSequenceManager.SetIsRunning(false);

                    // Thumbs up ?
                }
                else
                {
                    StartCoroutine(m_playSequenceManager.NextStep());
                }
            }
        }
    }
}
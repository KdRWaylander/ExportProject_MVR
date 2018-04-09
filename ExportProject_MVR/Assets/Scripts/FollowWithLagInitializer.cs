using UnityEngine;

public class FollowWithLagInitializer : MonoBehaviour
{
    [SerializeField] bool m_lag;
    [SerializeField] float m_lagTime;

    void Start()
    {
        FollowWithLag_MVR headnode = GameObject.Find("HeadNode").AddComponent<FollowWithLag_MVR>();
        headnode.SetLag(m_lag);
        headnode.SetLagTime(m_lagTime);
    }
}
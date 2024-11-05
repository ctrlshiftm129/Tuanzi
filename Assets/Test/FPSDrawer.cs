using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDrawer : MonoBehaviour
{
    public float interval = 1f;

    private int m_lastFrame;
    private int m_frameCount;
    private float m_timeCount;

    // Update is called once per frame
    void Update()
    {
        ++m_frameCount;
        m_timeCount += Time.deltaTime;
        if (m_timeCount > 1)
        {
            m_lastFrame = m_frameCount;
            m_frameCount = 0;
            m_timeCount -= 1;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("FPS:" + m_lastFrame);
    }
}

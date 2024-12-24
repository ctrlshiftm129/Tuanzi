using UnityEngine;
using UnityEngine.UI;

public class BackgroundScrollBar : MonoBehaviour
{
    public float speed;
    public Vector2 offset;
    private float m_currentScroll;
    
    private Material m_material;
    
    void Start()
    {
        m_currentScroll = 0;
        m_material = GetComponent<Image>().material;
        m_material.mainTextureOffset = Vector2.zero;
    }

    void Update()
    {
        m_currentScroll += speed * Time.deltaTime;
        m_material.mainTextureOffset = new Vector2(offset.x * m_currentScroll, offset.y * m_currentScroll);
        m_currentScroll %= 1;
    }
}

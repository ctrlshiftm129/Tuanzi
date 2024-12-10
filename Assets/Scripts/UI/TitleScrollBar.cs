using UnityEngine;
using UnityEngine.UI;

public class TitleScrollBar : MonoBehaviour
{
    public float speed = 1;
    private float m_currentScroll;
    
    private Material m_material;
    
    void Start()
    {
        m_material = GetComponent<Image>().material;
    }

    void Update()
    {
        m_currentScroll += speed * Time.deltaTime;
        m_material.mainTextureOffset = new Vector2(-m_currentScroll, m_currentScroll);
        m_currentScroll %= 1;
    }
}

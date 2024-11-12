using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControll : MonoBehaviour
{
    public int speed = 5;
    Rigidbody2D m_rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private Vector2 m_moveDirection;

    // Update is called once per frame
    void Update()
    {
        m_moveDirection = InputUtils.GetMoveDirection();
    }

    private void FixedUpdate()
    {
        m_rigidbody2D.velocity = speed * m_moveDirection;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Íæ¼Ò¿ØÖÆÆ÷
/// </summary>
public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    public float fireRate = 1f;

    private float fireRateCounter = 0f;
    private Vector2 m_moveDirection;
    private Rigidbody2D m_rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SolveMoveInput();
        SolveAttackInput();
    }

    private void SolveMoveInput()
    {
        m_moveDirection = InputUtils.GetMoveDirection();
        m_rigidbody2D.velocity = m_moveDirection * speed;
    }

    private void SolveAttackInput()
    {
        fireRateCounter = Mathf.Max(0, fireRateCounter - Time.deltaTime);
        if (fireRateCounter > 0) return;
        var attackDirection = InputUtils.GetAttackDirection();
        if (attackDirection != Vector3.zero)
        {
            var bulletManager = ManagerLocator.Instance.Get<BulletManager>();
            bulletManager.PlayerShoot(transform.position, attackDirection, 10);
            fireRateCounter = fireRate;
        }

    }
}

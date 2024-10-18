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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SolveMoveInput();
        SolveAttackInput();
    }

    private void SolveMoveInput()
    {
        var moveDirection = InputUtils.GetMoveDirection();
        if (moveDirection != Vector3.zero)
        {
            transform.Translate(InputUtils.GetMoveDirection() * speed * Time.deltaTime);
        }
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

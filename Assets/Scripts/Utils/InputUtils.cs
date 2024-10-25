using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入相关工具函数
/// </summary>
public static class InputUtils
{
    /// <summary>
    /// 获取移动方向向量
    /// </summary>
    public static Vector2 GetMoveDirection()
    {
        int moveX = 0;
        int moveY = 0;
        if (Input.GetKey(KeyCode.W)) ++moveY;
        if (Input.GetKey(KeyCode.S)) --moveY;
        if (Input.GetKey(KeyCode.A)) --moveX;
        if (Input.GetKey(KeyCode.D)) ++moveX;

        return new Vector2(moveX, moveY).normalized;
    }

    /// <summary>
    /// 获取攻击方向向量
    /// </summary>
    [Obsolete("改为自动攻击，下个版本删除")]
    public static Vector3 GetAttackDirection()
    {
        int attackX = 0;
        int attackY = 0;
        if (Input.GetKey(KeyCode.UpArrow)) ++attackY;
        if (Input.GetKey(KeyCode.DownArrow)) --attackY;
        if (Input.GetKey(KeyCode.LeftArrow)) --attackX;
        if (Input.GetKey(KeyCode.RightArrow)) ++attackX;

        return new Vector3(attackX, attackY, 0).normalized;
    }
}

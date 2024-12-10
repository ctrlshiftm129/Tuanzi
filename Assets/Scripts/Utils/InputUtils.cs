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
}

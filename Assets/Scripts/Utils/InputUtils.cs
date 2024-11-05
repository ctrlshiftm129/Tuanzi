using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ع��ߺ���
/// </summary>
public static class InputUtils
{
    /// <summary>
    /// ��ȡ�ƶ���������
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

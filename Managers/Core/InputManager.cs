using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    // public Action<> MouseAction = null;

    public void OnUpdate()
    {
        //if (EventSystem.current.IsPointerOverGameObject())  // UI�� ������ return
        //    return;

        if (KeyAction != null)      // Ű���� �Է� ����
        {
            KeyAction.Invoke();
        }
    }

    public void Clear()
    {
        KeyAction = null;
    }
}
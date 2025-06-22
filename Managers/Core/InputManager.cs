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
        //if (EventSystem.current.IsPointerOverGameObject())  // UI를 누르면 return
        //    return;

        if (KeyAction != null)      // 키보드 입력 관련
        {
            KeyAction.Invoke();
        }
    }

    public void Clear()
    {
        KeyAction = null;
    }
}
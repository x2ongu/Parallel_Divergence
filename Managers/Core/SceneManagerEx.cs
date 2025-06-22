using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public Vector2 TeleportPos { get; set; }
    public bool IsTeleport { get; set; }

    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();

        LoadingScene.LoadNextScene(GetSceneName(type));
    }

    public void SceneTeleportPlayer()
    {
        Managers.Game.GetPlayer().transform.SetPositionAndRotation(TeleportPos, Quaternion.identity);

        IsTeleport = false;
    }

    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);

        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}

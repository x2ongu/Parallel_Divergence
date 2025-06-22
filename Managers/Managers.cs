using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    readonly GameManagerEx _game = new();

    public static GameManagerEx Game { get { return Instance._game; } }
    #endregion

    #region Core
    readonly DataManager _data = new();
    readonly InputManager _input = new();
    readonly PoolManager _pool = new();
    readonly ResourceManager _resource = new();
    readonly SceneManagerEx _scene = new();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        Input.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");

            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._pool.Init();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        Scene.Clear();

        Pool.Clear();
    }
}
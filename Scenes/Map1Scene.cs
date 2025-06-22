using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1Scene : BaseScene
{
    [SerializeField] private GameObject _bossRachel;
    [SerializeField] private GameObject _bossHpBar;

    private bool bossSpawn = false;

    private void Update()
    {
        if (Managers.Game.GetPlayer().transform.position.x > 2400)
        {
            if (!bossSpawn)
            {
                _bossRachel.SetActive(true);
                _bossHpBar.SetActive(true);
                bossSpawn = true;
            }
        }                   
    }

    protected override void Init()  // = Awake()
    {
        base.Init();

        Managers.Game.GetCamera();
    }

    protected override void OnEnableInit()
    {
        base.OnEnableInit();

        if (Managers.Scene.IsTeleport)
        {
            Managers.Scene.SceneTeleportPlayer();
        }
    }

    public override void Clear()
    {
        Managers.Game.Clear();
    }
}

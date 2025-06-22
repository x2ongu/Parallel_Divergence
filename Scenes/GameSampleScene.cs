using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSampleScene : BaseScene
{
    protected override void Init()  // = Awake()
    {
        base.Init();

        Managers.Game.GetCamera();
    }

    public override void Clear()
    {
        Managers.Game.Clear();
    }
}
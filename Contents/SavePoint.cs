using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public void SetSavePoint()
    {
        PlayerData data = Managers.Game.GetPlayer().GetComponent<PlayerData>();

        if (data == null)
        {
            Debug.Log("PlayerData is null");
            return;
        }

        data.SaveSceneIndex = (int)Managers.Scene.CurrentScene.GetComponent<BaseScene>().SceneType;
        data.SavePositionX = transform.position.x;
        data.SavePositionY = transform.position.y;

        Rest(data);
    }

    private void Rest(PlayerData data)
    {
        data.Hp = data.MaxHp;
        data.Battery = data.MaxBattery;

        data.HealCount = data.MaxHealCount;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int _hp;
    [SerializeField] private int _battery;

    [SerializeField] private int _healCount;

    [SerializeField] private int _saveSceneIndex;
    [SerializeField] private float _savePositionX;
    [SerializeField] private float _savePositionY;

    public int Hp { get { return _hp; } set { _hp = value; } }
    public int Battery { get { return _battery; } set { _battery = value; } }

    public int HealCount { get { return _healCount; } set { _healCount = value; } }

    public int SaveSceneIndex {get { return _saveSceneIndex; } set { _saveSceneIndex = value; } }
    public float SavePositionX { get { return _savePositionX; } set { _savePositionX = value; } }
    public float SavePositionY { get { return _savePositionY; } set { _savePositionY = value; } }

    // only getter
    public int AttackPower;

    public int MaxHp;
    public int MaxBattery;

    public float BatteryChargeTime;
    public float BatteryChargeAmount;

    public int MaxHealCount;
    public int HealAmount;

    public void TakeDamage(int damage)
    {
        Hp -= damage;

        if (Hp <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        Hp = 0;

        Vector2 spawnPos;

        if (SaveSceneIndex == (int)Managers.Scene.CurrentScene.SceneType)
            spawnPos = new(SavePositionX, SavePositionY);
        else
            spawnPos = Vector2.zero;

        StartCoroutine(Respawn(spawnPos));
    }

    public void Spawn()
    {
        Hp = MaxHp;
        Battery = MaxBattery;

    }

    public void Heal()
    {
        if (HealCount <= 0)
            return;

        // node ¼ÒÈ¯..
        HealCount--;
        Hp += HealAmount;
        if (Hp >= MaxHp)
        {
            Hp = MaxHp;
        }
    }

    private IEnumerator Respawn(Vector2 spawnPos)
    {
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(Managers.Game.FadeOutCoroutine());

        Managers.Game.GetPlayer().transform.position = spawnPos;
        Managers.Game.GetPlayer().m_anim.SetTrigger("Respawn");

        Hp = MaxHp;
        Battery = MaxBattery;

        yield return StartCoroutine(Managers.Game.FadeInCoroutine());

        Managers.Game.GetPlayer().CanMove = true;
        Managers.Game.GetPlayer().CanHit = true;
    }
}
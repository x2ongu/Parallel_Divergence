using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Archive : Animator_Base
{
    [Header("# Effect Transform")]
    [SerializeField] private Transform _handPos;

    public void SummonWeapon_PreAction()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Archive/SummonWeapon_PreAction");
        GameObject sphere = Managers.Resource.Instantiate("Effect/Archive/SummonWeapon");

        obj.transform.position = _handPos.position;
        sphere.transform.position = _handPos.position;

        sphere.GetComponent<SummonWeapon>().DoAttack(2f);
    }

    public void ExplosionWave()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Archive/ExplosionWave_PreAction");

        obj.transform.position = new Vector3(Managers.Game.GetPlayer().transform.position.x, -6.5f, 0f);

        StartCoroutine(obj.GetComponent<ExplosionWave_PreAction>().DoAttack());
    }

    public void VerticalDrop_PreAction()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Archive/VerticalDrop_PreAction");

        obj.transform.position = _handPos.position;

        obj.GetComponent<EffectTracer>().SetTargetAndDamage(_handPos, 0);
    }

    public void VerticalDrop_Small()
    {
        VerticalDrop("Small");
    }

    public void VerticalDrop_Middle()
    {
        VerticalDrop("Middle");
    }

    public void VerticalDrop_Large()
    {
        VerticalDrop("Large");
    }

    private void VerticalDrop(string size)
    {
        GameObject obj = Managers.Resource.Instantiate($"Effect/Archive/VerticalDrop_{size}");

        obj.transform.position = new Vector3(_handPos.position.x, -6f, 0f);
    }
}
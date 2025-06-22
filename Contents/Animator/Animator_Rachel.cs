using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Rachel : Animator_Base
{
    [Header("# Effect Transform")]
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _gauntletPointFront;
    [SerializeField] private Transform _gauntletPointBack;
    [SerializeField] private Transform _uppercutTransform;

    public void DeadEffect()
    {
        Managers.Resource.Instantiate("Effect/Rachel/Rachel_Dead").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
    }

    public void DoubleJab_Front()
    {
        GameObject frontObj = Managers.Resource.Instantiate("Effect/Rachel/DoubleJab");

        frontObj.transform.position = _gauntletPointFront.position;

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        frontObj.transform.localScale = scale;

        frontObj.GetComponent<EffectTracer>().SetTargetAndDamage(_gauntletPointFront, 1, transform);
    }

    public void DoubleJab_Back()
    {
        GameObject backObj = Managers.Resource.Instantiate("Effect/Rachel/DoubleJab");

        backObj.transform.position = _gauntletPointBack.position;

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        backObj.transform.localScale = scale;

        backObj.GetComponent<EffectTracer>().SetTargetAndDamage(_gauntletPointBack, 1, transform);
    }

    public void Uppercut()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/Uppercut");

        obj.transform.position = _uppercutTransform.position;

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(_uppercutTransform.position, new(30, 150), 0);

        foreach (var coll in colliders)
        {
            if (coll.CompareTag("Player"))
            {
                Managers.Game.GetPlayer().OnHit(5, obj.transform);
            }
        }
    }

    public void Uppdercut_PreAction()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/Uppercut_PreAction");

        obj.transform.position = _body.position;
    }

    public void GrandSlam()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/GrandSlam");

        obj.transform.position = _gauntletPointFront.position;

        RaycastHit2D hit = Physics2D.Raycast(obj.transform.position, Vector2.down, 3f, (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Ground_Wall));

        if (hit)
        {
            GameObject groundObj = Managers.Resource.Instantiate("Effect/Rachel/GrandSlam_Ground");

            groundObj.transform.position = hit.point;
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll(hit.point, new(50, 30), 0);

        foreach (var coll in colliders)
        {
            if (coll.CompareTag("Player"))
            {
                if (coll.GetComponent<PlayerMovement>().LastOnGroundTime < 0)
                    return;

                Managers.Game.GetPlayer().OnHit(3, obj.transform);
            }
        }

        Managers.Game.GetCamera()._impulseSource.GenerateImpulse();
    }

    public void GrandSlam_PreAction()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/GrandSlam_PreAction");

        obj.transform.position = transform.position;        
    }

    public void GrandSlam_Warning()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/GrandSlam_Warning");

        obj.transform.SetParent(_gauntletPointFront);
        obj.transform.localPosition = Vector3.zero;
    }

    public void ChargeFoward()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/ChargeFoward");

        obj.transform.position = _gauntletPointFront.position;

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;

        obj.GetComponent<EffectTracer>().SetTargetAndDamage(_gauntletPointFront, 3);
    }

    public void ChargeFoward_PreAction()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Rachel/ChargeFoward_PreAction");

        obj.transform.position = _body.position;
    }
}

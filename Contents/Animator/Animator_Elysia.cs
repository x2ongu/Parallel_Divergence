using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Animator_Elysia : Animator_Base
{
    [Header("# Effect Transform")]
    [SerializeField] private Transform _shootPointFront;
    [SerializeField] private Transform _shootPointBack;
    [SerializeField] private Transform _frontHand;
    [SerializeField] private Transform _backHand;
    [SerializeField] private Transform _dashAndBashPos;

    public void DeadEffect()
    {
        Managers.Resource.Instantiate("Effect/Elysia/Elysia_Dead").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
    }

    public void Warning_Both_Shoot()
    {
        Warning_Front_Shoot();
        Warning_Back_Shoot();
    }

    public void Warning_Front_Shoot()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Elysia/Elysia_Warning");

        obj.transform.SetParent(_shootPointFront);
        obj.transform.localPosition = Vector3.zero;
    }

    public void Warning_Back_Shoot()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Elysia/Elysia_Warning");

        obj.transform.SetParent(_shootPointBack);
        obj.transform.localPosition = Vector3.zero;
    }

    public void Shoot_OverdriveLaser()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Elysia/OverdriveLaser");

        obj.transform.position = _shootPointFront.position;

        Vector3 scale = obj.transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;

        obj.GetComponent<OverdriveLaserEffect>()._target = _shootPointFront;
    }
    
    public void Shoot_HomingBoltVolley()
    {
        SetHomingBoltVolleyEffect(_shootPointFront, _frontHand);
        SetHomingBoltVolleyEffect(_shootPointBack, _backHand);
        SetHomingBoltVolleyBullet();
    }

    public void Shoot_Front_StrafeAndShoot()
    {
        SetStrafeAndShootBullet(_shootPointFront);
    }

    public void Shoot_Back_StrafeAndShoot()
    {
        SetStrafeAndShootBullet(_shootPointBack);
    }

    public void SetDashAndBash()
    {
        GameObject obj = Managers.Resource.Instantiate("Effect/Elysia/DashAndBash");

        obj.transform.position = _dashAndBashPos.position;

        Vector3 scale = obj.transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        obj.transform.localScale = scale;
    }

    public void DoDashAndBash()
    {
        Collider2D coll = Physics2D.OverlapCircle(_dashAndBashPos.position, 7f, 1 << (int)Define.Layer.Player);
        PlayerController player;

        if (coll != null)
        {
            player = coll.gameObject.GetComponent<PlayerController>();

            if (player == null)
                return;

            Managers.Game.GetCamera()._impulseSource.GenerateImpulse();
            player.OnHit(3, transform);
        }        
    }

    private void SetHomingBoltVolleyEffect(Transform firePos, Transform handPos)
    {
        GameObject impact = Managers.Resource.Instantiate("Effect/Elysia/Shoot_Impact_Elysia");

        impact.transform.SetPositionAndRotation(firePos.position, handPos.rotation);

        Vector3 scale = impact.transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        impact.transform.localScale = scale;
    }
    
    private void SetHomingBoltVolleyBullet()
    {
        GameObject bullet1 = Managers.Resource.Instantiate("Effect/Elysia/Elysia_Bullet");
        GameObject bullet2 = Managers.Resource.Instantiate("Effect/Elysia/Elysia_Bullet");
        GameObject bullet3 = Managers.Resource.Instantiate("Effect/Elysia/Elysia_Bullet");

        if (transform.localScale.x == 1)
        {
            bullet1.transform.SetPositionAndRotation(_shootPointFront.position, Quaternion.Euler(0, 0, -10));
            bullet2.transform.SetPositionAndRotation(_shootPointFront.position, Quaternion.Euler(0, 0, -45));
            bullet3.transform.SetPositionAndRotation(_shootPointFront.position, Quaternion.Euler(0, 0, -80));
        }
        else if (transform.localScale.x == -1)
        {
            bullet1.transform.SetPositionAndRotation(_shootPointFront.position, Quaternion.Euler(0, 0, -170));
            bullet2.transform.SetPositionAndRotation(_shootPointFront.position, Quaternion.Euler(0, 0, -135));
            bullet3.transform.SetPositionAndRotation(_shootPointFront.position, Quaternion.Euler(0, 0, -100));
        }
    }

    private void SetStrafeAndShootBullet(Transform firePos)
    {
        GameObject impact = Managers.Resource.Instantiate("Effect/Elysia/Shoot_Impact_Elysia");
        GameObject bullet = Managers.Resource.Instantiate("Effect/Elysia/Elysia_Bullet");

        if (transform.localScale.x == 1)
        {
            impact.transform.SetPositionAndRotation(firePos.position, Quaternion.Euler(0, 0, 0));
            bullet.transform.SetPositionAndRotation(firePos.position, Quaternion.Euler(0, 0, 0));
        }
        else if (transform.localScale.x == -1)
        {
            impact.transform.SetPositionAndRotation(firePos.position, Quaternion.Euler(0, 0, 180));
            bullet.transform.SetPositionAndRotation(firePos.position, Quaternion.Euler(0, 0, 180));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_Sentry : General_Aireal
{
    [Header("# Sentry Setting")]
    public AnimationCurve _attackCurve;

    public Transform _attackPosFront;
    public Transform _attackPosBack;

    public override void Init()
    {
        base.Init();

        AdjustInitialFloatingHeight();
    }

    public void StartAttack()
    {
        StartCoroutine(RotateWithAnimationCurve(StateManager.Player));
    }

    public override void Attack()
    {
        GameObject bullet1;
        GameObject bullet2;

        string name = gameObject.name;
        int index = name.LastIndexOf('_');

        if (index >= 0)
            name = name.Substring(index + 1);

        if (name.Equals("A"))
        {
            bullet1 = Managers.Resource.Instantiate("Effect/Sentry_Bullet_Yellow");
            bullet2 = Managers.Resource.Instantiate("Effect/Sentry_Bullet_Yellow");
        }
        else
        {
            bullet1 = Managers.Resource.Instantiate("Effect/Sentry_Bullet_Green");
            bullet2 = Managers.Resource.Instantiate("Effect/Sentry_Bullet_Green");
        }

        Vector3 scale = transform.localScale;
        scale.x = transform.localScale.x == 1 ? 1 : -1;
        bullet1.transform.localScale = scale;
        bullet2.transform.localScale = scale;

        Quaternion currentRot = transform.rotation;
        Vector3 euler = currentRot.eulerAngles;
        euler.z = (scale.x == -1) ? euler.z + 180f : euler.z;
        Quaternion finalRot = Quaternion.Euler(euler);

        bullet1.transform.SetPositionAndRotation(_attackPosFront.position, finalRot);
        bullet2.transform.SetPositionAndRotation(_attackPosBack.position, finalRot);
    }

    public override void OnHitMethod(float targetPosX)
    {
        base.OnHitMethod(targetPosX);

        string name = gameObject.name;
        int index = name.LastIndexOf('_');

        if (index >= 0)
            name = name.Substring(index + 1);

        GameObject obj;

        if (name.Equals("A"))
            obj = Managers.Resource.Instantiate("Effect/AirealEnemyHit_Yellow");
        else
            obj = Managers.Resource.Instantiate("Effect/AirealEnemyHit_Green");

        obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
    }

    public override void Dead()
    {
        base.Dead();
        transform.rotation = Quaternion.identity;
        Managers.Resource.Instantiate("Effect/EnemyDeadA").transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Managers.Resource.Destroy(gameObject);
    }

    private IEnumerator RotateWithAnimationCurve(Transform player)
    {
        float duration = StateManager.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        float time = 0f;

        Vector3 adjustedTargetPos = player.position + new Vector3(0, 7f, 0);

        // 목표 각도 계산 (Z축)
        Vector2 direction = (adjustedTargetPos - transform.position).normalized * transform.localScale.x;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 내 현재 각도
        float startAngle = RB.rotation;

        // 예: 스프라이트가 기본적으로 위(y+)를 보고 있다면 보정
        // targetAngle -= 90f;

        while (time < duration)
        {
            float t = time / duration;
            float curveValue = _attackCurve.Evaluate(t); // 0 ~ 1

            float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, curveValue);
            RB.MoveRotation(currentAngle);

            time += Time.deltaTime;
            yield return null;
        }

        // 회전 원상 복구
        RB.MoveRotation(startAngle);
    }

    private void AdjustInitialFloatingHeight()
    {
        transform.rotation = Quaternion.identity;

        float targetHeight = 15f;
        float rayDistance = 20f;
        EnemyData.AirealHeight = targetHeight;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, _groundCheckLayer);

        if (hit.collider != null)
        {
            Vector3 pos = transform.position;
            float currentHeight = hit.distance;

            float offset = targetHeight - currentHeight;
            pos.y += offset;

            transform.position = pos;

            if (RB != null)
                RB.velocity = Vector2.zero; // 혹시 모를 이동 방지
        }
        else
        {
            Debug.LogWarning($"[AirealEnemy] Ground not found below at position {transform.position}");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 50f);
    }
}

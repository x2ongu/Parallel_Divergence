using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSlash : MonoBehaviour
{
    [Header("# Setting")]
    public LayerMask _hitLayer;
    public Vector2 _hitBox;

    public void DoUltimateSlash()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _hitBox, 0f, _hitLayer);

        if (colliders.Length != 0)
            StartCoroutine(UltimateSlashCoroutine(colliders));
    }

    private IEnumerator UltimateSlashCoroutine(Collider2D[] colliders)
    {
        List<GameObject> frozenEnemies = new();

        foreach (var item in colliders)
        {
            if (item.gameObject.CompareTag("Enemy"))
                Managers.Game.Freeze(item.gameObject);

            frozenEnemies.Add(item.gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        float duration = 0.5f;
        float interval = 0.05f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Managers.Game.GetCamera()._impulseSource.GenerateImpulse();

            foreach (var enemy in frozenEnemies)
            {
                if (enemy != null) // 살아있다면
                {
                    var damageable = enemy.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.OnAttacked(2, true);
                    }
                }
            }

            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }

        foreach (var enemy in frozenEnemies)
        {
            if (enemy != null)
            {
                Managers.Game.Unfreeze(enemy);
            }
        }
    }
}

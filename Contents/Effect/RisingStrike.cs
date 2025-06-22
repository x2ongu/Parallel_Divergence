using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingStrike : MonoBehaviour
{
    [Header("# Setting")]
    public LayerMask _hitLayer;
    public Vector2 _hitBox;

    public void DoRisingStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _hitBox, 0f, _hitLayer);

        if (colliders.Length != 0)
            StartCoroutine(RisingStrikeCoroutine(colliders));
    }

    private IEnumerator RisingStrikeCoroutine(Collider2D[] colliders)
    {
        List<GameObject> frozenEnemies = new();

        foreach (var item in colliders)
        {
            frozenEnemies.Add(item.gameObject);
        }

        float duration = 0.6f;
        float interval = 0.2f;
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
                        damageable.OnAttacked(2);
                    }
                }
            }

            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, _hitBox);
    }
}

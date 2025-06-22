using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverdriveLaserEffect : MonoBehaviour
{
    public Transform _target;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().OnHit(4, _target);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(RemoveEffect());
    }

    void Update()
    {
        if (_target != null)
        {
            transform.SetPositionAndRotation(new Vector3(_target.position.x, transform.position.y, 0), Quaternion.identity);
        }
    }

    IEnumerator RemoveEffect()
    {
        float duration = 0.7f;
        float interval = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Managers.Game.GetCamera()._impulseSource.GenerateImpulse();

            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(0.3f);

        Managers.Resource.Destroy(gameObject);
    }
}

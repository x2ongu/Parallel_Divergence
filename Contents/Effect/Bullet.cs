using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("# Bullet Setting")]
    [SerializeField] private GameObject _hitObject;

    [SerializeField] private LayerMask _hitLayer;

    [SerializeField] private float _moveSpeed = 70;

    private Coroutine _destroyCoroutine;
    
    private void OnEnable()
    {
        _destroyCoroutine = StartCoroutine(RemoveBullet());
    }

    void LateUpdate()
    {
        transform.Translate(_moveSpeed * Time.deltaTime * Vector3.right);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1f, _hitLayer);

        if (hit.collider != null)
            HitObj(hit);
    }

    void InstantiateHitEffect(RaycastHit2D hit)
    {
        StopCoroutine(_destroyCoroutine);

        GameObject obj = Managers.Resource.Instantiate(_hitObject);
        obj.transform.position = hit.point;

        Managers.Resource.Destroy(gameObject);
    }

    void HitObj(RaycastHit2D hit)
    {
        if (hit.transform.CompareTag("Player"))
        {
            PlayerController player = hit.collider.gameObject.GetComponent<PlayerController>();

            if (player == null || !player.CanHit)
                return;

            player.OnHit(2, transform);
        }

        InstantiateHitEffect(hit);
    }

    private IEnumerator RemoveBullet()
    {
        yield return new WaitForSeconds(3f);

        GameObject obj = Managers.Resource.Instantiate("Effect/Elysia/Elysia_Bullet_Hit");
        obj.transform.position = transform.position;

        Managers.Resource.Destroy(gameObject);
    }
}

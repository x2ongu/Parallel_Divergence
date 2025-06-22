using System.Collections;
using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    [SerializeField] float _duration;

    private void OnEnable()
    {
        StartCoroutine(DestroyEffect(_duration));
    }

    IEnumerator DestroyEffect(float duration)
    {
        yield return new WaitForSeconds(duration);

        Managers.Resource.Destroy(gameObject);
    }
}
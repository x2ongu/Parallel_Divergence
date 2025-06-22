using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionWave_PreAction : MonoBehaviour
{
    public IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(2f);

        GameObject obj = Managers.Resource.Instantiate("Effect/Archive/ExplosionWave");
        obj.transform.position = transform.position;

        Managers.Resource.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBoomSwitch : MonoBehaviour
{
    [SerializeField]
    GameObject _gate;

    [SerializeField]
    float _openTime = 3f;

    public void OnTriggerSwitch()
    {
        StartCoroutine(OpenTheGate());
    }

    public IEnumerator OpenTheGate()
    {
        _gate.SetActive(false);

        yield return new WaitForSeconds(_openTime);

        _gate.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSwitch : MonoBehaviour
{
    [Header("# Gate Swtich")]
    [SerializeField] private GameObject _gate;

    [SerializeField] private float _openTime = 3f;

    public void OnTriggerGateSwitch()
    {
        if (_openTime <= 0)
            _gate.SetActive(false);
        else
            StartCoroutine(OpenTheGate());
    }

    public IEnumerator OpenTheGate()
    {
        _gate.SetActive(false);

        yield return new WaitForSeconds(_openTime);

        _gate.SetActive(true);
    }
}

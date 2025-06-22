using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSwitch : MonoBehaviour
{
    [Header("# Moving Platform Swtich")]
    [SerializeField] private GameObject[] _platforms;

    public void OnTriggerMovingPlatformSwitch()
    {
        StartCoroutine(SwitchOn());
    }

    IEnumerator SwitchOn()
    {
        // È¿°úÀ½

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _platforms.Length; i++)
        {
            MovingPlatform platform = _platforms[i].GetComponent<MovingPlatform>();

            if (platform == null)
            {
                Debug.Log("null MovingPlatform Component");
                continue;
            }

            platform.InvokeMoveingPlatform();
        }
    }
}

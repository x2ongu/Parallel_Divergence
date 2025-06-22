using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public GameObject m_hackingEffect;

    [Header("# Camera")]
    public CinemachineVirtualCamera[] m_virtualCams;

    [Header("# Camera Setting")]
    [SerializeField]
    private Vector3 m_cameraOffset = new Vector3(0f, 10f, -18f);
    [SerializeField]
    private float m_cameraAngle = 2f;

    [Header("# Subject")]
    public Transform m_followProxy;

    [HideInInspector] public CinemachineImpulseSource _impulseSource;
    private Coroutine m_lerpRoutine;

    public int HackingIndex { get; private set; }

    private void Start() { Init(); }
    private void Init()
    {
        m_virtualCams = GetComponentsInChildren<CinemachineVirtualCamera>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();

        m_virtualCams[(int)Define.VirtualCamera.PlayerCam].Follow = Managers.Game.GetPlayer().transform;

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        brain.m_IgnoreTimeScale = true;

        for (int i = 0; i < m_virtualCams.Length; i++)
        {
            if (m_virtualCams[i].GetCinemachineComponent<CinemachineFramingTransposer>() == null)
            {
                Debug.Log("CinemachineVirtualCamera/Follow를 Framing Transposer으로 설정해 주세요!!");
                return;
            }

            if (m_virtualCams[i].GetCinemachineComponent<CinemachinePOV>() == null)
            {
                Debug.Log("CinemachineVirtualCamera/Aim을 POV로 설정해 주세요!!");
                return;
            }

            var body = m_virtualCams[i].GetCinemachineComponent<CinemachineFramingTransposer>();
            body.m_TrackedObjectOffset = m_cameraOffset;

            var aim = m_virtualCams[i].GetCinemachineComponent<CinemachinePOV>();
            aim.m_VerticalAxis.Value = m_cameraAngle;
            aim.m_HorizontalAxis.m_InputAxisName = "";
            aim.m_VerticalAxis.m_InputAxisName = "";
        }

        m_virtualCams[(int)Define.VirtualCamera.PlayerCam].Priority = 10;
        m_virtualCams[(int)Define.VirtualCamera.HackingCam].Priority = 20;

        m_virtualCams[(int)Define.VirtualCamera.PlayerCam].gameObject.SetActive(true);
        m_virtualCams[(int)Define.VirtualCamera.HackingCam].gameObject.SetActive(false);
    }

    public void SwitchHackingTarget(List<Collider2D> collList, int dir)
    {
        HackingIndex = (HackingIndex + dir + collList.Count) % collList.Count;

        if (m_lerpRoutine != null)
            StopCoroutine(m_lerpRoutine);

        m_lerpRoutine = StartCoroutine(LerpFollowTarget(collList[HackingIndex].transform));
    }

    IEnumerator LerpFollowTarget(Transform target)
    {
        if (m_hackingEffect != null)
            Managers.Resource.Destroy(m_hackingEffect);

        m_hackingEffect = Managers.Resource.Instantiate("Effect/Hacking");
        m_hackingEffect.transform.SetPositionAndRotation(target.position, Quaternion.identity);

        while (Vector3.Distance(m_followProxy.position, target.position) > 0.05f)
        {
            m_followProxy.position = Vector3.Lerp(m_followProxy.position, target.position, Time.unscaledDeltaTime * 5f);
            yield return null;
        }        

        m_followProxy.position = target.position;
    }
}
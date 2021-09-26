using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeShake : MonoBehaviour
{
    public static CameraShakeShake Instance { get; private set; }
    private CinemachineVirtualCamera cvCam;
    CinemachineBasicMultiChannelPerlin cvCamPerlin;
    float shakeOvertime;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        cvCam = GetComponent<CinemachineVirtualCamera>();
        cvCamPerlin = cvCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intesity, float time)
    {
        cvCamPerlin.m_AmplitudeGain = intesity;
        shakeOvertime = time;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeOvertime > 0 )
        {
            shakeOvertime -= Time.deltaTime;
            if (shakeOvertime <=0)
            {
                cvCamPerlin.m_AmplitudeGain = 0f;

            }
        }
    }
}

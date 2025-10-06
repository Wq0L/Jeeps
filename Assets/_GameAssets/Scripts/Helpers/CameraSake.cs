using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingIntensity;

    void Awake()
    {
        _cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private IEnumerator CameraShakeRoutine(float intensity, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        _cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        _shakeTimer = time;
        _shakeTimerTotal = time;
        _startingIntensity = intensity;
    }
    public void ShakeCamera(float intensity, float time, float delay = 0f)
    {
        StartCoroutine(CameraShakeRoutine(intensity, time, delay));
    }
    void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0f)
            {
                _cinemachineBasicMultiChannelPerlin.AmplitudeGain =
                    Mathf.Lerp(_startingIntensity, 0f, 1 - (_shakeTimer / _shakeTimerTotal)); 
            }
        }
    }
}

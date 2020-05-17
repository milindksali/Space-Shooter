using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private bool _isShakeCamera = false;
    [SerializeField] private float _shakeTime;
    private float shakeTime;
    [SerializeField] private float _shakeIntensity = 0.5f;
    [SerializeField] private float _shakeFadeRate = 0.5f;

    private Vector3 _initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.localPosition;
        shakeTime = _shakeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isShakeCamera)
        {
            if (shakeTime > 0)
            {
                transform.localPosition = _initialPosition + Random.insideUnitSphere * _shakeIntensity;
                shakeTime = shakeTime - _shakeFadeRate;
            }
            else
            {
                _isShakeCamera = false;
                shakeTime = 0f;
                transform.localPosition = _initialPosition;
            }
        }
    }

    public void ShakeCamera()
    {
        _isShakeCamera = true;
        shakeTime = _shakeTime;
    }
}

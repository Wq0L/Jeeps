using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerVehicleController : MonoBehaviour
{
    public class SpringData
    {
        public float _currentLenght;
        public float _currentVelocity;
    }

    private static readonly WheelType[] _wheels = new WheelType[]
    {
        WheelType.FrontLeft, WheelType.FrontRight, WheelType.BackLeft, WheelType.BackRight
    };

    [Header("References")]
    [SerializeField] private VehicleSettingsSO _vehicleSettings;
    [SerializeField] private Rigidbody _vehicleRigidbody;
    [SerializeField] private BoxCollider _VehicleColider;

    private Dictionary<WheelType, SpringData> _springDatas;
    private float _steerInput;
    private float _accelerationInput;

    private void Awake()
    {
        _springDatas = new Dictionary<WheelType, SpringData>();
        foreach (WheelType wheelType in _wheels)
        {
            _springDatas.Add(wheelType, new());
        }
    }


    private void Update()
    {
        SetSteerInput(Input.GetAxis("Horizontal"));
        SetAccelerateInput(Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        UpdateSuspension();
    }

    private void SetSteerInput(float steerInput)
    {
        _steerInput = Mathf.Clamp(steerInput, -1f, 1f);
    }

    private void SetAccelerateInput(float accelerationInput)
    {
        _accelerationInput = Mathf.Clamp(accelerationInput, -1f, 1f);
    }
    private void UpdateSuspension()
    {
        foreach (WheelType id in _springDatas.Keys)
        {
            CastSpring(id);
            float currentVelocity = _springDatas[id]._currentVelocity;
            float currentLenght = _springDatas[id]._currentLenght;

            float force = SpringMathExtensions.CalculateForceDamped(currentLenght, currentVelocity,
                _vehicleSettings.SpringRestLenght, _vehicleSettings.SpringStrenght, _vehicleSettings.SpringDamper);

            _vehicleRigidbody.AddForceAtPosition(force * transform.up, GetSpringPosition(id));
        }
    }

    private void CastSpring(WheelType wheelType)
    {
        Vector3 position = GetSpringPosition(wheelType);
        float previousLenght = _springDatas[wheelType]._currentLenght;
        float currentLenght;

        if (Physics.Raycast(position, -transform.up, out var hit, _vehicleSettings.SpringRestLenght))
        {
            currentLenght = hit.distance;
            Debug.DrawLine(position, hit.point, Color.red);
        }
        else
        {
            currentLenght = _vehicleSettings.SpringRestLenght;
            Debug.DrawRay(position, -transform.up * _vehicleSettings.SpringRestLenght, Color.green);
        }
        _springDatas[wheelType]._currentVelocity = (currentLenght - previousLenght) / Time.fixedDeltaTime;
        _springDatas[wheelType]._currentLenght = currentLenght;
    }

    private Vector3 GetSpringPosition(WheelType wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePostion(wheelType));
    }
    private Vector3 GetSpringRelativePostion(WheelType wheelType)
    {
        Vector3 boxSize = _VehicleColider.size;
        float boxBottom = boxSize.y * -0.5f;


        float paddingX = _vehicleSettings.WheelsPaddingX;
        float paddingZ = _vehicleSettings.WheelsPaddingZ;

        return wheelType switch
        {
            WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            _ => default
        };
    }
}
public static class SpringMathExtensions
{
    public static float CalculateForceDamped(float currentLenght, float LenghtVelocity,
    float restLenght, float strenght, float damper)
    {
        float lenghtOffset = restLenght - currentLenght;
        return (lenghtOffset * strenght) - (LenghtVelocity * damper);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle Settnigs", menuName = "Scriptable Objects/Vehicle Settings")]
public class VehicleSettingsSO : ScriptableObject
{
    [Header("Wheel Paddings")]
    [SerializeField] private float _wheelsPaddingX;
    [SerializeField] private float _wheelsPaddingZ;

    [Header("Suspension Settings")]
    [SerializeField] private float _springRestLenght;
    [SerializeField] private float _springStrenght;
    [SerializeField] private float _springDamper;

    [Header("Handling Settings")]
    [SerializeField] private float _steerAngle;
    [SerializeField] private float _frontWheelsGripFactor;
    [SerializeField] private float _rearWheelsGripFactor;

    [Header("Body Settings")]
    [SerializeField] private float _tireMass;

    [Header("Power Settings")]
    [SerializeField] private float _acceleratePower;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxReverseSpeed;

    public float WheelsPaddingX => _wheelsPaddingX;
    public float WheelsPaddingZ => _wheelsPaddingZ;
    public float SpringRestLenght => _springRestLenght;
    public float SpringStrenght => _springStrenght;
    public float SpringDamper => _springDamper;
    public float SteerAngle => _steerAngle;
    public float FrontWheelsGripFactor => _frontWheelsGripFactor;
    public float RearWheelsGripFactor => _rearWheelsGripFactor;
    public float TireMass => _tireMass;
    public float AcceleratePower => _acceleratePower;
    public float MaxSpeed => _maxSpeed;
    public float MaxReverseSpeed => _maxReverseSpeed;

   


}

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

    public float WheelsPaddingX => _wheelsPaddingX;
    public float WheelsPaddingZ => _wheelsPaddingZ;
    public float SpringRestLenght => _springRestLenght;
    public float SpringStrenght => _springStrenght;
    public float SpringDamper => _springDamper;


}

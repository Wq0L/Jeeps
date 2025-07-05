using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerVehicleVisualController : MonoBehaviour
{
    [SerializeField] private PlayerVehicleController _playerVehicleController;
    [SerializeField] private Transform _wheelFrontLeft, _wheelFrontRight, _wheelBackLeft, _wheelBackRight;
    [SerializeField] private float _wheelsSpinSpeed, _wheelYWhenSpringMin, _wheelYWhenSpringMax;
    private Quaternion _wheelFrontLeftRoll;
    private Quaternion _wheelFrontRightRoll;
    private float _springRestLenght;
    private float _forwardSpeed;
    private float _steerInput;

    private Dictionary<WheelType, float> _springCurrentLenght = new()
    {
        { WheelType.FrontLeft, 0f },
        { WheelType.FrontRight, 0f },
        { WheelType.BackLeft, 0f },
        { WheelType.BackRight, 0f }
    };

    private void Start()
    {
        _wheelFrontLeftRoll = _wheelFrontLeft.localRotation;
        _wheelFrontRightRoll = _wheelFrontRight.localRotation;
        _springRestLenght = _playerVehicleController.Settings.SpringRestLenght;
    }

    private void Update()
    {
        UpdateVisualState();
        RotateWheels();
    }

    private void UpdateVisualState()
    {
        _steerInput = Input.GetAxis("Horizontal");
        
        _forwardSpeed = Vector3.Dot(_playerVehicleController.Forward, _playerVehicleController.Velocity);

        _springCurrentLenght[WheelType.FrontLeft] = _playerVehicleController.GetSpringCurrentLenght(WheelType.FrontLeft);
        _springCurrentLenght[WheelType.FrontRight] = _playerVehicleController.GetSpringCurrentLenght(WheelType.FrontRight);
        _springCurrentLenght[WheelType.BackLeft] = _playerVehicleController.GetSpringCurrentLenght(WheelType.BackLeft);
        _springCurrentLenght[WheelType.BackRight] = _playerVehicleController.GetSpringCurrentLenght(WheelType.BackRight);
    }

    private void RotateWheels()
    {
        if (_springCurrentLenght[WheelType.FrontLeft] < _springRestLenght)
        {
            _wheelFrontLeftRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }
        if (_springCurrentLenght[WheelType.FrontRight] < _springRestLenght)
        {
            _wheelFrontRightRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }
        if (_springCurrentLenght[WheelType.BackLeft] < _springRestLenght)
        {
            _wheelBackLeft.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }
         if(_springCurrentLenght[WheelType.BackRight] < _springRestLenght)
        {
            _wheelBackRight.localRotation *= Quaternion.AngleAxis(_forwardSpeed* _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }
    }   
}

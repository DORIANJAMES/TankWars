using System;
using Unity.Netcode;
using UnityEngine;


public class PlayerAiming : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;
    [SerializeField] private JoystickVirtualController joystickVirtualController;

    private void Start()
    {
        joystickVirtualController = FindAnyObjectByType<JoystickVirtualController>();
        joystickVirtualController.AimJoystick += AimTurret;
    }

    private void OnDisable()
    {
        joystickVirtualController.AimJoystick -= AimTurret;
    }

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        AimTurret();
    }

    private void AimTurret()
    {
        Vector2 aimScreenPosition = inputReader.AimPosition;
        
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
        );
    }
}
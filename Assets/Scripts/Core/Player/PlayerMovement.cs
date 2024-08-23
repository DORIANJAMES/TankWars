using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private JoystickVirtualController joystickVirtualController;
    
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turningRate = 30f;
    [SerializeField] private Vector2 previousMovementInput;

    private float canAccelerete = 1f;

    private void Start()
    {
        joystickVirtualController = FindAnyObjectByType<JoystickVirtualController>();
    }
    

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        inputReader.MovementEvent += HandleMove;
        inputReader.AccelerateEvent += AccelaretionBool;
        joystickVirtualController = FindAnyObjectByType<JoystickVirtualController>();
    }

    private void AccelaretionBool(float obj)
    {
        this.canAccelerete = obj;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
        inputReader.MovementEvent -= HandleMove;
        inputReader.AccelerateEvent -= AccelaretionBool;
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        
        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        float moveDireciton = previousMovementInput.y * moveSpeed * canAccelerete;
        rb.velocity = (Vector2)bodyTransform.up * moveDireciton;
    }

    public void  HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }
    
}

using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turningRate = 30f;

    [SerializeField] private Vector2 previousMovementInput;

    

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        inputReader.MovementEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
        inputReader.MovementEvent -= HandleMove;
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

        float moveDireciton = previousMovementInput.y * moveSpeed;
        rb.velocity = (Vector2)bodyTransform.up * moveDireciton;
    }

    public void  HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;

    public float moveSpeed;

    private Vector2 _moveDirection;
    
    [Header("Player Input References")]
    public InputActionReference move;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {
        MovePlayer();
    }
    
    private void MovePlayer()
    {
        Vector2 direction = move.action.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * (moveSpeed * Time.deltaTime);
    }
}

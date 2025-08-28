using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCtrl : MonoBehaviour
{
    private CharacterInput inputActions;

    private Vector2 move;
    public Vector2 Move => move;

    public event Action ActivateAttack;

    void Awake()
    {
        inputActions = new CharacterInput();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.CharacterAction.Move.performed += OnMove;
        inputActions.CharacterAction.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        inputActions.Disable();

        inputActions.CharacterAction.Move.performed -= OnMove;
        inputActions.CharacterAction.Attack.performed -= OnAttack;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        ActivateAttack?.Invoke();
    }
}

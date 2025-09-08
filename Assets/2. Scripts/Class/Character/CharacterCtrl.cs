using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCtrl : MonoBehaviour
{
    private CharacterInput inputActions;
    private CharacterBase currentCharacter;

    private Vector2 move;
    public Vector2 Move => move;

    void Awake()
    {
        inputActions = new CharacterInput();

        currentCharacter = GetComponentInChildren<CharacterBase>();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.CharacterAction.Move.performed += OnMove;
        inputActions.CharacterAction.Move.canceled += OnMove;
        inputActions.CharacterAction.Attack.performed += OnAttack;
        inputActions.CharacterAction.SkillActive.performed += OnActiveSkill;
    }

    void OnDisable()
    {
        inputActions.Disable();

        inputActions.CharacterAction.Move.performed -= OnMove;
        inputActions.CharacterAction.Move.canceled -= OnMove;
        inputActions.CharacterAction.Attack.performed -= OnAttack;
        inputActions.CharacterAction.SkillActive.performed -= OnActiveSkill;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        currentCharacter.ActiveAttack();
    }

    private void OnActiveSkill(InputAction.CallbackContext context)
    {
        currentCharacter.UseActiveSkill();
    }
}

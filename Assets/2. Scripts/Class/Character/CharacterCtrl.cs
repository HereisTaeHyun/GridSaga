using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCtrl : MonoBehaviour
{
    private CharacterInput inputActions;
    private CharacterBase currentCharacter;
    private Vector2 aimDir;

    private Vector2 move;
    public Vector2 Move => move;
    public Vector2 AimDir => aimDir;

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

    void FixedUpdate()
    {
        Aim();
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

    private void Aim()
    {
        Vector2 screen = inputActions.CharacterAction.Aim.ReadValue<Vector2>();
        var cam = Camera.main;

        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        aimDir = mouseWorld - currentCharacter.transform.position;
    }
}

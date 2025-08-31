using UnityEngine;

public class Warrior : CharacterBase
{
    protected override void Init()
    {
        base.Init();
    }

    private void Awake()
    {
        Init();
    }

    void FixedUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        isMove = characterCtrl.Move.magnitude > 0.0001f;
        anim.SetBool(isMoveHash, isMove);
        
        Vector2 dir = UtilityManager.utility.DirSet(characterCtrl.Move);

        if (isMove)
        {
            lastDir.x = dir.x;
            lastDir.y = dir.y;
        }

        anim.SetFloat(moveXHash, lastDir.x);
        anim.SetFloat(moveYHash, lastDir.y);

        Vector2 newVelocity = new Vector2(currentSpeed * characterCtrl.Move.x, currentSpeed * characterCtrl.Move.y);
        rb2D.linearVelocity = newVelocity;
    }

    protected override void Attack()
    {
        anim.SetTrigger(attackHash);
    }
}

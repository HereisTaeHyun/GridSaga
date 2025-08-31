using System.Collections;
using UnityEngine;

public class Warrior : CharacterBase
{
    protected override void Init()
    {
        base.Init();

        attackActiveTime = 0.25f;
        attackEndTime = 0.8f;

        attackRadius = 2.4f;
        attackDegree = 90.0f;
        dieTime = 1.5f;
    }

    private void Awake()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (characterState == CharacterState.Die)
        {
            return;
        }
        Move();
    }

    protected override void Move()
    {
        isMove = characterCtrl.Move.magnitude > 0.0001f;
        anim.SetBool(isMoveHash, isMove);

        Vector2 dir = UtilityManager.utility.DirSet(characterCtrl.Move);

        if (isMove)
        {
            characterState = CharacterState.Move;
            lastDir.x = dir.x;
            lastDir.y = dir.y;
        }
        else
        {
            characterState = CharacterState.Idle;
        }

        anim.SetFloat(moveXHash, lastDir.x);
        anim.SetFloat(moveYHash, lastDir.y);

        Vector2 newVelocity = new Vector2(currentSpeed * characterCtrl.Move.x, currentSpeed * characterCtrl.Move.y);
        rb2D.linearVelocity = newVelocity;
    }

    protected override IEnumerator Attack()
    {
        if (characterState == CharacterState.Die || characterState == CharacterState.Move)
        {
            yield break;
        }

        characterState = CharacterState.Attack;

        // 공격 적용
        anim.SetTrigger(attackHash);
        yield return new WaitForSeconds(attackActiveTime);

        // 부채꼴 공격 범위 내 적들에게 공격 적용
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius, attackableLayer);
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<ICombat>();
            int damage = UtilityManager.utility.CalculateDamage(this, target);
            target.GetDamage(target, damage);
        }

        yield return new WaitForSeconds(attackEndTime);
        characterState = CharacterState.Idle;
    }
}

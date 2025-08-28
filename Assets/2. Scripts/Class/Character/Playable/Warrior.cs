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
        Vector2 dir = UtilityManager.utility.DirSet(characterCtrl.Move);

        anim.SetFloat(moveXHash, dir.x);
        anim.SetFloat(moveYHash, dir.y);
    }

    protected override void Attack()
    {
        anim.SetTrigger(attackHash);
    }
}

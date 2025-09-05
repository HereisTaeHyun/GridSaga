using System;
using UnityEngine;

public interface ICombat
{
    int MaxHp { get; }
    int CurrentHp { get; }
    int CurrentDefense { get; }
    int CurrentAttack { get; }
    int CurrentSpeed { get; }
    float CurrentCritRate { get; }
    float CurrentAttackRange { get; }
    Vector3 Position { get; }
    LayerMask AttackableLayer { get; }
    void UsePassiveSkill();
    bool IsPassiveTriggered { get; set; }
    void UseActiveSkill();
    void GetDamage(ICombat target, int damage);
    void ChangeStat(StatKind statKind, int value);
    event Action<int, int> HpChanged;
}

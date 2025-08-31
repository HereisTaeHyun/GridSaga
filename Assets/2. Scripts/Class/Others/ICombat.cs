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
    void InvokeDamageDataEvent(float damage);
    void UsePassiveSkill();
    void UseActiveSkill();
    void ChangeStat(StatKind statKind, int value);
    event Action<float> RefreshDamageData;
}

using UnityEngine;

// 이벤트 전달 용 데이터 버스, UI나 애니메이션 재생이나 트리거 등 HP 사용하는 요소들이 구독하게 할 것
public readonly struct DamageDataBus
{
    public readonly ICombat attacker;
    public readonly ICombat target;
    public readonly int damage;
    public readonly int currentHp;
    public readonly bool isDied;

    public DamageDataBus(ICombat attackerParam, ICombat targetParam, int damageParam, int currentHpParam)
    {
        attacker = attackerParam;
        target = targetParam;
        damage = damageParam;
        currentHp = currentHpParam;
        isDied = currentHp <= 0;
    }
}

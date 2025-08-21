using System.Collections.Generic;
using UnityEngine;

// 던전 내부에서 시작, 진행, 게임 오버를 다루는 총괄자 매니저
public class GameManager : MonoBehaviour
{
    private float floorTimer;
    private int currentStage;
    private int currentFloor;
    private bool isMove;
    private bool isAuto;
    private int defenseTuningFactor = 50;


    // 싱글톤 선언
    public static GameManager gameManager = null;
    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else if (gameManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public int CalculateDamage(CharacterBase attacker, CharacterBase target)
    {
        // 계산 필요 로직 정리
        float atk = Mathf.Max(0f, attacker.CurrentAttack);
        float def = Mathf.Max(0f, target.CurrentDefense);
        bool isCrit = CalculateIsCrit(attacker);

        // 크리티컬 여부에 따라 atk 분리
        if (isCrit)
        {
            atk = attacker.CurrentAttack * 2;
        }
        else
        {
            atk = attacker.CurrentAttack;
        }

        // 방어도에 따른 차감
        float K = Mathf.Max(0.0001f, defenseTuningFactor);
        float damageMultiplier = K / (def + K);
        float rawDamage = atk * damageMultiplier;
        int damage = Mathf.Max(1, Mathf.FloorToInt(rawDamage));
        return damage;
    }

    private bool CalculateIsCrit(CharacterBase attacker)
    {
        float rollPercent = Random.Range(0.0f, 100.0f);
        if (rollPercent - attacker.CurrentCritRate <= 0)
        {
            return true;
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 현재 스테이지 또는 계층에서 무슨 일이 벌어질지는 정의하는 매니저
public class DungeonManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> monsterPrefabs;
    public List<CharacterBase> unitOnStage = new List<CharacterBase>();

    [SerializeField] private GameObject board;
    private Queue<CharacterBase> attackQueue = new Queue<CharacterBase>();

    private bool isBattle;

    // 공격 속도 제어
    // 스피드 1 = 0.25초의 딜레이 경감을 가짐
    private float delayBySpeed = 0.25f;
    private float minDelay = 0.25f;
    private float maxDelay = 2.5f;

    // 보스 스테이지인지 아닌지를 GameManager가 체크하게 하여 스테이지 진행인지 보상 수령 및 계층 이동인지 참고 필요
    private bool isBoss;
    public bool IsBoss => isBoss;

    // 싱글톤 선언
    public static DungeonManager dungeonManager = null;
    void Awake()
    {
        if (dungeonManager == null)
        {
            dungeonManager = this;
        }
        else if (dungeonManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // 테스트용으로 Enable, Start에 뒀지만 이후 GameManager에서 StageChange 등 이벤트 트리거로 몬스터 배치 예정
    // 방식도 즉시 생성 대신 미리 씬에 배치되어 있거나 걸어 오는 등의 방식으로 자연스럽게 스폰시킬 것
    void OnEnable()
    {
        var formation = board.transform.Find("RightFormation");
        unitOnStage = new List<CharacterBase>();

        for (int i = 0; i < monsterPrefabs.Count; i++)
        {
            var unit = monsterPrefabs[i];
            var unitPosition = formation.GetChild(i);

            var instanceUnit = Instantiate(unit, unitPosition);
            instanceUnit.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
            unitOnStage.Add(instanceUnit.GetComponent<CharacterBase>());
        }

        // speed에 따라 정렬해서 Queue에 넣기
        unitOnStage.Sort((a, b) => b.CurrentSpeed.CompareTo(a.CurrentSpeed));
        foreach (var elem in unitOnStage)
        {
            elem.formation = true;
            attackQueue.Enqueue(elem);
        }
    }

    void Start()
    {
        isBattle = true;
        StartCoroutine(Battle());
    }

    private IEnumerator Battle()
    {
        while (isBattle)
        {
            var enemies = CommandingPlayer.commandingPlayer.unitOnStage;
            if (enemies == null || enemies.Count == 0)
            {
                isBattle = false;
                break;
            }

            // 공격자와 타겟 지정
            var attacker = attackQueue.Dequeue();
            var target = SetTarget(enemies, attacker);

            // 스피드에 따른 딜레이 지정
            float wait = GetDelay(attacker.CurrentSpeed);
            yield return new WaitForSeconds(wait);

            // 공격 실제 적용 후 다시 큐로
            StartCoroutine(attacker.Attack(target));
            GameManager.gameManager.ApplyDamage(attacker, target);
            attackQueue.Enqueue(attacker);
        }
    }

    private CharacterBase SetTarget(List<CharacterBase> enemies, CharacterBase attacker)
    {
        var selected = new List<CharacterBase>();

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i].GetComponent<CharacterBase>();
            if (enemy != attacker && enemy.canTarget)
            {
                selected.Add(enemy);
            }
        }

        return selected[Random.Range(0, selected.Count)];
    }

    private float GetDelay(int speed)
    {
        speed = Mathf.Max(0, speed);
        float delay = maxDelay - (speed * delayBySpeed);
        return Mathf.Max(minDelay, delay);
    }
}

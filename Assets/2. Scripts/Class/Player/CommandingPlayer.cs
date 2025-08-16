using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CommandingPlayer : MonoBehaviour
{
    private int userId;
    private string userName;

    [SerializeField] private List<GameObject> partyPrefabs;
    public List<CharacterBase> unitOnStage = new List<CharacterBase>();

    [SerializeField] private GameObject board;
    private Queue<CharacterBase> attackQueue = new Queue<CharacterBase>();
    private bool isBattle;

    // 공격 속도 제어
    // 스피드 1 = 0.25초의 딜레이 경감을 가짐
    private float delayBySpeed = 0.25f;
    private float minDelay = 0.25f;
    private float maxDelay = 2.5f;


    // 싱글톤 선언
    public static CommandingPlayer commandingPlayer = null;
    void Awake()
    {
        if (commandingPlayer == null)
        {
            commandingPlayer = this;
        }
        else if (commandingPlayer != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // 테스트 용으로 OnEnable, Start 등에 뒀지만 이후 StageChange 등으로 분기할 것
    void OnEnable()
    {
        var formation = board.transform.Find("LeftFormation");

        for (int i = 0; i < partyPrefabs.Count; i++)
        {
            var unit = partyPrefabs[i];

            var unitPosition = formation.GetChild(i);

            var instanceUnit = Instantiate(unit, unitPosition);
            instanceUnit.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
            unitOnStage.Add(instanceUnit.GetComponent<CharacterBase>());
        }

        // speed에 따라 정렬해서 Queue에 넣기
        unitOnStage.Sort((a, b) => b.CurrentSpeed.CompareTo(a.CurrentSpeed));
        foreach (var elem in unitOnStage)
        {
            elem.formation = false;
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
            var enemies = DungeonManager.dungeonManager.unitOnStage;
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
            yield return StartCoroutine(attacker.Attack(target));
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
            if (enemy != attacker && enemy.CanBeTarget)
            {
                selected.Add(enemy);
            }
        }

        Debug.Log($"selected.Count : {selected.Count}");

        return selected[Random.Range(0, selected.Count)];
    }

    private float GetDelay(int speed)
    {
        speed = Mathf.Max(0, speed);
        float delay = maxDelay - (speed * delayBySpeed);
        return Mathf.Max(minDelay, delay);
    }
}

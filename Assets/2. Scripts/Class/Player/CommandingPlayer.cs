using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CommandingPlayer : MonoBehaviour
{
    private int userId;
    private string userName;

    [SerializeField] private List<GameObject> partyPrefabs;
    private List<CharacterBase> unitOnStage = new List<CharacterBase>();

    [SerializeField] private GameObject board;
    private Queue<CharacterBase> attackQueue = new Queue<CharacterBase>();
    private bool isBattle;

    // 아군 공격 속도 제어
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
            }

            // 공격자와 타겟 지정
            var attacker = attackQueue.Dequeue();
            int targetIdx = Random.Range(0, DungeonManager.dungeonManager.unitOnStage.Count);
            var target = DungeonManager.dungeonManager.unitOnStage[targetIdx].GetComponent<CharacterBase>();

            // 스피드에 따른 딜레이 지정
            float wait = GetDelay(attacker.CurrentSpeed);
            yield return new WaitForSeconds(wait);

            // 공격 실제 적용 후 다시 큐로
            attacker.Attack();
            GameManager.gameManager.ApplyDamage(attacker, target);
            attackQueue.Enqueue(attacker);
        }
    }

    private float GetDelay(int speed)
    {
        speed = Mathf.Max(0, speed);
        float delay = maxDelay - (speed * delayBySpeed);
        return Mathf.Max(minDelay, delay);
    }
}

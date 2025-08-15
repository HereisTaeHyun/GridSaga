using UnityEngine;
using System.Collections.Generic;

public class CommandingPlayer : MonoBehaviour
{
    private int userId;
    private string userName;

    [SerializeField] private List<GameObject> partyPrefabs;
    private List<CharacterBase> unitOnStage = new List<CharacterBase>();
    
    [SerializeField] private GameObject board;
    private Queue<CharacterBase> attackQueue = new Queue<CharacterBase>();


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

    void Start()
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

    // 공격 타겟 지정 및 공격
    private void CommandAttack()
    {
        // 공격자와 타겟 지정
        var attacker = attackQueue.Dequeue();
        int targetIdx = Random.Range(0, DungeonManager.dungeonManager.unitOnStage.Count);
        var target = DungeonManager.dungeonManager.unitOnStage[targetIdx].GetComponent<CharacterBase>();

        // 공격 실제 적용 후 다시 큐로
        attacker.Attack();
        GameManager.gameManager.ApplyDamage(attacker, target);
        attackQueue.Enqueue(attacker);
    }
}

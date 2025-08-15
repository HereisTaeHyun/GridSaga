using UnityEngine;
using System.Collections.Generic;

public class CommandingPlayer : MonoBehaviour
{
    private int userId;
    private string userName;
    [SerializeField] private List<GameObject> party;
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
        var unitOnStage = new List<CharacterBase>();

        for (int i = 0; i < party.Count; i++)
        {
            var unit = party[i];

            var unitPosition = formation.GetChild(i);

            var instanceUnit = Instantiate(unit, unitPosition);
            instanceUnit.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
            unitOnStage.Add(instanceUnit.GetComponent<CharacterBase>());
        }

        // speed에 따라 정렬해서 Queue에 넣기
        unitOnStage.Sort((a, b) => b.CurrentSpeed.CompareTo(a.CurrentSpeed));
        foreach (var elem in unitOnStage)
        {
            Debug.Log(elem.CurrentSpeed);
            attackQueue.Enqueue(elem);
        }

        CommandAttack();
    }

    private void CommandAttack()
    {
        var attacker = attackQueue.Dequeue();

        int targetIdx = Random.Range(0, DungeonManager.dungeonManager.CurrentMonsters.Count);
        var target = DungeonManager.dungeonManager.CurrentMonsters[targetIdx].GetComponent<CharacterBase>();

        attacker.Attack();
        GameManager.gameManager.ApplyDamage(attacker, target);
    }
}

using System.Collections.Generic;
using UnityEngine;

// 현재 스테이지 또는 계층에서 무슨 일이 벌어질지는 정의하는 매니저
public class DungeonManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> currentMonsters;
    public IReadOnlyList<GameObject> CurrentMonsters => currentMonsters;

    [SerializeField] private GameObject board;
    private Queue<CharacterBase> attackQueue = new Queue<CharacterBase>();

    // 보스 스테이지인지 아닌지를 GameManager가 체크하게 하여 스테이지 진행인지 보상 수령 및 계층 이동인지 참고 필요
    private bool isBoss;
    public bool IsBoss => isBoss;

    // 싱글톤 선언
    public static DungeonManager dungeonManager = null;
    void Awake()
    {
        if(dungeonManager == null)
        {
            dungeonManager = this;
        }
        else if(dungeonManager != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // 테스트용으로 Start에 뒀지만 이후 GameManager에서 StageChange 등 이벤트 트리거로 몬스터 배치 예정
    // 방식도 즉시 생성 대신 미리 씬에 배치되어 있거나 걸어 오는 등의 방식으로 자연스럽게 스폰시킬 것
    void Start()
    {
        var formation = board.transform.Find("RightFormation");
        var unitOnStage = new List<CharacterBase>();

        for (int i = 0; i < currentMonsters.Count; i++)
        {
            var unit = currentMonsters[i];
            var unitPosition = formation.GetChild(i);

            var instanceUnit = Instantiate(unit, unitPosition);
            instanceUnit.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
            unitOnStage.Add(instanceUnit.GetComponent<CharacterBase>());
        }

        // speed에 따라 정렬해서 Queue에 넣기
        unitOnStage.Sort((a, b) => b.CharacterData.Speed.CompareTo(a.CharacterData.Speed));
        foreach (var elem in unitOnStage)
        {
            attackQueue.Enqueue(elem);
        }
    }
}

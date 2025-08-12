using System.Collections.Generic;
using UnityEngine;

// 현재 스테이지 또는 계층에서 무슨 일이 벌어질지는 정의하는 매니저
public class DungeonManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> ableMonster;
    [SerializeField] private GameObject board;

    // 보스 스테이지인지 아닌지를 GameManager가 체크하게 하여 스테이지 진행인지 보상 수령 및 계층 이동인지 참고 필요
    private bool isBoss;
    public bool IsBoss => isBoss;

    // 테스트용으로 Start에 뒀지만 이후 GameManager에서 StageChange 등 이벤트 트리거로 몬스터 배치 예정
    // 방식도 즉시 생성 대신 미리 씬에 배치되어 있거나 걸어 오는 등의 방식으로 자연스럽게 스폰시킬 것
    void Start()
    {
        var formation = board.transform.Find("RightFormation");

        for (int i = 0; i < ableMonster.Count; i++)
        {
            var unit = ableMonster[i];
            var unitPosition = formation.GetChild(i);

            var instance = Instantiate(unit, unitPosition);
            instance.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
        }
    }
}

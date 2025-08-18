using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 현재 스테이지 또는 계층에서 무슨 일이 벌어질지는 정의하는 매니저
public class DungeonManager : MonoBehaviour
{


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
}

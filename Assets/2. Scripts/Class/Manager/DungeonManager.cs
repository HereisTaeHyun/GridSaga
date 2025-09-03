using UnityEngine;
using UnityEngine.Tilemaps;

// 던전 렌더링 담당자
public class DungeonManager : MonoBehaviour
{
    // 보스 스테이지인지 아닌지를 GameManager가 체크하게 하여 스테이지 진행인지 보상 수령 및 계층 이동인지 참고 필요
    private bool isBoss;
    public bool IsBoss => isBoss;

    //     1. 방은 Isometric한 정사각형 모양을 기본으로 여러 형태나 기믹, 아이템을 가질 가능성이 있다
    //     2. 복도는 각 방에 연결될 가능성이 있으며 4 방향을 가진다
    //     3. DungeonManager는 여러 방 형태에 대한 offset과4방향 복도 offset을 prefab이나 id 구조로 가진다
    //     4. SceneLoad(게임 적으로는 던전 진입)에서 DungeonManager가 무조건 존재해야 하는 플레이어 시작 방을 기초로 놓는다
    //     5. 4 방향에 복도가 있을 가능성을 계산한다. 1개는 무조건 존재하며 2개가 존재할 가능성은 66%, 3개가 존재할 가능성은 50%, 4개가 존재할 가능성은 33%이다. 
    //     6. 복도의 끝에 방을 놓는다.
    //     7. 방에서 다시 나머지 3 방향에 복도가 있을 가능성을 계산한다.
    //        추가 복도가 1개 존재할 가능성은 80%, 2개 존재할 가능성은 60% 3개 존재할 가능성은 40%, 복도가 없이 그 방이 끝일 가능성은 20%이다. 
    //     8. 던전 어딘가에는 보스룸이 존재하며 보스룸까지는 무조건 도달 가능한 구조여야 한다.
    public Tilemap ground;
    public Tilemap walls;
    public TileBase groundTile;
    public TileBase wallTile;
    

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

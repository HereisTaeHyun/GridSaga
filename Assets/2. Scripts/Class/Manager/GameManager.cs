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
}

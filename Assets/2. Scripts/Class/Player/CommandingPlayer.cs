using UnityEngine;
using System.Collections.Generic;

public class CommandingPlayer : MonoBehaviour
{
    private int userId;
    private string userName;
    [SerializeField] private List<GameObject> party;
    [SerializeField] private GameObject board;

    void Start()
    {
        var formation = board.transform.Find("LeftFormation");

        for (int i = 0; i < party.Count; i++)
        {
            var unit = party[i];
            var unitPosition = formation.GetChild(i);

            var instance = Instantiate(unit, unitPosition);
            instance.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
        }
    }
}

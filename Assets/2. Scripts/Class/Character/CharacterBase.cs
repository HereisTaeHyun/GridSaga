using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private CharacterSO characterData;
    public CharacterSO CharacterData => characterData;
}

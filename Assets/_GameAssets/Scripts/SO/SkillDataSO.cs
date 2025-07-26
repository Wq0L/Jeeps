using UnityEngine;


[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private Transform _skillPrefab;


    public Transform SkillPrefab => _skillPrefab;
}

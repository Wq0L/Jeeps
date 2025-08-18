
using UnityEngine;


[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private Transform _skillPrefab;
    [SerializeField] private Vector3 _skillOffset;
    [SerializeField] private int _spawnAmountOrTimer;
    [SerializeField] private bool _shouldBeAttacdhedToParent;
    [SerializeField] private int _respawnTimer;
    [SerializeField] private int _damageAmount;


    public Transform SkillPrefab => _skillPrefab;
    public Vector3 SkillOffset => _skillOffset;
    public int SpawnAmountOrTimer => _spawnAmountOrTimer;
    public bool ShouldBeAttacdhedToParent => _shouldBeAttacdhedToParent;
    public int RespawnTimer => _respawnTimer;
    public int DamageAmount => _damageAmount;
}

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillManager : NetworkBehaviour
{
    [SerializeField] private MysteryBoxSkillsSO[] _mysteryBoxSkills;

    private Dictionary<SkillType, MysteryBoxSkillsSO> _skillDictionary;


    void Awake()
    {
        _skillDictionary = new Dictionary<SkillType, MysteryBoxSkillsSO>();

        foreach(MysteryBoxSkillsSO skill in _mysteryBoxSkills)
        {
           _skillDictionary[skill.SkillType] = skill;
        }
    }
}   

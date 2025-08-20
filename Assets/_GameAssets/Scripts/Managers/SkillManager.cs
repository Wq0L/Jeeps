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

        foreach (MysteryBoxSkillsSO skill in _mysteryBoxSkills)
        {
            _skillDictionary[skill.SkillType] = skill;
        }
    }

    private void Spawn(SkillTransformDataSerializable skillTransformDataSerializable,
    ulong spawnerClientId, MysteryBoxSkillsSO skillData)
    {
        if (IsServer)
        {
            Transform skillInstance = Instantiate(skillData.SkillData.SkillPrefab);
            skillInstance.SetPositionAndRotation(skillTransformDataSerializable.Position, skillTransformDataSerializable.Rotation);
            var networkObject = skillInstance.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(spawnerClientId);

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(spawnerClientId, out var client))
            {
                if (skillData.SkillType != SkillType.Rocket)
                {
                    networkObject.TrySetParent(client.PlayerObject);
                }
                else
                {
                    //roket özel
                }

                if (skillData.SkillData.ShouldBeAttacdhedToParent)
                {
                    networkObject.transform.localPosition = Vector3.zero;
                }

                PositionDataSerializable positionDataSerializable = new PositionDataSerializable(
                    skillInstance.transform.localPosition + skillData.SkillData.SkillOffset);
                UpdateSkillPositionRpc(networkObject.NetworkObjectId, positionDataSerializable);

                //devam edecek
            }
        }

    }
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateSkillPositionRpc(ulong objectId, PositionDataSerializable positionDataSerializable)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject networkObject))
        {
            networkObject.transform.localPosition = positionDataSerializable.Position;
        }
    }
}   

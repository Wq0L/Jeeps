using UnityEngine;

public interface ICollectables
{
   void Collect(PlayerSkillController playerSkillController);

   void CollectRpc();
}

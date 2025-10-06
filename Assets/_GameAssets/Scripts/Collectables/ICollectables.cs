using UnityEngine;

public interface ICollectables
{
   void Collect(PlayerSkillController playerSkillController, CameraSake cameraShake);

   void CollectRpc();
}

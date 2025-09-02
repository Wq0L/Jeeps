

public interface IDamageble
{
    void Damage(PlayerVehicleController playerVehicleController);
    ulong GetKillerClientId();
    int GetRespawnTimer();
}

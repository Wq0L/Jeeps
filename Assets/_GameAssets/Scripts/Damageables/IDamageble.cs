

public interface IDamageble
{
    void Damage(PlayerVehicleController playerVehicleController, string playerName);
    ulong GetKillerClientId();
    int GetRespawnTimer();
    int GetDamageAmount();
    string GetKillerName();
}

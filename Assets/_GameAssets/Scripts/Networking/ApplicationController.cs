using Cysharp.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientSingletonPrefab;
    [SerializeField] private HostSingleton _hostSingletonPrefab;
    

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);

    }
    private async UniTask LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

            Debug.Log("Launching in Dedicated Server Mode");
        }
        else
        {
            HostSingleton hostSingletonInstance = Instantiate(_hostSingletonPrefab);
            hostSingletonInstance.CreateHost();

            Debug.Log("Launching in Client Mode");
            ClientSingleton clientSingletonInstance = Instantiate(_clientSingletonPrefab);
            bool isAuthenticated = await clientSingletonInstance.CreateClient();

            if (isAuthenticated)
            {
                clientSingletonInstance.ClientGameManager.GoToMainMenu();
            }
        }
    }
}
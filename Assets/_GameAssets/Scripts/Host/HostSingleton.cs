using UnityEngine;

public class HostSingleton : MonoBehaviour
{
   private static HostSingleton instance;
    public HostGameManager HostGameManager{ get; private set; }
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            instance = FindAnyObjectByType<HostSingleton>();

            if (instance == null)
            {
                Debug.LogError("No instance of hostsingleton found in the scene.");
                return null;
            }
            return instance;

        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        HostGameManager = new HostGameManager();
    }
}

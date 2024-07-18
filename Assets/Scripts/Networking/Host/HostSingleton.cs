
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton _instance;
    public HostGameManager GameManager { get; private set; }

    public static HostSingleton Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = FindObjectOfType<HostSingleton>();
            if (_instance == null) 
            {
                Debug.LogError("There is no any host singleton in the scene.");
                return null;
            }

            return _instance;
        }
    }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

}

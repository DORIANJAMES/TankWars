using UnityEngine;
using System.Threading.Tasks;


public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton _instance;
    public ClientGameManager GameManager { get; private set; }
    
    public static ClientSingleton Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = FindObjectOfType<ClientSingleton>();
            
            if (_instance == null)
            {
                Debug.LogError("There is no any client singleton in the scene.");
                return null;
            }

            return _instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync();
    }
   
   
}

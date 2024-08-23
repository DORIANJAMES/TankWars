using UnityEngine;

public class MiniMap : MonoBehaviour
{

    private void Update()
    {
        switch (Input.deviceOrientation)
        {
            case DeviceOrientation.LandscapeLeft:
                gameObject.transform.position = new Vector3(-60, -40, 0);
                Debug.Log("Cihaz sola döndü");
                break;
            case DeviceOrientation.LandscapeRight:
                gameObject.transform.position = new Vector3(-40, -40, 0);
                Debug.Log("Cihaz Sağa Döndü");
                break;
            default:
                break;
        }
    }
}

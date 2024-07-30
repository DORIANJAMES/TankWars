using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class InGameCameraZoom : MonoBehaviour
{

    [SerializeField] private float targetZoom;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float initialZoom;
    [SerializeField] private bool isZoomedIn;
    [SerializeField] private TMP_Text sessionCode;
    

    private void Start()
    {
        sessionCode.text = PlayerPrefs.GetString("JoinCode", string.Empty);
        Camera.main.orthographicSize = initialZoom;
    }

    public void ToggleZoom()
    {
        if (isZoomedIn)
        {
            StartCoroutine(ZoomCoroutine(initialZoom));
        }
        else
        {
            StartCoroutine(ZoomCoroutine(targetZoom));
        }
        isZoomedIn = !isZoomedIn;
    }

    private IEnumerator ZoomCoroutine(float target)
    {
        float startZoom = Camera.main.orthographicSize;
        float t = 0f;
        while (t < 1f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(startZoom, target, t);
            t += Time.deltaTime * zoomSpeed;
            yield return null;
        }
        Camera.main.orthographicSize = target;
    }
}

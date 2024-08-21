using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;


public class InGameCameraZoom : NetworkBehaviour
{
    [SerializeField] private float targetZoom;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float initialZoom;
    [SerializeField] private bool isZoomedIn;
    [SerializeField] private TMP_Text sessionCode;
    [SerializeField] private Button changeButton;
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;


    public override void OnNetworkSpawn()
    {
        virtualCamera = GameObject.FindGameObjectWithTag("PlayerVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sessionCode.text = PlayerPrefs.GetString("JoinCode",string.Empty);
    }

    public void OnButtonPressed()
    {
        if (IsServer && IsLocalPlayer)
        {
            InnerToggleZoom(targetZoom);
        } else if (IsClient && IsLocalPlayer)
        {
            InnerToggleZoomServerRPC(targetZoom);
        }
    }

    private void InnerToggleZoom(float target)
    {
        StartCoroutine(ZoomToggleLerper(target));
    }

    [ServerRpc]
    private void InnerToggleZoomServerRPC(float target)
    {
        InnerToggleZoom(target);
        
    }

    private IEnumerator ZoomToggleLerper(float target)
    {
        var startZoom = virtualCamera.m_Lens.OrthographicSize;
        var t = 0f;
        while (t < 1f)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, t);
            t += Time.deltaTime * zoomSpeed;
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = target;
    }
}

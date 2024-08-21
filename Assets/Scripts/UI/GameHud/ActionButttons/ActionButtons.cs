using System.Collections;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActionButtons : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject leaderBoard;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private CinemachineVirtualCamera cmVirtualCam;
    [SerializeField] private GameObject player;

    [Header("Settings")]
    [SerializeField] private float targetZoom;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float initialZoom;
    private Color _leaderBoardColor;
    private Color _miniMapColor;
    private float _targetOpacity;
    private float _initialOpacityOfLeaderBoard;
    private float _initialOpacityOfMiniMap;
    private bool _toggle = true;
    //private bool _firstClick = false;
    private bool _isZoomed;
 

    private void OnEnable()
    {
        cmVirtualCam = player.GetComponentInChildren<CinemachineVirtualCamera>();
        initialZoom = cmVirtualCam.m_Lens.OrthographicSize;
        _leaderBoardColor = leaderBoard.GetComponent<Image>().color;
        _miniMapColor = miniMap.GetComponent<Image>().color;
        _initialOpacityOfLeaderBoard = _leaderBoardColor.a;
        _initialOpacityOfMiniMap = _miniMapColor.a;
        Debug.Log(cmVirtualCam.m_Lens.Orthographic); // True dönüyor.
    }

    public void ToggleHud()
    {
        leaderBoard.SetActive(_toggle);
        miniMap.SetActive(!_toggle);
        _toggle = !_toggle;
    }

    public void ToggleZoom()
    {
        if (!IsOwner) { return; }
        if (cmVirtualCam != null)
        {
            if (!_isZoomed)
            {
                StartCoroutine(Zooming(targetZoom));
                _isZoomed = true;
            }
            else
            {
                StartCoroutine(Zooming(initialZoom));
                _isZoomed = false;
            }
        }
        else
        {
            Debug.Log("Cinemachine Virtual Cam reference is missing.");
        }
    }

    private IEnumerator Zooming(float target)
    {
        float initialZoom = cmVirtualCam.m_Lens.OrthographicSize;
        var t = 0f;
        while (t < 1f)
        {
            cmVirtualCam.m_Lens.OrthographicSize = Mathf.Lerp(initialZoom, target, t);
            Camera.main.orthographicSize = cmVirtualCam.m_Lens.OrthographicSize;
            t += Time.deltaTime * zoomSpeed;
            Debug.Log("Zooming in progress: " + cmVirtualCam.m_Lens.OrthographicSize);
            yield return null;
        }
        cmVirtualCam.m_Lens.OrthographicSize = target;
        Camera.main.orthographicSize = cmVirtualCam.m_Lens.OrthographicSize;
        Debug.Log("Zooming completed: " + cmVirtualCam.m_Lens.OrthographicSize);
    }
    
}

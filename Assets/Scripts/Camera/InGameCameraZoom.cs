using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCameraZoom : MonoBehaviour
{

    [SerializeField] private float targetZoom;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float initialZoom;
    [SerializeField] private bool isZoomedIn;

    private void Start()
    {
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

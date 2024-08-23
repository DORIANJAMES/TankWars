using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class JoystickVirtualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform moveCenter;
    [SerializeField] private RectTransform moveKnob;
    [SerializeField] private RectTransform aimCenter;
    [SerializeField] private RectTransform aimKnob;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Settings")] 
    [SerializeField] private float range;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private Vector2 aimDirection;
    [SerializeField] private bool aimActive;
    public bool moveActive { get; private set; }

    public event Action<Vector2> MoveJoystick = delegate {};
    public event Action AimJoystick = delegate {};
    public event Action DoubleTapFire = delegate {}; // Çift tıklama için Action

    private float lastTapTime = 0f; // Son tıklamanın zamanı
    private const float doubleTapThreshold = 0.3f; // Çift tıklama eşiği

    private void OnEnable()
    {
        ShowJoystick(false, false);
        moveActive = false;
        aimActive = false;
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        Vector2 pos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastTapTime < doubleTapThreshold)
            {
                // Çift tıklama algılandı
                DoubleTapFire?.Invoke();
            }
            lastTapTime = Time.time;

            // Ekranın sol yarısına tıklama durumu (hareket joystick)
            if (pos.x < Screen.width / 2)
            {
                if (!IsPointerOverUIElement())
                {
                    ShowJoystick(true, false);
                    moveActive = true;
                    moveCenter.position = pos;
                    moveKnob.position = moveCenter.position; // Knob'ı center ile aynı konuma getir
                    this.MoveJoystick += playerMovement.HandleMove;
                }
            }
            // Ekranın sağ yarısına tıklama durumu (aim joystick)
            else if (pos.x >= Screen.width / 2)
            {
                if (!IsPointerOverUIElement())
                {
                    ShowJoystick(false, true);
                    aimActive = true;
                    aimCenter.position = pos;
                    aimKnob.position = aimCenter.position; // Knob'ı center ile aynı konuma getir
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (moveActive)
            {
                moveKnob.position = pos;
                moveKnob.position = moveCenter.position + Vector3.ClampMagnitude(moveKnob.position - moveCenter.position, moveCenter.sizeDelta.x * range);
                moveDirection = (moveKnob.position - moveCenter.position).normalized;
                MoveJoystick?.Invoke(moveDirection);
            }
            else if (aimActive)
            {
                aimKnob.position = pos;
                aimKnob.position = aimCenter.position + Vector3.ClampMagnitude(aimKnob.position - aimCenter.position, aimCenter.sizeDelta.x * range);
                aimDirection = (aimKnob.position - aimCenter.position).normalized;
                AimJoystick?.Invoke();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            this.MoveJoystick -= playerMovement.HandleMove;
            MoveJoystick?.Invoke(Vector2.zero);
            moveActive = false;
            aimActive = false;
            ShowJoystick(false, false);
            moveDirection = Vector2.zero;
            aimDirection = Vector2.zero;
        }
        else
        {
            moveActive = false;
            aimActive = false;
            ShowJoystick(false, false);
            moveDirection = Vector2.zero;
            aimDirection = Vector2.zero;
        }
    }

    private void ShowJoystick(bool showMove, bool showAim)
    {
        moveCenter.gameObject.SetActive(showMove);
        moveKnob.gameObject.SetActive(showMove);
        aimCenter.gameObject.SetActive(showAim);
        aimKnob.gameObject.SetActive(showAim);
    }

    private bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    } 
}

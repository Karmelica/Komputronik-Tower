using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Linq;

public class DeviceRecognitionScript : MonoBehaviour
{
    [Header("Mobile Controllers")]
    [SerializeField] private GameObject mobileMoveController;
    [SerializeField] private GameObject mobileJumpController;

    [Header("Initialization Text")]
    [SerializeField] private GameObject startText;
    
    public bool gameStarted = false;
    private bool usingTouch;
    
    private void Start()
    {
        mobileMoveController.SetActive(false);
        mobileJumpController.SetActive(false);
        //startText.SetActive(true);
        Time.timeScale = 0;
    }

    private void Update()
    {
        bool touch = Touchscreen.current?.touches.Any(t => t.isInProgress) ?? false;
        bool keyboard = Keyboard.current?.anyKey.wasPressedThisFrame ?? false;
        bool mouse = Mouse.current?.allControls.Any(c => c is ButtonControl b && b.wasPressedThisFrame) ?? false;
        bool pad = Gamepad.current?.allControls.Any(c => c is ButtonControl b && b.wasPressedThisFrame) ?? false;

        if (keyboard || mouse || pad)
        {
            ChangeInputDevice(false);
        }
        else if (touch)
        {
            ChangeInputDevice(true);
        }
    }
    
    private void ChangeInputDevice(bool isTouch)
    {
        mobileMoveController.SetActive(isTouch);
        mobileJumpController.SetActive(isTouch);

        //Debug.Log(isTouch ? "Switched to TOUCH" : "Switched to KEY/MOUSE/CONTROLLER");
    }
}

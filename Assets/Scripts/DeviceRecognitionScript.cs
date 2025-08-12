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

    private InputSystemActions inputActions;
    public bool gameStarted = false;
    private bool usingTouch;

    private void Awake()
    {
        inputActions = new InputSystemActions();
        
        inputActions.Player.Touch.performed += ctx =>
        {
            if (!gameStarted)
            {
                usingTouch = true;
                StartGame();
            }
            else if (!usingTouch)
            {
                usingTouch = true;
                ChangeInputDevice(true);
            }
        };
    }

    private void Start()
    {
        mobileMoveController.SetActive(false);
        mobileJumpController.SetActive(false);
        startText.SetActive(true);
        Time.timeScale = 0;
    }
    
    private void Update()
    {
        if (!gameStarted)
        {
            CheckForStartInput();
        }
        
        if (usingTouch && IsRealGamepad(Gamepad.current) && Gamepad.current.allControls.Any(c => c is ButtonControl b && b.wasPressedThisFrame))
        {
            usingTouch = false;
            ChangeInputDevice(false);
        }
        else if (usingTouch && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            usingTouch = false;
            ChangeInputDevice(false);
        }
    }
    
    private void CheckForStartInput()
    {
        // Detect keyboard/mouse press
        if (!usingTouch && (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame))
        {
            Debug.Log("Keyboard and Mouse was pressed");
            usingTouch = false;
            StartGame();
        }
        
        // Detect controller button press
        if (!usingTouch && IsRealGamepad(Gamepad.current) &&
            Gamepad.current.allControls.Any(c => c is ButtonControl b && b.wasPressedThisFrame))
        {
            Debug.Log("Gamepad");
            usingTouch = false;
            StartGame();
        }  
    }
    
    private void ChangeInputDevice(bool isTouch)
    {
        mobileMoveController.SetActive(isTouch);
        mobileJumpController.SetActive(isTouch);

        Debug.Log(isTouch ? "Switched to TOUCH" : "Switched to KEY/MOUSE/CONTROLLER");
    }
    
    private bool IsRealGamepad(Gamepad pad)
    {
        if (pad == null) return false;
        return !string.IsNullOrEmpty(pad.device.description.product) &&
               !pad.device.description.product.ToLower().Contains("virtual");
    }
    
    private void StartGame()
    {
        startText.SetActive(false);
        gameStarted = true;
        ChangeInputDevice(usingTouch);
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}

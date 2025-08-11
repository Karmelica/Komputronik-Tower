using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceRecognitionScript : MonoBehaviour
{
    [SerializeField] private Color pcColor;
    [SerializeField] private Color mobileColor;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private InputSystemActions inputActions;
    private bool gameStarted = false;

    private void Awake()
    {
        inputActions = new InputSystemActions();

        // Subscribe to touch
        inputActions.Player.Touch.performed += ctx =>
        {
            if (!gameStarted)
            {
                StartGame(isTouch: true);
            }
        };

        // Subscribe to key/mouse
        inputActions.Player.AnyKey.performed += ctx =>
        {
            if (!gameStarted)
            {
                StartGame(isTouch: false);
            }
        };
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        Debug.Log("Tap screen or press any input to start");
        Time.timeScale = 0;
    }

    private void StartGame(bool isTouch)
    {
        gameStarted = true;
        Time.timeScale = 1;

        if (isTouch)
        {
            spriteRenderer.color = mobileColor;
            Debug.Log("Started via TOUCH");
        }
        else
        {
            spriteRenderer.color = pcColor;
            Debug.Log("Started via KEY/MOUSE");
        }
    }
}
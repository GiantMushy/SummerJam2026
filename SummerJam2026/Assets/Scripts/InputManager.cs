using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public PlayerCharacterManager player;
    public static InputManager instance;
    private Keyboard keyboard;

    public float horizontalInput;
    public float verticalInput;

    public bool IsBoosting => keyboard.leftShiftKey.isPressed;
    private bool pressingUp => keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed;
    private bool pressingDown => keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed;
    private bool pressingLeft => keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
    private bool pressingRight => keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;

    private void Awake()
    {
        // set singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        keyboard = Keyboard.current;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        horizontalInput = 0f;
        verticalInput = 0f;

        if (pressingUp)     verticalInput += 1f;
        if (pressingDown)   verticalInput -= 1f;
        if (pressingRight)  horizontalInput += 1f;
        if (pressingLeft)   horizontalInput -= 1f;

        // Normalize the input vector to prevent faster diagonal movement
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
        if (inputVector.magnitude > 1f)
        {
            inputVector.Normalize();
            horizontalInput = inputVector.x;
            verticalInput = inputVector.y;
        }
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        if (next.buildIndex == 0) // TODO: FIX MAGIC NUMBER
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled = false;
        }
    }
}
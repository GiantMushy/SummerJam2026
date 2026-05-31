using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public PlayerCharacterManager player;
    public static InputManager instance;

    public float horizontalInput;
    public float verticalInput;

    PlayerControls playerControls;

    [SerializeField] Vector2 movementInput; // assigned by the input system
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
        SceneManager.activeSceneChanged += OnSceneChanged;
    }


    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();

        }

        playerControls.Enable();
    }


    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = Mathf.Round(movementInput.y);
        horizontalInput = Mathf.Round(movementInput.x);
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
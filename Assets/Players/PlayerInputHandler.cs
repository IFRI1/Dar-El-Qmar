using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private DarElQmarControls controls;

    public bool ReactPressed { get; private set; }

    private void Awake()
    {
        controls = new DarElQmarControls();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Gameplay.React.performed += OnReact;
    }

    private void OnDisable()
    {
        controls.Gameplay.React.performed -= OnReact;

        controls.Disable();
    }

    private void OnReact(InputAction.CallbackContext context)
    {
        ReactPressed = true;

        Debug.Log($"{gameObject.name} reacted!");
    }

    public void ResetInput()
    {
        ReactPressed = false;
    }
}
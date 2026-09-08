using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ReactionInput
{
    FaceSouth,
    FaceEast,
    FaceWest,
    FaceNorth,

    L1,
    L2,
    L3,

    R1,
    R2,
    R3,

    DPadUp,
    DPadDown,
    DPadLeft,
    DPadRight
}

public class PlayerInputHandler : MonoBehaviour
{
    private DarElQmarControls controls;

    [SerializeField] private Animator characterAnimator;

    [Header("Controller Assignment")]
    [Tooltip("0 = first controller, 1 = second controller, etc.")]
    [SerializeField] private int controllerIndex = 0;

    public event Action<PlayerInputHandler, ReactionInput> OnReactionInput;

    private InputDevice assignedDevice;

    private void Awake()
    {
        controls = new DarElQmarControls();
    }

    private void OnEnable()
    {
        AssignController();

        controls.Enable();

        controls.Gameplay.FaceSouth.performed += OnFaceSouth;
        controls.Gameplay.FaceEast.performed += OnFaceEast;
        controls.Gameplay.FaceWest.performed += OnFaceWest;
        controls.Gameplay.FaceNorth.performed += OnFaceNorth;

        controls.Gameplay.L1.performed += OnL1;
        controls.Gameplay.L2.performed += OnL2;
        controls.Gameplay.L3.performed += OnL3;

        controls.Gameplay.R1.performed += OnR1;
        controls.Gameplay.R2.performed += OnR2;
        controls.Gameplay.R3.performed += OnR3;

        controls.Gameplay.DPadUp.performed += OnDPadUp;
        controls.Gameplay.DPadDown.performed += OnDPadDown;
        controls.Gameplay.DPadLeft.performed += OnDPadLeft;
        controls.Gameplay.DPadRight.performed += OnDPadRight;
    }

    private void OnDisable()
    {
        controls.Gameplay.FaceSouth.performed -= OnFaceSouth;
        controls.Gameplay.FaceEast.performed -= OnFaceEast;
        controls.Gameplay.FaceWest.performed -= OnFaceWest;
        controls.Gameplay.FaceNorth.performed -= OnFaceNorth;

        controls.Gameplay.L1.performed -= OnL1;
        controls.Gameplay.L2.performed -= OnL2;
        controls.Gameplay.L3.performed -= OnL3;

        controls.Gameplay.R1.performed -= OnR1;
        controls.Gameplay.R2.performed -= OnR2;
        controls.Gameplay.R3.performed -= OnR3;

        controls.Gameplay.DPadUp.performed -= OnDPadUp;
        controls.Gameplay.DPadDown.performed -= OnDPadDown;
        controls.Gameplay.DPadLeft.performed -= OnDPadLeft;
        controls.Gameplay.DPadRight.performed -= OnDPadRight;

        controls.Disable();
    }

    private void AssignController()
    {
        var gamepads = Gamepad.all;

        if (controllerIndex < gamepads.Count)
        {
            assignedDevice = gamepads[controllerIndex];

            Debug.Log($"{gameObject.name} assigned to {assignedDevice.displayName}");
        }
        else
        {
            assignedDevice = null;

            Debug.LogWarning(
                $"{gameObject.name} could not find controller {controllerIndex + 1}."
            );
        }
    }

    private bool IsAssignedController(InputAction.CallbackContext context)
    {
        return assignedDevice != null &&
               context.control.device == assignedDevice;
    }

    private void OnFaceSouth(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.FaceSouth);
    }

    private void OnFaceEast(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.FaceEast);
    }

    private void OnFaceWest(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.FaceWest);
    }

    private void OnFaceNorth(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.FaceNorth);
    }

    private void OnL1(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.L1);
    }

    private void OnL2(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.L2);
    }

    private void OnL3(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.L3);
    }

    private void OnR1(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.R1);
    }

    private void OnR2(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.R2);
    }

    private void OnR3(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.R3);
    }

    private void OnDPadUp(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.DPadUp);
    }

    private void OnDPadDown(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.DPadDown);
    }

    private void OnDPadLeft(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.DPadLeft);
    }

    private void OnDPadRight(InputAction.CallbackContext context)
    {
        if (IsAssignedController(context))
            ReportInput(ReactionInput.DPadRight);
    }

    private void ReportInput(ReactionInput input)
    {
        Debug.Log($"{gameObject.name} pressed {input}");

        OnReactionInput?.Invoke(this, input);
    }

    public void ResetInput()
    {
        //Intentionally empty, ReactionManager uses this to reset the player's reaction state.
    }

    public void PlayLoseAnimation()
    {
        if (characterAnimator == null)
        {
            Debug.LogError($"{gameObject.name}: Animator is missing.");
            return;
        }

        Debug.Log($"{gameObject.name} -> PLAYING LOSE ANIMATION");
        characterAnimator.ResetTrigger("Lose");
        characterAnimator.SetTrigger("Lose");
    }
}
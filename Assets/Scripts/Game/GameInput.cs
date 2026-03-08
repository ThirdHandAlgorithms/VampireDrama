namespace VampireDrama
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class GameInput
    {
        private static GameInput instance;

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction interactAction;
        private InputAction cycleAbilityAction;
        private InputAction useAbilityAction;
        private InputAction confirmAction;

        private GameInput()
        {
            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            moveAction.AddBinding("<Gamepad>/leftStick");
            moveAction.AddBinding("<Gamepad>/dpad");

            jumpAction = new InputAction("Jump", InputActionType.Button);
            jumpAction.AddBinding("<Keyboard>/leftCtrl");
            jumpAction.AddBinding("<Mouse>/leftButton");
            jumpAction.AddBinding("<Gamepad>/buttonSouth");

            interactAction = new InputAction("Interact", InputActionType.Button);
            interactAction.AddBinding("<Keyboard>/leftAlt");
            interactAction.AddBinding("<Mouse>/rightButton");
            interactAction.AddBinding("<Gamepad>/buttonEast");

            cycleAbilityAction = new InputAction("CycleAbility", InputActionType.Button);
            cycleAbilityAction.AddBinding("<Keyboard>/leftShift");
            cycleAbilityAction.AddBinding("<Mouse>/middleButton");
            cycleAbilityAction.AddBinding("<Gamepad>/buttonWest");

            useAbilityAction = new InputAction("UseAbility", InputActionType.Button);
            useAbilityAction.AddBinding("<Keyboard>/space");
            useAbilityAction.AddBinding("<Gamepad>/buttonNorth");

            confirmAction = new InputAction("Confirm", InputActionType.Button);
            confirmAction.AddBinding("<Keyboard>/leftCtrl");
            confirmAction.AddBinding("<Mouse>/leftButton");
            confirmAction.AddBinding("<Gamepad>/buttonSouth");

            moveAction.Enable();
            jumpAction.Enable();
            interactAction.Enable();
            cycleAbilityAction.Enable();
            useAbilityAction.Enable();
            confirmAction.Enable();
        }

        public static GameInput GetInstance()
        {
            if (instance == null)
            {
                instance = new GameInput();
            }
            return instance;
        }

        public Vector2 GetMoveInput()
        {
            return moveAction.ReadValue<Vector2>();
        }

        public bool JumpPressed()
        {
            return jumpAction.WasPressedThisFrame();
        }

        public bool InteractPressed()
        {
            return interactAction.WasPressedThisFrame();
        }

        public bool CycleAbilityPressed()
        {
            return cycleAbilityAction.WasPressedThisFrame();
        }

        public bool UseAbilityPressed()
        {
            return useAbilityAction.WasPressedThisFrame();
        }

        public bool ConfirmPressed()
        {
            return confirmAction.WasPressedThisFrame();
        }
    }
}

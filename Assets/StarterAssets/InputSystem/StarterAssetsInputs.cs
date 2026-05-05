using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        [SerializeField] private Vector2 move;
        [SerializeField] private Vector2 look;
        [SerializeField] private bool jump;
        [SerializeField] private bool sprint;

        [Header("Movement Settings")]
        [SerializeField] private bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
        [Header("Mouse Cursor Settings")]
        [SerializeField] private bool cursorLocked = true;
        [SerializeField] private bool cursorInputForLook = true;
#endif

        private void Awake()
        {
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("[INPUTS] No se encontró PlayerInput en el GameObject!");
                return;
            }

            Debug.Log("[INPUTS] Suscribiendo eventos manualmente...");

            var actions = playerInput.actions;

            actions["Move"].performed += ctx =>
            {
                MoveInput(ctx.ReadValue<Vector2>());
                Debug.Log("OnMove: " + move);
            };
            actions["Move"].canceled += ctx =>
            {
                MoveInput(Vector2.zero);
            };

            actions["Look"].performed += ctx =>
            {
                if (cursorInputForLook)
                    LookInput(ctx.ReadValue<Vector2>());
            };

            actions["Jump"].performed += ctx => JumpInput(true);
            actions["Jump"].canceled += ctx => JumpInput(false);

            actions["Sprint"].performed += ctx => SprintInput(ctx.ReadValue<float>() == 1);
            actions["Sprint"].canceled += ctx => SprintInput(false);

            Debug.Log("[INPUTS] Eventos suscritos OK");
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public Vector2 GetMove()
        {
            return move;
        }

        public Vector2 GetLook()
        {
            return look;
        }

        public bool IsJumping()
        {
            return jump;
        }

        public bool IsSprinting()
        {
            return sprint;
        }

        public bool IsAnalog()
        {
            return analogMovement;
        }

#if !UNITY_IOS || !UNITY_ANDROID

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

#endif

    }

}

using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    //弄个事件处理，这个事件处理就负责调用有关交互的相关函数
    public event EventHandler onInteractAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        //启用输入的话，就调用事件相关的函数
        playerInputActions.Player.Interact.performed += Interact_Performed;
    }

    //负责调用有关交互的相关函数
    private void Interact_Performed(InputAction.CallbackContext context) {
        onInteractAction?.Invoke(this, EventArgs.Empty);
    }


    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}

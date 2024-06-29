using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    //Ū���¼���������¼�����͸�������йؽ�������غ���
    public event EventHandler onInteractAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        //��������Ļ����͵����¼���صĺ���
        playerInputActions.Player.Interact.performed += Interact_Performed;
    }

    //��������йؽ�������غ���
    private void Interact_Performed(InputAction.CallbackContext context) {
        onInteractAction?.Invoke(this, EventArgs.Empty);
    }


    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}

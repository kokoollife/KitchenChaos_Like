using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    private PlayerInputActions playerInputActions;

    //����Ҫ�������ǵ��������ã�����������֮ǰ������Ū���ַ����������Ѷ�������json
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    //�����ǵİ�ť����ö��
    public enum Binding {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
    }

    private void Awake() {
        Instance = this;
        playerInputActions = new PlayerInputActions();

        //����Ҫע��������ǵ�json����õ���������ʱ��Ҫ��enableǰִ��
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_Performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy() {
        playerInputActions.Player.Interact.performed -= Interact_Performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext context) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(InputAction.CallbackContext context) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_Performed(InputAction.CallbackContext context) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }


    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    //�������ǵ�����ö�٣�Ȼ�󷵻ض�Ӧ��������ϵͳ�����İ󶨰����ַ���
    //������ϵͳ���±�����ǣ����ǰ�һ��Ŀ¼���֣�ÿ������Ŀ¼���ж���±���϶���
    //�������ǲ�Ҫ������Щ�۵��ʹ���Ϊ�±�᲻һ����ʵ�����Ǵ������°�˳�������±��
    //�������ǵ�move��move���۵����������Ұ�������
    //����move�������bindings[0]
    //����bindings[1]�������ơ�
    public string GetBindingText(Binding binding) {
        switch (binding) {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    //�ṩ�����޸İ����󶨵ĺ���
    public void RebindBinding(Binding binding, Action onActionRebound) {
        //Ҫע�⻻�󰴼���������Ҫ��ʱͣס����ϵͳ�����á�
        playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;
        switch (binding) {
            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback => {
            //���������ַ�
            //Debug.Log(callback.action.bindings[1].path);
            //���Ը��Ǻ�İ��������ַ�
            //Debug.Log(callback.action.bindings[1].overridePath);
            callback.Dispose();//����Ҫ�����ڴ����
            playerInputActions.Player.Enable();
            //ͨ��ί�У��������ǾͲ��ð�OptionsUI������߼��������õ������ˣ����ǿ�����ί�а���ס���˵ĺ�����Ȼ��ֱ��ʹ�á�
            onActionRebound();

            //��Ϊjson��ʽ
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }).Start();
    }
}

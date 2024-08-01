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

    //我们要保存我们的输入设置，所以我们像之前那样，弄个字符串常量，把东西存入json
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    //对我们的按钮操作枚举
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

        //我们要注意加载我们的json保存好的输入设置时，要在enable前执行
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

    //输入我们的输入枚举，然后返回对应的新输入系统的来的绑定按键字符串
    //新输入系统的下标规律是，我们按一个目录区分，每个最终目录下有多个下标组合而成
    //其中我们不要看到有些折叠就错认为下标会不一样，实际上是从上往下按顺序排列下标的
    //比如我们的move，move下折叠是上下左右按键操作
    //所以move这个父是bindings[0]
    //上是bindings[1]依此类推。
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

    //提供可以修改按键绑定的函数
    public void RebindBinding(Binding binding, Action onActionRebound) {
        //要注意换绑按键，我们需要暂时停住输入系统的启用。
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
            //测试输入字符
            //Debug.Log(callback.action.bindings[1].path);
            //测试覆盖后的按键输入字符
            //Debug.Log(callback.action.bindings[1].overridePath);
            callback.Dispose();//换绑要避免内存错误
            playerInputActions.Player.Enable();
            //通过委托，这样我们就不用把OptionsUI里面的逻辑函数调用到这里了，我们可以用委托包裹住别人的函数，然后直接使用。
            onActionRebound();

            //存为json格式
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }).Start();
    }
}

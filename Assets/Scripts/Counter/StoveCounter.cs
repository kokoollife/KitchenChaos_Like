using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    //private State state;
    //枚举是可以序列化的
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    //private float fryingTimer;
    //计时器应该交给服务端处理，而不能让客户端与服务端共有
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    //private float burningTimer;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);

    //然后就没必要在这里初始化了，因为网络同步变量已经做了默认初始化
    //private void Start() {
    //    state = State.Idle;
    //}

    //重载网络生成
    public override void OnNetworkSpawn() {
        //利用联网提供的事件，获取判断数值的前后变化
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousState, State newState) {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
            state = state.Value,
        });

        if (state.Value == State.Burned || state.Value == State.Idle) {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = 0f,
            });
        }
    }

    //这种写法一般是涉及到fryingTimer和客户端必须要执行的操作
    private void FryingTimer_OnValueChanged(float previousValue,float newValue) {
        //fryingRecipeSO.fryingTimerMax为什么不用SO数据了，而是使用局部变量？
        //会不会就是因为SO有概率无法联网传递？
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = fryingTimer.Value / fryingTimerMax,
        });
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue) {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = burningTimer.Value / burningTimerMax,
        });
    }

    private void Update() {
        //杜绝客户端也执行下面的代码，因为涉及到状态切换/fryingTimer/burningTimer/销毁物品，这些属于同步变量，只能交给服务端处理
        //如果要穿插一些客户端的操作，只能通过clientrpc
        if (!IsServer) {
            return;
        }

        if (HasKitchenObject()) {
            switch (state.Value) {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax) {

                        //GetKitchenObject().DestroySelf();
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state.Value = State.Fried;
                        burningTimer.Value = 0f;
                        //在这里调用客户端处理烤制肉的操作，通过SO切换预制体
                        SetBurningRecipeSOClientRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );
                        //burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        //    state = state,
                        //});
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    //这段处理已经交给BurningTimer_OnValueChanged
                    //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    //    progressNormalized = burningTimer.Value / burningRecipeSO.burningTimerMax,
                    //});

                    if (burningTimer.Value > burningRecipeSO.burningTimerMax) {
                        //GetKitchenObject().DestroySelf();
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state.Value = State.Burned;

                        //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        //    state = state,
                        //});

                        //之所以注释这个初始化传递，是因为要告诉客户端也要做，要留意是空闲情况+烤焦的时候，重置初始化
                        //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        //    progressNormalized = 0f,
                        //});
                    }
                    break;
                case State.Burned:
                    break;
            }
            
        }
    }

    //改动原来的交互操作，保存我们的厨房物品，而不是跟player做唯一绑定
    //交互我们虽然里面也涉及到了客户端才能有的操作，但是不做IsServer的限制，所以只能通过ServerRpc来同步有关于状态相关的变量。
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    KitchenObject kitchenObject = player.GetKitchenObject();

                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                    );

                }
            }
            else {

            }
        }
        else {
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        //GetKitchenObject().DestroySelf();
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //state.Value = State.Idle;
                        SetStateIdleServerRpc();
                        //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        //    state = state,
                        //});

                        //因为已经交给联机事件处理了，所以这里交互的没必要重复交互给客户端
                        //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        //    progressNormalized = 0f,
                        //});
                    }
                }
            }
            else {
                GetKitchenObject().SetKitchenObjectParent(player);

                //state.Value = State.Idle;
                SetStateIdleServerRpc();

                //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                //    state = state,
                //});

                //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                //    progressNormalized = 0f,
                //});
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc() {
        state.Value = State.Idle;
    }

    //处理rpc逻辑
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex) {
        fryingTimer.Value = 0f;//在服务端中重置计时器，而不要交给客户端处理
        state.Value = State.Frying;//同样的，涉及到网络变量的修改，也只能交给服务端
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);

        //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
        //    state = state,
        //});

        //因为已经写了有关于计时器前后变化的联网事件，所以客户端这里没必要重复写了
        //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
        //    progressNormalized = fryingTimer.Value / fryingRecipeSO.fryingTimerMax,
        //});
    }
    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null) {
            return fryingRecipeSO.output;
        }
        else {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == inputKitchenObjectSO) {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
            if (burningRecipeSO.input == inputKitchenObjectSO) {
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried() {
        return state.Value == State.Fried;
    }
}

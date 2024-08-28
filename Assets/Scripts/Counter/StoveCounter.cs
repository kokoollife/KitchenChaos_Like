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
    //ö���ǿ������л���
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    //private float fryingTimer;
    //��ʱ��Ӧ�ý�������˴����������ÿͻ��������˹���
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    //private float burningTimer;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);

    //Ȼ���û��Ҫ�������ʼ���ˣ���Ϊ����ͬ�������Ѿ�����Ĭ�ϳ�ʼ��
    //private void Start() {
    //    state = State.Idle;
    //}

    //������������
    public override void OnNetworkSpawn() {
        //���������ṩ���¼�����ȡ�ж���ֵ��ǰ��仯
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

    //����д��һ�����漰��fryingTimer�Ϳͻ��˱���Ҫִ�еĲ���
    private void FryingTimer_OnValueChanged(float previousValue,float newValue) {
        //fryingRecipeSO.fryingTimerMaxΪʲô����SO�����ˣ�����ʹ�þֲ�������
        //�᲻�������ΪSO�и����޷��������ݣ�
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
        //�ž��ͻ���Ҳִ������Ĵ��룬��Ϊ�漰��״̬�л�/fryingTimer/burningTimer/������Ʒ����Щ����ͬ��������ֻ�ܽ�������˴���
        //���Ҫ����һЩ�ͻ��˵Ĳ�����ֻ��ͨ��clientrpc
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
                        //��������ÿͻ��˴�������Ĳ�����ͨ��SO�л�Ԥ����
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

                    //��δ����Ѿ�����BurningTimer_OnValueChanged
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

                        //֮����ע�������ʼ�����ݣ�����ΪҪ���߿ͻ���ҲҪ����Ҫ�����ǿ������+������ʱ�����ó�ʼ��
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

    //�Ķ�ԭ���Ľ����������������ǵĳ�����Ʒ�������Ǹ�player��Ψһ��
    //����������Ȼ����Ҳ�漰���˿ͻ��˲����еĲ��������ǲ���IsServer�����ƣ�����ֻ��ͨ��ServerRpc��ͬ���й���״̬��صı�����
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

                        //��Ϊ�Ѿ����������¼������ˣ��������ｻ����û��Ҫ�ظ��������ͻ���
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

    //����rpc�߼�
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex) {
        fryingTimer.Value = 0f;//�ڷ���������ü�ʱ��������Ҫ�����ͻ��˴���
        state.Value = State.Frying;//ͬ���ģ��漰������������޸ģ�Ҳֻ�ܽ��������
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);

        //OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
        //    state = state,
        //});

        //��Ϊ�Ѿ�д���й��ڼ�ʱ��ǰ��仯�������¼������Կͻ�������û��Ҫ�ظ�д��
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

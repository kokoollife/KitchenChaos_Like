using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler<OnPlateSpawnedPlateVisualEventArgs> OnPlateSpawned;
    //设定哪些是可以传入的参数
    public class OnPlateSpawnedPlateVisualEventArgs : EventArgs {
        public PlateVisualSO plateVisualSO;
    }

    [SerializeField] private PlateVisualSO plateVisualSO;

    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Awake() {
        // 从Resources文件夹中加载ScriptableObject
        plateVisualSO = Resources.Load<PlateVisualSO>("ScriptableObjects/PlateVisualSO");

        // 检查是否成功加载，只是对服务端有效果，客户端是不保证的，所以可写可不写，看自己喜欢
        if (plateVisualSO != null) {
            Debug.Log("Successfully loaded PlateVisualSO.");
        }
        else {
            Debug.LogError("Failed to load PlateVisualSO. Check the path and ensure the file exists in the Resources folder.");
        }
    }

    //利用rpc同步生成的逻辑
    private void Update() {
        if (!IsServer) {
            return;
        }

        if (!IsSpawned) {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0f;
            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc() {
        //
        //    GameObject plateVisualTF = Instantiate(plateVisualSO.plateVisualPrefab);
        //    NetworkObject plateVisualNetworkObject = plateVisualTF.GetComponent<NetworkObject>();
        //    plateVisualNetworkObject.Spawn(true);
        //
        
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc() {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, new OnPlateSpawnedPlateVisualEventArgs {
            plateVisualSO = plateVisualSO
        });
    }

    //参考交互的rpc同步方式，考虑减少的逻辑
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            if(platesSpawnedAmount > 0) {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}

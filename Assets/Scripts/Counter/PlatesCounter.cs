using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler<OnPlateSpawnedPlateVisualEventArgs> OnPlateSpawned;
    //�趨��Щ�ǿ��Դ���Ĳ���
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
        // ��Resources�ļ����м���ScriptableObject
        plateVisualSO = Resources.Load<PlateVisualSO>("ScriptableObjects/PlateVisualSO");

        // ����Ƿ�ɹ����أ�ֻ�ǶԷ������Ч�����ͻ����ǲ���֤�ģ����Կ�д�ɲ�д�����Լ�ϲ��
        if (plateVisualSO != null) {
            Debug.Log("Successfully loaded PlateVisualSO.");
        }
        else {
            Debug.LogError("Failed to load PlateVisualSO. Check the path and ensure the file exists in the Resources folder.");
        }
    }

    //����rpcͬ�����ɵ��߼�
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

    //�ο�������rpcͬ����ʽ�����Ǽ��ٵ��߼�
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

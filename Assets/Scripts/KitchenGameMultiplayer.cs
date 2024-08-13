using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    //Ҫ������Ϊһ�������ϵĵ���
    public static KitchenGameMultiplayer Instance { get; private set; }

    //�����Ǹ�ͳһ��¼����ͬ��ʳ��Ԥ����SO��SO�嵥
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private void Awake() {
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        //������Ҫע��netcode���ǲ���֪�������Լ�Ū��ʲô���͵ģ���ֻ����һЩĬ�ϵ�
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    
    //Ϊ���ÿͻ��˿������ɣ�������Ҫrpc
    [ServerRpc(RequireOwnership = false)]
    //private void SpawnKitchenObjectServerRpc(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        //Ҫת�����ͣ�����netcode ������
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);


        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        //��ȡ����ʳ����ЩԤ������������Ȼ��Ϊ�˷�����ԣ���KitchenObjectȡ�����漰�л��������Ĳ���
        //���ﵱ����ֻ���ɲ���
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        //���ж��ܲ��ܻ�ȡnetcode֧�ֵ�������͵ı���
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
    
    //��ȡ��Ӧʳ��SO��index
    private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }
}

using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    //要将其作为一个联网上的单例
    public static KitchenGameMultiplayer Instance { get; private set; }

    //引用那个统一记录所有同步食物预制体SO的SO清单
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private void Awake() {
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        //我们需要注意netcode它是不能知道我们自己弄了什么类型的，它只接受一些默认的
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    
    //为了让客户端可以生成，所以需要rpc
    [ServerRpc(RequireOwnership = false)]
    //private void SpawnKitchenObjectServerRpc(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        //要转换类型，避免netcode 的问题
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);


        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        //获取厨房食物这些预制体联网对象，然后为了方便测试，在KitchenObject取消了涉及切换父类对象的操作
        //这里当初的只生成测试
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        //先判断能不能获取netcode支持的这个类型的变量
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
    
    //获取对应食物SO的index
    private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }
}

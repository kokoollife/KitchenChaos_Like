using Unity.VisualScripting;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() { return kitchenObjectSO; }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        if(this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }

        kitchenObjectParent.SetKitchenObject(this);
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }
    public IKitchenObjectParent GetClearCounter() { return kitchenObjectParent; }

    public void DestroySelf() {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent) {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

        return kitchenObject;
    }

    //ʵ��Ŀ��3������һ����װ�ĺ�����ר�Ŵ���������ŵ���ȥ���Բ�ͬ�Ĺ�̨��ȡʳ���
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        //���ʳ���������ǵ���
        if(this is PlateKitchenObject) {
            //��������ж��⣬����Ҫ�����ҵ�ǰ�����õ��ǵ��������Ϸ����
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        //����������ǣ���ôʲôҲ�������͵����ж�
        else {
            plateKitchenObject = null;
            return false;
        }
    }
}

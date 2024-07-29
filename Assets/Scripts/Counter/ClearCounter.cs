using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {          
            if (player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else {

            }
        }
        else {
            if (player.HasKitchenObject()) {
                //ʵ��Ŀ��3����װԭ��������ŵ��ӣ�ʳ���������ӵĴ����߼�
                //if(player.GetKitchenObject() is PlateKitchenObject) {
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //PlateKitchenObject plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject;
                    //Ҫȷ�����ܲ����ã������ж��Ƿ��������
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
                //ʵ��Ŀ��4����ҵ�ǰ����ʳ�Ҫ���뵽������
                else {
                    //�����ǰ��̨ӵ�е��ӣ�ע�����ﲻ�ܸ�����һ��д��PlateKitchenObject plateKitchenObject��
                    //��Ϊ�����Ѿ������ˣ�����ֻ��Ū��ͬ���ľֲ�����
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        //��̨�ϵĵ��Ӿ���Ҫ��ȡ������������ŵ�ʳ�����ݽ����ж�
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            //�����������������ŵ�ʳ������
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}

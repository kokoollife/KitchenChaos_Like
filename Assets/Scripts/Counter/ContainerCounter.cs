using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        //��������������û����Ʒ
        if (!player.HasKitchenObject()) {
            //���ڵ��߼��ĳ����ֱ���õ���Ʒ�����ǲ���Ҫ�ڹ�̨�����ɣ�Ȼ�������ȡ�����Բ���Ҫ��ȡ�����ɵ�λ�á�
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
        //������������������Ʒ
        else {
            Debug.Log("�����ж���������debugһ��");
        }
    }
}

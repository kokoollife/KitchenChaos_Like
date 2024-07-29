using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        //如果现在玩家手上没有物品
        if (!player.HasKitchenObject()) {
            //现在的逻辑改成玩家直接拿到物品，我们不需要在柜台上生成，然后玩家拿取，所以不需要获取到生成的位置。
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
        //如果玩家现在手上有物品
        else {
            Debug.Log("手上有东西，这里debug一下");
        }
    }
}

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
                //实现目标3：封装原来玩家拿着碟子，食物遇到碟子的处理逻辑
                //if(player.GetKitchenObject() is PlateKitchenObject) {
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //PlateKitchenObject plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject;
                    //要确认我能不能拿，再来判断是否可以销毁
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
                //实现目标4：玩家当前拿着食物，要放入到碟子中
                else {
                    //如果当前柜台拥有碟子，注意这里不能跟上面一样写成PlateKitchenObject plateKitchenObject，
                    //因为上面已经定义了，这里只能弄个同名的局部变量
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        //柜台上的碟子就需要获取到玩家手上拿着的食物数据进行判断
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            //最后销毁玩家手上拿着的食物数据
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

using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    //碟子父对象
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    //其下子对象IconTemplate，也是我们之后要生成多个的游戏对象
    [SerializeField] private Transform iconTemplate;

    private void Awake() {
        //唤醒的时候，是不能让IconTemplate游戏对象启用的，因为还没有加入新的食物
        iconTemplate.gameObject.SetActive(false);
    }

    //一开始还是启用之前的事件OnIngredientAdded
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        //这个事件新加入一个处理的函数，这个函数负责生成碟子上有的物体的图标
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach(Transform child in transform) {
            //如果我们新加入的食物，已经之前有过了，跳过生成图标这一步操作
            if (child == iconTemplate) continue;

            //因为我们把食物加入到碟子当中，会保留原来食物的游戏对象，为了避免重复需要销毁掉
            Destroy(child.gameObject);
        }

        //最后遍历出目前碟子上有的所有的食物数据
        foreach(KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
            //图标模板游戏对象iconTemplate生成的位置确定在transform也就是自己之下
            Transform iconTransform = Instantiate(iconTemplate, transform);
            //既然都生成了，那么就启用它
            iconTransform.gameObject.SetActive(true);
            //调用相应的方法，去生成图标模板下的图标图片。
            iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}

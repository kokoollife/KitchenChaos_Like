using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    //解决难题：
    //就是我们怎么知道我们加入的食物数据，就对应上我们目前视觉模型对应的子模型呢？
    //如果依靠模型的名称去判断，很容易出错，该怎么处理？
    //用结构体，重新定义好我们要拿取的数据，做一个值类型中，相互对应的键值对
    //要注意序列化，不然无法序列化引用
    [Serializable]
    public struct KitchenObjectSO_GameObject {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;

    //启用事件
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList) {
            //默认一开始都不启用汉堡包模型的所有游戏对象
            kitchenObjectSO_GameObject.gameObject.SetActive(false);
        }
    }

    //这个就是OnIngredientAdded公共事件可以做到的函数功能：
    //如下逻辑
    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        //遍历列表中的数据
        foreach(KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList) {
            //如果我新加入食物数据到碟子对象上，这个数据等于我记录的结构体中的食物数据，那么就启用结构体中的模型，也就是游戏对象
            if(kitchenObjectSO_GameObject.kitchenObjectSO == e.kitchenObjectSO) {
                kitchenObjectSO_GameObject.gameObject.SetActive(true);
            }
        }
    }
}

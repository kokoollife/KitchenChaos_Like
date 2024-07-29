using System;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    //就是显示在世界空间里面的UI系统下的容器，这个容器装下所有食谱菜单的模板
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }

    //我们一般都是在start里面加入事件处理的相关函数的
    private void Start() {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        UpdateVisual();//同样这里也要写入，避免前面的事件都没有做，出现失效
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e) {
        UpdateVisual(); 
    }

    private void UpdateVisual() {
        //遍历获取旗下的子对象
        foreach(Transform child in container) {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        //接着获取当前等待的食谱列表里面具体的食谱数据
        foreach(RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
            //接着在container下新建子对象recipeTemplate
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            //修改具体的名称
            recipeTransform.GetComponent<DeliverManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}

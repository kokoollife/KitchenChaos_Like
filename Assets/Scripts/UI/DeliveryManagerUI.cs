using System;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    //������ʾ������ռ������UIϵͳ�µ��������������װ������ʳ�ײ˵���ģ��
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }

    //����һ�㶼����start��������¼��������غ�����
    private void Start() {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        UpdateVisual();//ͬ������ҲҪд�룬����ǰ����¼���û����������ʧЧ
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e) {
        UpdateVisual(); 
    }

    private void UpdateVisual() {
        //������ȡ���µ��Ӷ���
        foreach(Transform child in container) {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        //���Ż�ȡ��ǰ�ȴ���ʳ���б���������ʳ������
        foreach(RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
            //������container���½��Ӷ���recipeTemplate
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            //�޸ľ��������
            recipeTransform.GetComponent<DeliverManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    //������⣺
    //����������ô֪�����Ǽ����ʳ�����ݣ��Ͷ�Ӧ������Ŀǰ�Ӿ�ģ�Ͷ�Ӧ����ģ���أ�
    //�������ģ�͵�����ȥ�жϣ������׳�������ô����
    //�ýṹ�壬���¶��������Ҫ��ȡ�����ݣ���һ��ֵ�����У��໥��Ӧ�ļ�ֵ��
    //Ҫע�����л�����Ȼ�޷����л�����
    [Serializable]
    public struct KitchenObjectSO_GameObject {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;

    //�����¼�
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList) {
            //Ĭ��һ��ʼ�������ú�����ģ�͵�������Ϸ����
            kitchenObjectSO_GameObject.gameObject.SetActive(false);
        }
    }

    //�������OnIngredientAdded�����¼����������ĺ������ܣ�
    //�����߼�
    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        //�����б��е�����
        foreach(KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList) {
            //������¼���ʳ�����ݵ����Ӷ����ϣ�������ݵ����Ҽ�¼�Ľṹ���е�ʳ�����ݣ���ô�����ýṹ���е�ģ�ͣ�Ҳ������Ϸ����
            if(kitchenObjectSO_GameObject.kitchenObjectSO == e.kitchenObjectSO) {
                kitchenObjectSO_GameObject.gameObject.SetActive(true);
            }
        }
    }
}

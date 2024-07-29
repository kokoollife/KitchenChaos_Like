using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    //���Ӹ�����
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    //�����Ӷ���IconTemplate��Ҳ������֮��Ҫ���ɶ������Ϸ����
    [SerializeField] private Transform iconTemplate;

    private void Awake() {
        //���ѵ�ʱ���ǲ�����IconTemplate��Ϸ�������õģ���Ϊ��û�м����µ�ʳ��
        iconTemplate.gameObject.SetActive(false);
    }

    //һ��ʼ��������֮ǰ���¼�OnIngredientAdded
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        //����¼��¼���һ������ĺ�������������������ɵ������е������ͼ��
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach(Transform child in transform) {
            //��������¼����ʳ��Ѿ�֮ǰ�й��ˣ���������ͼ����һ������
            if (child == iconTemplate) continue;

            //��Ϊ���ǰ�ʳ����뵽���ӵ��У��ᱣ��ԭ��ʳ�����Ϸ����Ϊ�˱����ظ���Ҫ���ٵ�
            Destroy(child.gameObject);
        }

        //��������Ŀǰ�������е����е�ʳ������
        foreach(KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
            //ͼ��ģ����Ϸ����iconTemplate���ɵ�λ��ȷ����transformҲ�����Լ�֮��
            Transform iconTransform = Instantiate(iconTemplate, transform);
            //��Ȼ�������ˣ���ô��������
            iconTransform.gameObject.SetActive(true);
            //������Ӧ�ķ�����ȥ����ͼ��ģ���µ�ͼ��ͼƬ��
            iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}

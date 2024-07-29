using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour
{
    //加入图标 Image
    [SerializeField] private Image icon;

    //修改它子类的图标
    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
        icon.sprite = kitchenObjectSO.sprite;
    }
}

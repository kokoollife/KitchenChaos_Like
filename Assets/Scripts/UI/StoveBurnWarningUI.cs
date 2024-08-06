using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start() {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

        Hide();
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        //就是因为传入了正则化处理的滚条值，所以我们可以直接判断是否显示警告标志
        float burnShowProgressAmount = .5f;
        //要注意是正在烤的情况
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;
    
        if (show) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}

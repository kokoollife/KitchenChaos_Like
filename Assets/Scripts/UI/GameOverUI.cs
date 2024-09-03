using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    //注意补充相应的ui按钮
    [SerializeField] private Button playAgainButton;
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;

    //补充这个函数
    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e) {
        if (KitchenGameManager.Instance.IsGameOver()) {
            Show();
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
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

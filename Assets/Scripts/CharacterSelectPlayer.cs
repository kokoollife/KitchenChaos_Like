using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObjectText;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    //补充
    [SerializeField] private TextMeshPro playerNameText;

    private void Awake() {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            //我们踢人不忘处理lobby
            KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e) {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e) {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObjectText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            //这里改动玩家的名称
            playerNameText.text = playerData.playerName.ToString();
            
            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));

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

    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged-= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
    }
}

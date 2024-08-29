using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake() {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e) {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        //����ӳ�û�м����ǲ�����ʾ�ı��ģ������Լ�����
        if(messageText.text == "") {
            messageText.text = "Failed to connect";
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    //ȡ�������¼�����������������Ƿ������붩�����������ڲ�һ�µ����
    //�������������Ϣ�����ˣ��͸�ֻ��lobby��������ʾ��������Ӱ�쵽��������������Ҫ����
    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
    }
}

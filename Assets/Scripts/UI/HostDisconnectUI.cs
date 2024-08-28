using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    //��ʱ�Ȳ������½�����Ϸ�����
    [SerializeField] private Button playAgainButton;

    private void Awake() {
        
    }

    private void Start() {
        //������������������
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        //ר���жϷ���˵�id���ͽ������3
        if(clientId == NetworkManager.ServerClientId) {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

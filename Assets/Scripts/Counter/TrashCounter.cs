using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;

    new public static void ResetStaticData() {
        OnAnyObjectTrashed = null;
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            //player.GetKitchenObject().DestroySelf();
            //�Ķ�ԭ���ķ���
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());

            InteractLogicServerRpc();
        }
    }
    //Ūrpc��������Ҫ�Ķ���������ٶ���ķ�ʽ��
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}

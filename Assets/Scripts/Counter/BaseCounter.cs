using System;
using Unity.Netcode;
using UnityEngine;

//�Ķ��̳�
public class BaseCounter : NetworkBehaviour,IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public virtual void Interact(Player player) {
        Debug.LogError("BaseCounter.Interact()");
    }

    public virtual void InteractAlternate (Player player) {
        //Debug.LogError("BaseCounter.InteractAlternate()");
    }

    public Transform GetKitchenObjectFollowTransform() {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if(kitchenObject != null) {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() { return kitchenObject; }

    public void ClearKitchenObject() { kitchenObject = null; }

    public bool HasKitchenObject() { return kitchenObject != null; }

    public Transform GetCounterTopPoint() {
        return counterTopPoint;
    }

    public static void ResetStaticData() {
        OnAnyObjectPlacedHere = null;
    }

    //�Ķ�
    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}

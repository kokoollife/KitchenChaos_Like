using Unity.Netcode;
using UnityEngine;

public class PlayerSync : NetworkBehaviour {
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<Quaternion> _syncRota = new();
    private Transform _syncTransform;
    private NetworkVariable<bool> _syncisWalking = new();
    private Animator _syncAnimator;
    private string isWalking_Anim;

    private void Awake() {
        SetTarget(this.transform);
    }

    private void Start() {
        isWalking_Anim = PlayerAnimator.GetPlayerAnimatorName();
    }

    private void SetTarget(Transform player) {
        _syncTransform = player;
        _syncAnimator = player.GetComponentInChildren<Animator>();
    }

    private void Update() {
        if (IsLocalPlayer) {
            UploadTransform();
            UploadAnimation();
        }
    }

    private void FixedUpdate() {
        if (!IsLocalPlayer) {
            SyncTransform();
            SyncAnimation();
        }
    }

    private void SyncTransform() {
        _syncTransform.position = _syncPos.Value;
        _syncTransform.rotation = _syncRota.Value;
    }

    private void SyncAnimation() {
        _syncAnimator.SetBool(isWalking_Anim, _syncisWalking.Value);
    }

    private void UploadTransform() {
        if (IsServer) {
            _syncPos.Value = transform.position;
            _syncRota.Value = transform.rotation;
        }
        else {
            UploadTransformServerRpc(transform.position, transform.rotation);
        }
    }

    [ServerRpc]
    private void UploadTransformServerRpc(Vector3 position, Quaternion rotation) {
        _syncPos.Value = position;
        _syncRota.Value = rotation;
    }

    private void UploadAnimation() {
        if (IsServer) {
            _syncisWalking.Value = _syncAnimator.GetBool(isWalking_Anim);
        }
        else {
            UploadAnimationServerRpc();
        }
    }

    [ServerRpc]
    private void UploadAnimationServerRpc() {
        _syncisWalking.Value = _syncAnimator.GetBool(isWalking_Anim);
    }
}


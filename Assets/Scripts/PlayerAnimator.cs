using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player player;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }
        animator.SetBool(IS_WALKING, player.IsWalking());
    }

    public static string GetPlayerAnimatorName() {
        return IS_WALKING;
    }
}

using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    //就是隔一段时间，就让脚步声音播放
    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = .1f;

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        //计时器启动
        footstepTimer -= Time.deltaTime;
        if(footstepTimer < 0f) {
            //重置
            footstepTimer = footstepTimerMax;

            if (player.IsWalking()) {
                //使用音效管理的单例相关的方法，
                //不过不建议滥用单例，这里因为跟角色绑定在一起了，
                //所以我们可以这样搞，也不会影响很大。
                float volume = 1f;
                SoundManager.Instance.PlayFootstepsSound(player.transform.position, volume);
            }
        }
    }
}

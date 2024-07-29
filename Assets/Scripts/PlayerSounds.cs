using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    //���Ǹ�һ��ʱ�䣬���ýŲ���������
    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = .1f;

    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        //��ʱ������
        footstepTimer -= Time.deltaTime;
        if(footstepTimer < 0f) {
            //����
            footstepTimer = footstepTimerMax;

            if (player.IsWalking()) {
                //ʹ����Ч����ĵ�����صķ�����
                //�������������õ�����������Ϊ����ɫ����һ���ˣ�
                //�������ǿ��������㣬Ҳ����Ӱ��ܴ�
                float volume = 1f;
                SoundManager.Instance.PlayFootstepsSound(player.transform.position, volume);
            }
        }
    }
}

using System;
using UnityEngine;


public class KitchenGameManager : MonoBehaviour
{
    //Ū����
    public static KitchenGameManager Instance { get; private set; }
    //����Ϸ�е�״̬
    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    //�¼�
    public event EventHandler OnStateChanged;

    private State state;
    //Ū����ʱ��
    [SerializeField] private float waitingToStartTimer = 1f;
    [SerializeField] private float countdownToStartTimer = 3f;
    [SerializeField] private float gamePlayingTimer;
    [SerializeField] private float gamePlayingTimerMax = 10f;

    private void Awake() {
        Instance = this;
        //һ��ʼ���ó�ʼ״̬��׼����ʼ״̬
        state = State.WaitingToStart;
    }

    private void Update() {
        //�л�״̬
        switch (state) {
            //һ��ʼ׼����ʼ״̬��������ʱ�����������л���ȷ����ʼ״̬
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime; 
                if(waitingToStartTimer < 0f) {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this,EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if(countdownToStartTimer < 0f) {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if(gamePlayingTimer < 0f) {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
        Debug.Log(state);
    }


    //����״̬�ж�
    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }

    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1-(gamePlayingTimer / gamePlayingTimerMax);
    }
}

using System;
using UnityEngine;


public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private State state;
    //[SerializeField] private float waitingToStartTimer = 1f;
    [SerializeField] private float countdownToStartTimer = 3f;
    [SerializeField] private float gamePlayingTimer;
    [SerializeField] private float gamePlayingTimerMax = 10f;

    private bool isGamePaused = false;

    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        //�����¼�����������������������������ͼƬ�����½�����֮�����ǲ�����ʽ������Ϸ������ʱ�׶�
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        //�����ǰ״̬��׼����ʼ������Ϸ
        if(state == State.WaitingToStart) {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }

    }

    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                //���ǰ��߼��Ĵ��������¼�����Ϊ�̳���صĽű�����
                //��Ϸ�������У�����ֻ�ø�״̬��Ϊ�ж�
                //ͬʱҲ������Ҫ��ʱ����һ��������
                //ֱ�Ӿ��ǽ��뵽��������ͼƬ����ס��ֱ�����ǽ���
                //waitingToStartTimer -= Time.deltaTime; 
                //if(waitingToStartTimer < 0f) {
                //    state = State.CountdownToStart;
                //    OnStateChanged?.Invoke(this,EventArgs.Empty);
                //}
                //�ͽ��뵽��Ϸ����ʱǰ�ĵ����׶Ρ�
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

using System;
using UnityEngine;


public class KitchenGameManager : MonoBehaviour
{
    //弄单例
    public static KitchenGameManager Instance { get; private set; }
    //搞游戏中的状态
    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    //事件
    public event EventHandler OnStateChanged;

    private State state;
    //弄个计时器
    [SerializeField] private float waitingToStartTimer = 1f;
    [SerializeField] private float countdownToStartTimer = 3f;
    [SerializeField] private float gamePlayingTimer;
    [SerializeField] private float gamePlayingTimerMax = 10f;

    private void Awake() {
        Instance = this;
        //一开始设置初始状态，准备开始状态
        state = State.WaitingToStart;
    }

    private void Update() {
        //切换状态
        switch (state) {
            //一开始准备开始状态，经过计时器计数，就切换到确定开始状态
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


    //返回状态判断
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

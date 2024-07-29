using System;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    //负责处理碟子视觉生成效果
    public event EventHandler OnPlateSpawned;
    //负责移除
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    //碟子的生成时间与规定时间
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update() {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax) {
            //我们不要真的生成碟子这个物体对象，而是要生成对应的视觉效果
            //这样就不会遇到我们之前规定的一个柜台只能有一个物体。
            //KitchenObject.SpawnKitchenObject(plateKitchenObjectSO,this);
            spawnPlateTimer = 0f;

            if(platesSpawnedAmount < platesSpawnedAmountMax) {
                platesSpawnedAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player) {
        //如果玩家当前手上没物体
        if (!player.HasKitchenObject()) {
            //碟子已经有生成了
            if(platesSpawnedAmount > 0) {
                platesSpawnedAmount--;
                //那么就允许玩家交互获取这个碟子
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

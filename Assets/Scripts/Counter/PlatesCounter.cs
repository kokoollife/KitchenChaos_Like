using System;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    //����������Ӿ�����Ч��
    public event EventHandler OnPlateSpawned;
    //�����Ƴ�
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    //���ӵ�����ʱ����涨ʱ��
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update() {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax) {
            //���ǲ�Ҫ������ɵ������������󣬶���Ҫ���ɶ�Ӧ���Ӿ�Ч��
            //�����Ͳ�����������֮ǰ�涨��һ����ֻ̨����һ�����塣
            //KitchenObject.SpawnKitchenObject(plateKitchenObjectSO,this);
            spawnPlateTimer = 0f;

            if(platesSpawnedAmount < platesSpawnedAmountMax) {
                platesSpawnedAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player) {
        //�����ҵ�ǰ����û����
        if (!player.HasKitchenObject()) {
            //�����Ѿ���������
            if(platesSpawnedAmount > 0) {
                platesSpawnedAmount--;
                //��ô��������ҽ�����ȡ�������
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

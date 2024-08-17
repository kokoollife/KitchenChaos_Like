using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;

    private List<GameObject> plateVisualGameObjectList;

    private void Awake() {
        plateVisualGameObjectList = new List<GameObject>();
        
    }

    private void Start() {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    private void PlatesCounter_OnPlateRemoved(object sender, EventArgs e) {
        //移除最后的碟子视觉
        GameObject plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
        plateVisualGameObjectList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

    
    private void PlatesCounter_OnPlateSpawned(object sender, PlatesCounter.OnPlateSpawnedPlateVisualEventArgs e) {
        GameObject plateVisualTransform = Instantiate(e.plateVisualSO.plateVisualPrefab);
        //Debug.Log("counterTopPoint" + gameObject.transform.position);
        
        float plateOffsetY = .1f;
        //Debug.Log("plate"+ plateVisualTransform);
        Vector3 plateVisualBornPoint = new Vector3(gameObject.transform.position.x,counterTopPoint.transform.position.y, gameObject.transform.position.z);
        plateVisualTransform.transform.position = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0)+ plateVisualBornPoint;

        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
    }
    
    /*
    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e) {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);
        float plateOffsetY = .1f;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0);

        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
    }
    */
}

using UnityEngine;

public class FPSDisplay : MonoBehaviour {
    private float deltaTime = 0.0f;

    private void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI() {
        float fps = 1.0f / deltaTime;
        GUI.Label(new Rect(10, 10, 100, 20), $"FPS: {fps}");
    }
}

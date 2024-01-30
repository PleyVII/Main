using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform target;
    Vector3 Cameraposition;
    public Vector3 offset;
    public float pitch = 2f;
    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float currentRotation = 0f;
    public float cameraRotationSpeed = 100f;
    private float currentZoom = 10f;

    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.4f;
    // public FishNet.Component.Spawning.PlayerSpawner Sender;
    void Start() {
        // Sender = GameObject.FindObjectOfType<FishNet.Component.Spawning.PlayerSpawner>();
        // Sender.OnSpawned += OnSpawnedFunction;
        Cameraposition = transform.position;
    }
    bool DoubleClick() {
        if (Input.GetMouseButtonDown(1)) {
            clicked++;
            if (clicked == 1) clicktime = Time.time;
        }
        if (clicked > 1 && Time.time - clicktime <= clickdelay) {
            clicked = 0;
            clicktime = 0;
            return true;
        } else if (Time.time - clicktime > clickdelay) clicked = 0;
        return false;
    }

    void Update() {
        if (Input.GetMouseButton(1)) {
            currentRotation -= Input.GetAxis("Mouse X") * cameraRotationSpeed * Time.deltaTime;
        }
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        if (DoubleClick()) {
            currentRotation = 0;
        }
    }
    public void OnSpawnedFunction(GameObject ObjectPlayer) {
        target = ObjectPlayer.transform;
    }
    // public override void OnStopClient()
    // {
    //     base.OnStopClient();
    //     target = null;
    // }
    void LateUpdate() {
        if (target != null) {
            Cameraposition += (target.position - Cameraposition) * 0.1f;
            transform.position = Cameraposition - offset * currentZoom;
            transform.LookAt(Cameraposition + Vector3.up * pitch);
            transform.RotateAround(Cameraposition, Vector3.up, currentRotation);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager Instance { get; private set; }

    public GameObject prefabToSpawn; // Drag your character prefabs here
    public GameObject characterInitialization; // Drag your character prefabs here
    public AllObjectInformation whosEquipmentIsBeingHoveredOver; public bool isHoveringOverPersonEquipment;
    public AllObjectInformation[] characters = new AllObjectInformation[3];
    private Queue<AllObjectInformation> focused_queue = new Queue<AllObjectInformation>();

    public AllObjectInformation TemporaryFocusedCharacter {
        get {
            // Return the frontmost item, or null if the queue is empty
            return focused_queue.Count > 0 ? focused_queue.Peek() : null;
        }
        set {
            if (value != null) {
                focused_queue.Enqueue(value);  // Enqueue the object if it's not null
            } else if (focused_queue.Count > 0) {
                focused_queue.Dequeue();  // Dequeue the frontmost object if the queue is not empty
            }
        }
    }
    public AllObjectInformation focusedCharacter;
    public AllObjectInformation FocusedCharacter {
        get {
            if (TemporaryFocusedCharacter != null) return TemporaryFocusedCharacter;
            else return focusedCharacter;
        }
    }
    public float offsetX = 0, offsetY = 4, offsetZ = -10;
    public int focusedCharacterIndex = 0;
    void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.LogError("There are two instances of Player Manager, fix your shiet yo"); Destroy(gameObject);
        }
    }
    public Camera cam;

    void Start() {
        if (characterInitialization != null) { characters[0] = characterInitialization.GetComponent<CharacterUpdates>().allInfo; ChangeFocus(0); }
        cam = Camera.main;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            for (int i = 0; i < characters.Length; i++) {
                if (characters[i] == null) {
                    SpawnCharacter(i, prefabToSpawn);
                    break;
                }
            }
        }

        // Change focused character
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ChangeFocus(0); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ChangeFocus(1); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { ChangeFocus(2); }
    }

    void SpawnCharacter(int index, GameObject characterPrefab) {
        AllObjectInformation newCharacter = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity).GetComponent<CharacterUpdates>().allInfo;
        characters[index] = newCharacter;
        // Get and store character info if needed
        // CharacterUpdates characterUpdates = newCharacter.GetComponent<CharacterUpdates>();
        // characterUpdates.allInfo = ...;
    }

    void ChangeFocus(int index) {
        if (index < characters.Length && characters[index] != null) {
            focusedCharacterIndex = index;
            GetFocusedCharacter();
            // Logic to focus camera on new character
            Vector3 cameraOffset = new Vector3(offsetX, offsetY, offsetZ); // You can adjust this as needed
            Camera.main.transform.position = focusedCharacter.Owner.transform.position + cameraOffset;
            // Make the camera look at the character
            Camera.main.transform.LookAt(focusedCharacter.Owner.transform.position);
        }
    }

    private void GetFocusedCharacter() {
        focusedCharacter = characters[focusedCharacterIndex];
    }

    public AllObjectInformation WhosEquipmentAreYouHoveringOver() {
        return whosEquipmentIsBeingHoveredOver;
    }
}

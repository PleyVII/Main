using UnityEngine;

public class InventoryUI : MonoBehaviour {
    Inventory inventory;
    public GameObject inventoryUI;
    public GameObject statUI;
    public GameObject characterUI;
    public Transform itemsParent;
    InventorySlot[] slots;
    void Start() {
        inventory = Inventory.Instance;
        inventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Inventory")) {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
        if (Input.GetButtonDown("StatUI")) {
            statUI.SetActive(!statUI.activeSelf);
        }
        if (Input.GetButtonDown("CharacterUI")) {
            characterUI.SetActive(!characterUI.activeSelf);
        }



    }
    void UpdateUI() {
        for (var i = 0; i < slots.Length; i++) {
            if (i < inventory.items.Count) {
                slots[i].AddItem(inventory.items[i]);
            } else {
                slots[i].ClearSlot();
            }
        }
    }
}

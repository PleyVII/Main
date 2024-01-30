using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
    Item item;
    public Button removeButton;
    public Image icon;
    public void AddItem(Item newItem) {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }
    public void ClearSlot() {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton() {
        Inventory.Instance.RemoveFromInventory(item);
    }
    public void UseItem(AllObjectInformation allInfo) {
        if (item != null) {
            item.Use(PlayerManager.Instance.focusedCharacter);
        }
    }
}
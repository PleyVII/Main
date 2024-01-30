using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public abstract class Item : ScriptableObject {
    new public string name = "New Item";
    public string description; 
    public Sprite icon = null;
    public float pickUpRadius = 2f;

    public abstract void Use(AllObjectInformation allInfo);

    public void RemoveFromInventory() {
        Inventory.Instance.RemoveFromInventory(this);
    }

    public void AddToInventory() {
        Inventory.Instance.AddToInventory(this);
    }
}
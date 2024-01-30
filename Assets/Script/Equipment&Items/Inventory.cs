using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    #region Singleton
    private static Inventory instance;
    public static Inventory Instance {
        get {
            if (instance == null) Debug.Log("There is no Singleton of Inventory Instance Yo");
            return instance;
        }
    }
    void Awake() {
        if (instance != null) Debug.LogWarning("There was more than one Singleton of Inventory Instance Yo");
        instance = this;
    }
    #endregion
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    public int space = 100;
    public List<Item> items = new List<Item>();

    public bool AddToInventory(Item item) {
        if (items.Count >= space) {
            Debug.Log("No empty slots left, " + space + "/" + space + " slots being used");
            return false;
        }

        items.Add(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
        return true;
    }
    public void RemoveFromInventory(Item item) {
        items.Remove(item);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}

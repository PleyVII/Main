using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Equipment : EquipmentBase {

    public int maximumCardSlots;
    public float weight = 0f;
    [SerializeField] public EquipmentSlot[] equipSlot;
    [HideInInspector] public AllObjectInformation equipedOn;
    [HideInInspector] public List<Card> cardSlots = new List<Card>();

    public void AddCard(Card card, bool fromInventory = true) {
        if (!equipSlot.Contains(card.equippedInSlot) && cardSlots.Count < maximumCardSlots - card.howManySlotsDoesItRequireDefault_1) return;
        cardSlots.Add(card);
        card.insertedIn = this;
        if (fromInventory) card.RemoveFromInventory();
        if (equipedOn != null) card.Use(equipedOn);
    }

    public void RemoveCard(Card card) {
        cardSlots.Remove(card);
        card.insertedIn = null;
        if (equipedOn != null) card.StopUsing();
        else card.AddToInventory();
    }

    public override void Use(AllObjectInformation allInfo) {
        Debug.Log("Equiping " + name);
        equipedOn = allInfo;
        EquipmentManager.Instance.Equip(allInfo, this);
        foreach (Card card in cardSlots) {
            card.Use(allInfo);
        }
    }

    public void StopUsing() {
        foreach (Card card in cardSlots) {
            card.StopUsing(); // Assuming the Card class has a StopUsing() method that takes AllObjectInformation as a parameter
        }
        if (equipedOn == null) { Debug.LogError("That shouldnt be happening Yo"); return; }
        EquipmentManager.Instance.Unequip(equipedOn, this);
    }

    void Start() {
        CheckEquipmentConditions();
    }

    private void CheckEquipmentConditions() {
        foreach (EquipmentModifier modifier in EquipmentModifiersList) {
            foreach (ConditionalModifier condition in modifier.conditions) {
                EquipmentCondition equipmentCondition = condition as EquipmentCondition;
                if (equipmentCondition != null) equipmentCondition.ComboItemsAlwaysWillAddItselfByDefault.Add(this);
                else equipmentCondition.ComboItemsAlwaysWillAddItselfByDefault = new List<EquipmentBase>();

            }
        }
    }
}

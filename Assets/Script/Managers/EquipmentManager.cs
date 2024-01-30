using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {
    #region Singleton
    private static EquipmentManager instance;
    public static EquipmentManager Instance {
        get {
            if (instance == null) Debug.Log("There is no Singleton of EquipmentManager Instance Yo");
            return instance;
        }
    }
    public void Awake() {
        if (instance != null) Debug.LogWarning("There was more than one Singleton of EquipmentManager Instance Yo");
        instance = this;
    }
    #endregion

    Inventory inventory;
    public delegate void OnEquipmentChanged(AllObjectInformation character, EquipmentBase newEquipment, EquipmentBase oldEquipment);
    public delegate void OnModifiersChangeCheck(AllObjectInformation character, List<EquipmentBase.EquipmentModifier> modifiersList);
    public OnEquipmentChanged onEquipmentChanged;
    public OnModifiersChangeCheck onModifiersChangeCheck;

    void Start() {
        inventory = Inventory.Instance;
    }

    public void Equip(AllObjectInformation character, EquipmentBase newEquipment) {
        bool newItemEquipped = false;

        switch (newEquipment) {
            case Weapon newWeapon:
                if (newWeapon.weaponType == WeaponType.TwoHandedSword ||
                            newWeapon.weaponType == WeaponType.TwoHandedSpear ||
                            newWeapon.weaponType == WeaponType.TwoHandedAxe ||
                            newWeapon.weaponType == WeaponType.Bow ||
                            newWeapon.weaponType == WeaponType.MusicalInstrument) {
                    // Assign both hands for two-handed weapons
                    EquipInSlot(character, newWeapon, EquipmentSlot.Lefthand, true);
                    EquipInSlot(character, newWeapon, EquipmentSlot.Righthand, false);
                } else if (character.currentEquipment[(int)EquipmentSlot.Lefthand] == null) {
                    EquipInSlot(character, newWeapon, EquipmentSlot.Lefthand, false);
                } else if (character.currentEquipment[(int)EquipmentSlot.Righthand] == null) {
                    EquipInSlot(character, newWeapon, EquipmentSlot.Righthand, false);
                } else {
                    EquipInSlot(character, newWeapon, EquipmentSlot.Lefthand, false);
                }
                break;

            case Armor NewEquipment:
                foreach (EquipmentSlot equipmentSlot in NewEquipment.equipSlot) {
                    newItemEquipped = EquipInSlot(character, NewEquipment, equipmentSlot, newItemEquipped);
                }
                break;

            case Card newCard:
                onEquipmentChanged?.Invoke(character, newEquipment, null); // No oldItem in this case
                break;
        }
    }


    private bool EquipInSlot(AllObjectInformation character, Equipment newEquipment, EquipmentSlot slot, bool alreadyEquiped) {
        bool newItemEquipped = alreadyEquiped;
        Equipment previousEquipmentTakenOff = null;
        EquipmentSlot? slotsToNull = null;
        Equipment oldEquipment = character.currentEquipment[(int)slot];
        if (oldEquipment != null)
            foreach (EquipmentSlot equipmentSlot in oldEquipment.equipSlot) {
                if (previousEquipmentTakenOff == null) inventory.AddToInventory(oldEquipment);               //TODO: Its fucked, I can tell
                previousEquipmentTakenOff = oldEquipment;

                if (equipmentSlot != slot) slotsToNull = equipmentSlot;
            }
        if (slotsToNull != null) slotsToNull = null;
        character.currentEquipment[(int)slot] = newEquipment;


        if (!newItemEquipped) {
            onEquipmentChanged?.Invoke(character, newEquipment, oldEquipment);
            newItemEquipped = true;
        } else {
            onEquipmentChanged?.Invoke(character, null, oldEquipment);
        }

        return newItemEquipped;
    }
    public void Unequip(AllObjectInformation character, EquipmentBase oldEquipment) {
        bool alreadyAddedToInventory = false;
        if (oldEquipment is Equipment OldEquipment) {
            foreach (EquipmentSlot equipmentSlot in OldEquipment.equipSlot) {
                if (character.currentEquipment[(int)equipmentSlot] == OldEquipment) {
                    character.currentEquipment[(int)equipmentSlot] = null; // Remove the item from the slot

                    // Optionally add the unequipped item back to the inventory
                    if (alreadyAddedToInventory == false) {
                        inventory.AddToInventory(OldEquipment);
                        alreadyAddedToInventory = true;
                    }

                    // If you have a delegate or event for when equipment changes, invoke it here
                    onEquipmentChanged?.Invoke(character, null, oldEquipment);
                }
            }
        } else if (oldEquipment is Card oldCard) {
            foreach (Equipment equippedItem in character.currentEquipment) {
                if (equippedItem != null && equippedItem.cardSlots.Contains(oldCard)) {

                    inventory.AddToInventory(oldCard);
                    onEquipmentChanged?.Invoke(character, null, oldEquipment);
                    break;
                }
            }
        }
    }
    //TODO:Add equip for weapons, make sure it frees up two slots for weapons that require those, and make sure it Adds to inventory once.

    public void RecheckingConditionsOnModifierChange(AllObjectInformation character) {
        foreach (Equipment equipment in character.currentEquipment) {
            foreach (Card card in equipment.cardSlots) {
                onModifiersChangeCheck?.Invoke(character, card.EquipmentModifiersList);
            }
            onModifiersChangeCheck?.Invoke(character, equipment.EquipmentModifiersList);
        }
    }

    public bool isEquippedForBonus(AllObjectInformation character, List<EquipmentBase> comboRequirementCheckList, int requiredCount) {
        if (comboRequirementCheckList.Count < requiredCount) return false;
        if (requiredCount <= 0 || comboRequirementCheckList == null) return true;

        // Makes sure that the Equipment check will work with multiple of the same item requirement
        List<EquipmentBase> checkAndRemoveList = new List<EquipmentBase>(character.currentEquipment);
        int countdownToReachRequiredCount = 0; // Counter for the number of matched items

        foreach (EquipmentBase equipment in comboRequirementCheckList) {
            if (checkAndRemoveList.Contains(equipment)) {
                checkAndRemoveList.Remove(equipment);
                countdownToReachRequiredCount++; // Increment the counter when an item is matched

                // Check if the required number of items are matched.
                if (countdownToReachRequiredCount >= requiredCount) return true;
            }
        }
        return false;
    }
}

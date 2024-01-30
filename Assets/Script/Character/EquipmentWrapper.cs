using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataWrapper", menuName = "ScriptableObjects/CharacterDataWrapper", order = 1), SerializeField]
public class EquipmentWrapper : ScriptableObject {
    [SerializeField] public AllObjectInformation allObjectInfo;
    [SerializeField] public Equipment[] currentEquipment = new Equipment[10]; // Array to be set in the Unity Editor
    [SerializeField] public List<Card>[] cardLists = new List<Card>[10]; // Arrays of card lists to be set in the Unity Editor

}

[CustomEditor(typeof(EquipmentWrapper))]
public class EquipmentWrapperEditor : Editor {

    private static readonly EquipmentSlot[] equipmentSlots = {
        EquipmentSlot.Headgear, EquipmentSlot.Armor, EquipmentSlot.Garment,
        EquipmentSlot.Handwear, EquipmentSlot.Footgear,
        EquipmentSlot.Righthand, EquipmentSlot.Lefthand, EquipmentSlot.Necklace,
        EquipmentSlot.Ring1, EquipmentSlot.Ring2
    };

    public override void OnInspectorGUI() {
        EquipmentWrapper dataWrapper = (EquipmentWrapper)target;
        dataWrapper.allObjectInfo = (AllObjectInformation)EditorGUILayout.ObjectField("Drop Equipment to Add", dataWrapper.allObjectInfo, typeof(AllObjectInformation), false);



        Equipment[] equipmentArray = dataWrapper.currentEquipment;
        for (int i = 0; i < equipmentArray.Length; i++) {
            if (equipmentArray[i] != null && !equipmentArray[i].equipSlot.Contains(equipmentSlots[i])) {
                equipmentArray[i] = null;
            }
        }
        if (dataWrapper.currentEquipment == null || dataWrapper.currentEquipment.Length != equipmentSlots.Length) {
            dataWrapper.currentEquipment = new Equipment[equipmentSlots.Length];
        }

        // Array to track if card slots have been added for an equipment
        bool[] isSlotAlreadyHasAddedCards = new bool[equipmentSlots.Length];

        for (int i = 0; i < equipmentSlots.Length; i++) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(equipmentSlots[i].ToString(), GUILayout.Width(100));

            Equipment originalEquipment = dataWrapper.currentEquipment[i];
            dataWrapper.currentEquipment[i] = (Equipment)EditorGUILayout.ObjectField(originalEquipment, typeof(Equipment), false);

            EditorGUILayout.EndHorizontal();

            Equipment newEquipment = dataWrapper.currentEquipment[i];
            if (newEquipment != originalEquipment) {
                if (newEquipment == null) ClearOccupiedSlots(dataWrapper, null, i, originalEquipment);
                else {
                    dataWrapper.currentEquipment[i] = originalEquipment;
                    HandleEquipmentAssignment(dataWrapper, newEquipment, i);
                }
            }

            // Check if the current slot is part of an equipment that spans multiple slots
            bool isPartOfMultiSlotEquipment = newEquipment != null && newEquipment.equipSlot.Length > 1;

            // Display and manage card slots only for equipment that does not span multiple slots or hasn't been handled yet
            if (newEquipment != null && (!isPartOfMultiSlotEquipment || !isSlotAlreadyHasAddedCards[i])) {
                EnsureCardSlotCapacity(dataWrapper, i, newEquipment.maximumCardSlots);
                ManageCardSlots(dataWrapper, i, newEquipment.maximumCardSlots);
                ManageCardSlotButtons(dataWrapper, i, newEquipment);

                // Mark all slots occupied by this equipment as handled
                foreach (var slot in newEquipment.equipSlot) {
                    int index = FindCorrectSlotIndex(slot);
                    if (index != -1) {
                        isSlotAlreadyHasAddedCards[index] = true;
                    }
                }
            }
        }

        EditorUtility.SetDirty(dataWrapper);
    }

    private int FindCorrectSlotIndex(EquipmentSlot slot) {
        for (int i = 0; i < equipmentSlots.Length; i++) {
            if (slot == equipmentSlots[i]) {
                return i; // Return the index of the correct slot
            }
        }
        return -1; // Return -1 if no matching slot is found
    }

    private void HandleEquipmentAssignment(EquipmentWrapper dataWrapper, Equipment newEquipment, int currentIndex) {
        if (newEquipment is Weapon newWeapon) {
            // Handling for different types of weapons
            HandleWeaponAssignment(dataWrapper, newWeapon);
        } else {
            // Handling for non-weapon equipment
            HandleArmorAssignment(dataWrapper, newEquipment);
        }
    }

    private void HandleWeaponAssignment(EquipmentWrapper dataWrapper, Weapon newWeapon) {
        int leftHandIndex = FindCorrectSlotIndex(EquipmentSlot.Lefthand);
        int rightHandIndex = FindCorrectSlotIndex(EquipmentSlot.Righthand);

        if (IsTwoHandedWeapon(newWeapon)) {
            // Assign to both hands if two-handed weapon
            EquipInSlot(dataWrapper, newWeapon, rightHandIndex);
        } else {
            // Assign to left hand if free, else right hand, else left hand
            if (dataWrapper.currentEquipment[leftHandIndex] == null) {
                EquipInSlot(dataWrapper, newWeapon, leftHandIndex);
            } else if (dataWrapper.currentEquipment[rightHandIndex] == null) {
                EquipInSlot(dataWrapper, newWeapon, rightHandIndex);
            } else {
                EquipInSlot(dataWrapper, newWeapon, leftHandIndex);
            }
        }
    }


    private void EquipInSlot(EquipmentWrapper dataWrapper, Equipment equipment, int slotIndex) {
        if (slotIndex != -1) {
            if (equipment is Weapon weapon)
                if (IsTwoHandedWeapon(weapon)) {
                    dataWrapper.currentEquipment[(int)EquipmentSlot.Righthand] = equipment;
                    dataWrapper.currentEquipment[(int)EquipmentSlot.Lefthand] = equipment;

                } else {
                    ClearOccupiedSlots(dataWrapper, null, slotIndex);
                    dataWrapper.currentEquipment[slotIndex] = equipment;
                }
            else
                ClearOccupiedSlots(dataWrapper, equipment.equipSlot);
            dataWrapper.currentEquipment[slotIndex] = equipment;
        }
    }

    private void ClearOccupiedSlots(EquipmentWrapper dataWrapper, EquipmentSlot[] occupiedSlots = null, int specificslot = 0, Equipment equipment = null) {
        Weapon weapon = null;
        if (dataWrapper.currentEquipment[specificslot] is Weapon) weapon = (Weapon)dataWrapper.currentEquipment[specificslot];
        if (equipment is Weapon) weapon = (Weapon)equipment;
        if (weapon != null) {
            if (dataWrapper.currentEquipment[(int)EquipmentSlot.Righthand] != null)
                if (IsTwoHandedWeapon(weapon)) {
                    dataWrapper.currentEquipment[(int)EquipmentSlot.Righthand] = null;
                    dataWrapper.currentEquipment[(int)EquipmentSlot.Lefthand] = null;
                } else dataWrapper.currentEquipment[specificslot] = null;
            else if (occupiedSlots == null) dataWrapper.currentEquipment[specificslot] = null;
        } else if (occupiedSlots != null) {
            foreach (var slot in occupiedSlots) {
                ClearSlotsOccupiedByArmor(dataWrapper, (int)slot);
            }
        } else {
            ClearSlotsOccupiedByArmor(dataWrapper, specificslot, equipment);
        }
    }

    private void ClearSlotsOccupiedByArmor(EquipmentWrapper dataWrapper, int currentSlotIndex, Equipment Equipment = null) {
        Equipment equipment = null;
        if (dataWrapper.currentEquipment[currentSlotIndex] != null) equipment = dataWrapper.currentEquipment[currentSlotIndex];
        else if (Equipment != null) equipment = Equipment;
        if (equipment == null) return;
        if (equipment.equipSlot.Length == 1) dataWrapper.currentEquipment[currentSlotIndex] = null;
        else {
            List<int> indicesToClear = new List<int>();
            foreach (var equipSlot in equipment.equipSlot) {
                indicesToClear.Add((int)equipSlot);
            }
            foreach (int index in indicesToClear) {
                dataWrapper.currentEquipment[index] = null;
            }
        }
    }

    // if (dataWrapper.currentEquipment[correctIndex].equipSlot.Length > 1)

    private bool IsTwoHandedWeapon(Weapon weapon) {
        return weapon.weaponType == WeaponType.TwoHandedSword ||
               weapon.weaponType == WeaponType.TwoHandedSpear ||
               weapon.weaponType == WeaponType.TwoHandedAxe ||
               weapon.weaponType == WeaponType.Bow ||
               weapon.weaponType == WeaponType.MusicalInstrument;
    }

    private void HandleArmorAssignment(EquipmentWrapper dataWrapper, Equipment newEquipment) {
        if (newEquipment != null) {
            ClearOccupiedSlots(dataWrapper, newEquipment.equipSlot); // Clear slots that new equipment will occupy

            foreach (var equipSlot in newEquipment.equipSlot) {
                int correctIndex = FindCorrectSlotIndex(equipSlot);
                if (correctIndex != -1) {
                    dataWrapper.currentEquipment[correctIndex] = newEquipment;
                }
            }
        }
    }


    private void ManageCardSlots(EquipmentWrapper dataWrapper, int equipmentIndex, int maximumCardSlots) {
        for (int j = 0; j < maximumCardSlots; j++) {
            if (j % 3 == 0) {
                EditorGUILayout.BeginHorizontal();
            }

            dataWrapper.cardLists[equipmentIndex][j] = (Card)EditorGUILayout.ObjectField(dataWrapper.cardLists[equipmentIndex][j], typeof(Card), false);

            if (j % 3 == 2 || j == maximumCardSlots - 1) EditorGUILayout.EndHorizontal();
        }
    }

    private void EnsureCardSlotCapacity(EquipmentWrapper dataWrapper, int equipmentIndex, int maximumCardSlots) {
        if (dataWrapper.cardLists[equipmentIndex] == null) {
            dataWrapper.cardLists[equipmentIndex] = new List<Card>();
        }
        while (dataWrapper.cardLists[equipmentIndex].Count < maximumCardSlots) {
            dataWrapper.cardLists[equipmentIndex].Add(null);
        }
    }

    private void ManageCardSlotButtons(EquipmentWrapper dataWrapper, int equipmentIndex, Equipment equipment) {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Card Slot")) {
            equipment.maximumCardSlots++;
            EnsureCardSlotCapacity(dataWrapper, equipmentIndex, equipment.maximumCardSlots);
        }
        if (equipment.maximumCardSlots > 0 && GUILayout.Button("Remove Last Card Slot")) {
            equipment.maximumCardSlots--;
            if (dataWrapper.cardLists[equipmentIndex].Count > equipment.maximumCardSlots) {
                dataWrapper.cardLists[equipmentIndex].RemoveAt(dataWrapper.cardLists[equipmentIndex].Count - 1);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

}
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Armor")]
public class Armor : Equipment {
    public ElementId? BodyElement = null;
    public float BaseDef = 0f;
    public float BaseMDef = 0f;
    public float BaseFlee = 0f;

    void OnValidate() {
        if (equipSlot == null || equipSlot.Length == 0) return;

        for (int i = 0; i < equipSlot.Length; i++) {
            if (equipSlot.Count(x => x == equipSlot[i]) > 1) {
                // If duplicate found, adjust the value
                int nextValue = (int)equipSlot[i] + 1;
                if (nextValue >= System.Enum.GetValues(typeof(EquipmentSlot)).Length) {
                    nextValue -= 2; // Go back two steps if at the end of the enum
                }
                equipSlot[i] = (EquipmentSlot)nextValue;
            }
        }
    }
}
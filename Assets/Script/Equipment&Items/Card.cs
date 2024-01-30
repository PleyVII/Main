using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Inventory/Card")]
public class Card : EquipmentBase {

    // new public EquipmentSlot equipSlot;
    [HideInInspector] public Equipment insertedIn;
    public EquipmentSlot equippedInSlot;
    public int howManySlotsDoesItRequireDefault_1 = 1;
    public ElementId? BodyElement = null;

    public override void Use(AllObjectInformation allInfo) {
        EquipmentManager.Instance.Equip(allInfo, this);
    }

    public void StopUsing() {
        EquipmentManager.Instance.Unequip(insertedIn.equipedOn, this);
    }
}

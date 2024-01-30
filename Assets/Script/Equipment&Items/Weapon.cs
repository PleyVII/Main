using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment {
    public float WeaponDamage = 0f;
    public int weaponNumberOfHits = 1;
    public ElementId WeaponElement = ElementId.Neutral;
    public WeaponType weaponType = WeaponType.Sword;
    void OnValidate() {
        equipSlot = new EquipmentSlot[] { EquipmentSlot.Lefthand, EquipmentSlot.Righthand };
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentCondition", menuName = "ModifiersConditions/EquipmentCondition")]
public class EquipmentCondition : ConditionalModifier {
    public int optionalSpecificCountOrZeroIsAllItems = 0;
    public int OptionalSpecificCountOrZeroIsAllItems {
        get {
            if (optionalSpecificCountOrZeroIsAllItems > 0) return optionalSpecificCountOrZeroIsAllItems;
            else return ComboItemsAlwaysWillAddItselfByDefault?.Count ?? 0;
        }
    }

    //Add Equipment ScriptableObject first before adding to that Object's List
    public List<EquipmentBase> ComboItemsAlwaysWillAddItselfByDefault;
    public override bool IsConditionMet(AllObjectInformation character) {
        // Check if all linked items are equipped.
        bool allItemsEquipped = EquipmentManager.Instance.isEquippedForBonus(character, ComboItemsAlwaysWillAddItselfByDefault, OptionalSpecificCountOrZeroIsAllItems);

        // If all items are equipped and it hasn't happened yet, set DidAlreadyHappen to true and return true.
        if (allItemsEquipped && !isAlreadyHappen) {
            return true;  // Allow for the addition of bonuses.
        }
        // If not all items are equipped and it did happen, set DidAlreadyHappen to false and return true.
        else if (!allItemsEquipped && isAlreadyHappen) {
            return true;  // Allow for the removal of bonuses.
        }

        // In all other cases, return false as the condition hasn't changed.
        return false;
    }

    public override bool IsConditionMetWithCondition(AllObjectInformation character) {
        // Check if all linked items are equipped.
        bool allItemsEquipped = EquipmentManager.Instance.isEquippedForBonus(character, ComboItemsAlwaysWillAddItselfByDefault, OptionalSpecificCountOrZeroIsAllItems);

        // If all items are equipped and it hasn't happened yet, set DidAlreadyHappen to true and return true.
        if (allItemsEquipped && !isAlreadyHappen) {
            isAlreadyHappen = true;
            return true;  // Allow for the addition of bonuses.
        }
        // If not all items are equipped and it did happen, set DidAlreadyHappen to false and return true.
        else if (!allItemsEquipped && isAlreadyHappen) {
            isAlreadyHappen = false;
            return true;  // Allow for the removal of bonuses.
        }

        // In all other cases, return false as the condition hasn't changed.
        return false;
    }

    // This shouldnt be changing any isAlreadyHappen, because it is just constant check.
    public override bool ConditionCheckOnModifiersChange(AllObjectInformation character) {
        return IsConditionMet(character);
    }

}
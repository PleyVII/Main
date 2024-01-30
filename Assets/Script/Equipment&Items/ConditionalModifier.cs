using UnityEngine;

public abstract class ConditionalModifier : ScriptableObject {
    protected bool isAlreadyHappen = false;
    protected bool otherCheck = false;
    [Tooltip("It has additional check to prevent adding twice")]
    public virtual bool IsConditionMet(AllObjectInformation character) {
        //This is not really doing anything, going to be overriden, 
        //this structure by itself works only on EquipmentConditions, to be overriden for others.
        bool passedOtherCheck = otherCheck;

        // If all items are equipped and it hasn't happened yet, set DidAlreadyHappen to true and return true.
        if (passedOtherCheck && !isAlreadyHappen) {
            return true;  // Allow for the addition of bonuses.
        }
        if (!passedOtherCheck && isAlreadyHappen) {
            return true;  // Allow for the removal of bonuses.
        }

        // In all other cases, return false as the condition hasn't changed.
        return false;
    }
    public abstract bool IsConditionMetWithCondition(AllObjectInformation character);
    public abstract bool ConditionCheckOnModifiersChange(AllObjectInformation character);
    public void UseAfterApplyingAllModifiers() {
        isAlreadyHappen = true;
    }
    public void UseAfterRemovingAllModifiers() {
        isAlreadyHappen = false;
    }
}

using System;
using UnityEngine;
using DefaultElementMultipliers;
public class ElementModifiersCollection : BaseModifiersCollection<ElementId> {
    static int enumLengthReference = Enum.GetNames(typeof(ElementId)).Length;
    public float[,] flatDmgDoubleArray = new float[enumLengthReference, enumLengthReference];
    public float[,] multipliersDoubleArray = ElementMultipliers.DefaultElementMapCopy;

    public override void AddFlatModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.FlatValue == 0) return;
        if (!modifier.isUsingValue2) {
            AddFlatModifierAgainstElement((ElementId)modifier.IdValue, modifier.FlatValue);
        } else if (!modifier.isUsingOnlyValue2) {
            AddFlatModifierFromSpecificElement((ElementId)modifier.IdValue, modifier.IdValue2, modifier.FlatValue);
        } else AddFlatModifierFromElement(modifier.IdValue2, modifier.FlatValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }
    public override void AddPercentageModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.PercentageValue == 0) return;
        if (!modifier.isUsingValue2) {
            AddPercentageModifierAgainstElement((ElementId)modifier.IdValue, modifier.PercentageValue);
        } else if (!modifier.isUsingOnlyValue2) {
            AddPercentageModifierFromSpecificElement((ElementId)modifier.IdValue, (ElementId)modifier.IdValue, modifier.PercentageValue);
        } else AddPercentageModifierFromElement(modifier.IdValue2, modifier.FlatValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }
    public override void RemoveFlatModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.FlatValue == 0) return;
        if (!modifier.isUsingValue2) {
            RemoveFlatModifierAgainstElement((ElementId)modifier.IdValue, modifier.FlatValue);
        } else if (!modifier.isUsingOnlyValue2) {
            RemoveFlatModifierFromSpecificElement((ElementId)modifier.IdValue, modifier.IdValue2, modifier.FlatValue);
        } else RemoveFlatModifierFromElement(modifier.IdValue2, modifier.FlatValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }

    public override void RemovePercentageModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.PercentageValue == 0) return;
        if (!modifier.isUsingValue2) {
            RemovePercentageModifierAgainstElement((ElementId)modifier.IdValue, modifier.PercentageValue);
        } else if (!modifier.isUsingOnlyValue2) {
            RemovePercentageModifierFromSpecificElement((ElementId)modifier.IdValue, modifier.IdValue2, modifier.PercentageValue);
        } else RemovePercentageModifierFromElement(modifier.IdValue2, modifier.PercentageValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }

    public float CalculateAddedDmgFor(ElementId attackingElement, ElementId defendingElement) {
        return flatDmgDoubleArray[(int)attackingElement, (int)defendingElement];
    }

    //its literally the same function as Added damage, because its being used by subtracting as a defense, just here for clarity
    public float CalculateFlatDamageReductionFor(ElementId attackingElement, ElementId defendingElement) {
        return flatDmgDoubleArray[(int)attackingElement, (int)defendingElement];
    }

    public float CalculateMultiFor(ElementId attackingElement, ElementId defendingElement) {
        return multipliersDoubleArray[(int)attackingElement, (int)defendingElement];
    }

    public float CalculateDefensiveMultiFor(ElementId attackingElement, ElementId defendingElement) {
        float multiplierFromElements = multipliersDoubleArray[(int)defendingElement, (int)attackingElement];
        float gettingPercentageIncreaseFromBase = PercentageIncrementInRelationToDefault((int)defendingElement, attackingElement, multiplierFromElements * 100);

        return 1 - (gettingPercentageIncreaseFromBase - 1);
    }

    [Tooltip("Adds modifier that works only when attacking with specific element, it doesn't do anything for other attacking elements")]
    private void AddFlatModifierFromSpecificElement(ElementId attackingElement, ElementId defendingElement, float flatModifier) {
        flatDmgDoubleArray[(int)attackingElement, (int)defendingElement] += flatModifier;
    }

    private void AddFlatModifierAgainstElement(ElementId defendingElement, float flatModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            flatDmgDoubleArray[i, (int)defendingElement] += flatModifier;
        }
        AddFlatIdModifier(defendingElement, flatModifier);
    }

    private void AddFlatModifierFromElement(ElementId attackingElement, float flatModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            flatDmgDoubleArray[(int)attackingElement, i] += flatModifier;
        }
        AddFlatIdModifier(attackingElement, flatModifier);
    }

    [Tooltip("Adds modifier that works only when attacking with specific element, it doesn't do anything for other attacking elements")]
    private void AddPercentageModifierFromSpecificElement(ElementId attackingElement, ElementId defendingElement, float percentageModifier) {
        float percentageIncrement = PercentageIncrementInRelationToDefault((int)attackingElement, defendingElement, percentageModifier);

        if (percentageIncrement > 0) multipliersDoubleArray[(int)attackingElement, (int)defendingElement] += percentageIncrement;
        else multipliersDoubleArray[(int)attackingElement, (int)defendingElement] -= percentageIncrement;
    }

    private void AddPercentageModifierAgainstElement(ElementId defendingElement, float percentageModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            float percentageIncrement = PercentageIncrementInRelationToDefault(i, defendingElement, percentageModifier);

            if (percentageIncrement > 0) multipliersDoubleArray[i, (int)defendingElement] += percentageIncrement;
            else multipliersDoubleArray[i, (int)defendingElement] -= percentageIncrement;
        }
        AddPercentageIdModifier(defendingElement, percentageModifier);
    }

    private void AddPercentageModifierFromElement(ElementId attackingElement, float percentageModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            float percentageIncrement = PercentageIncrementInRelationToDefault((int)attackingElement, (ElementId)i, percentageModifier);

            if (percentageIncrement > 0) multipliersDoubleArray[(int)attackingElement, i] += percentageIncrement;
            else multipliersDoubleArray[(int)attackingElement, i] -= percentageIncrement;
        }
        AddPercentageIdModifier(attackingElement, percentageModifier);
    }

    [Tooltip("Removes modifier that works only when attacking with specific element, it doesn't do anything for other attacking elements")]
    private void RemoveFlatModifierFromSpecificElement(ElementId attackingElement, ElementId defendingElement, float flatModifier) {
        flatDmgDoubleArray[(int)attackingElement, (int)defendingElement] -= flatModifier;
    }

    private void RemoveFlatModifierAgainstElement(ElementId defendingElement, float flatModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            flatDmgDoubleArray[i, (int)defendingElement] -= flatModifier;
        }
        RemoveFlatIdModifier(defendingElement, flatModifier);
    }

    private void RemoveFlatModifierFromElement(ElementId attackingElement, float flatModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            flatDmgDoubleArray[(int)attackingElement, i] -= flatModifier;
        }
        RemoveFlatIdModifier(attackingElement, flatModifier);
    }

    [Tooltip("Removes modifier that works only when attacking with specific element, it doesn't do anything for other attacking elements")]
    private void RemovePercentageModifierFromSpecificElement(ElementId attackingElement, ElementId defendingElement, float percentageModifier) {
        float percentageIncrement = PercentageIncrementInRelationToDefault((int)attackingElement, defendingElement, percentageModifier);

        if (percentageIncrement > 0) multipliersDoubleArray[(int)attackingElement, (int)defendingElement] -= percentageIncrement;
        else multipliersDoubleArray[(int)attackingElement, (int)defendingElement] += percentageIncrement;
    }


    private void RemovePercentageModifierAgainstElement(ElementId defendingElement, float percentageModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            float percentageIncrement = PercentageIncrementInRelationToDefault(i, defendingElement, percentageModifier);

            if (percentageIncrement > 0) multipliersDoubleArray[i, (int)defendingElement] -= percentageIncrement;
            else multipliersDoubleArray[i, (int)defendingElement] += percentageIncrement;
        }
        RemovePercentageIdModifier(defendingElement, percentageModifier);
    }

    private void RemovePercentageModifierFromElement(ElementId attackingElement, float percentageModifier) {
        for (int i = 0; i < enumLengthReference; i++) {
            float percentageIncrement = PercentageIncrementInRelationToDefault((int)attackingElement, (ElementId)i, percentageModifier);

            if (percentageIncrement > 0) multipliersDoubleArray[(int)attackingElement, i] -= percentageIncrement;
            else multipliersDoubleArray[(int)attackingElement, i] += percentageIncrement;
        }
        RemovePercentageIdModifier(attackingElement, percentageModifier);
    }




    [Tooltip("That value is to relation to default global elemental multipliers, making sure 10 percentage will add and substract the same amount")]
    public float PercentageIncrementInRelationToDefault(int attackingElement, ElementId defendingElement, float percentageModifier) {
        float modifierValue = ElementMultipliers.GetDefaultElementMultiplier((ElementId)attackingElement, defendingElement) * (percentageModifier / 100f); return modifierValue;
    }
}
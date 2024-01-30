using System;

[Serializable]
public class StatModifiers : BaseModifiers<StatId> {
    public StatModifiers(StatId whatId) : base(whatId) { }
    public StatModifiers otherStatToShare;
    float thisStatProportionToUse = -1f;
    float otherStatProportionToUse = -1f;
    public Func<float, float> AddBaseValueFormula;
    public Func<float, float> AddFinishingFormula;
    public void ChangeBaseValue(float flatmodifier) {
        baseValue = flatmodifier;
    }

    public void AddToBaseValue(float flatmodifier) {
        baseValue += flatmodifier;
    }

    public float Value {
        get {
            float finalValue = baseValue;
            if (AddBaseValueFormula != null) finalValue = AddBaseValueFormula(finalValue);
            float finalPercentageValue = percentageBaseValue;
            if (percentageToBaseValueModifiers != null) percentageToBaseValueModifiers.ForEach(x => finalValue += x);
            if (flatModifiers != null) flatModifiers.ForEach(x => finalValue += x);
            if (percentageModifiers != null) percentageModifiers.ForEach(y => finalPercentageValue += y);
            finalValue = finalValue * (1 + finalPercentageValue / 100);
            if (AddFinishingFormula != null) finalValue = AddFinishingFormula(finalValue);
            return finalValue;
        }
    }

    public float DetectsWhichValueToReturn() {
        // If there's an alternate stat reference, return that other stat value instead of the original one
        if (otherStatToShare != null) {
            if (thisStatProportionToUse >= 0 && otherStatProportionToUse >= 0) {
                return (thisStatProportionToUse / 100 * Value + otherStatProportionToUse / 100 * otherStatToShare.Value) / 2;
            } else return otherStatToShare.Value;
        }

        // If total value ignoring all stats is not zero, return that fixed value instead of the original one
        if (totalValueIgnoringAllTheStats != 0)
            return totalValueIgnoringAllTheStats;

        // Otherwise, return the standard calculated value
        return Value;
    }

    public void UseOtherStatReference(StatModifiers someOtherStatReference, float thisPersonStatProportion = -1f, float otherPersonStatProportion = -1f) {
        otherStatToShare = someOtherStatReference;
        thisStatProportionToUse = thisPersonStatProportion;
        otherStatProportionToUse = otherPersonStatProportion;
    }

    public void StopUsingOtherStatReference() {
        otherStatToShare = null;
        thisStatProportionToUse = -1f;
        otherStatProportionToUse = -1f;
    }
}
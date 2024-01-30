using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BaseModifiers<T_Id> where T_Id : Enum {
    public T_Id Id;
    public BaseModifiers(T_Id whatId) {
        Id = whatId;
    }
    //not Initialising the lists due to big number of Stat variables, to save memory, but requires null checks.
    protected List<float> percentageToBaseValueModifiers { get; private set; }
    protected List<float> flatModifiers { get; private set; }
    protected List<float> percentageModifiers { get; private set; }
    [SerializeField] public float baseValue { get; protected set; }
    public float totalValueIgnoringAllTheStats = 0;
    public float percentageBaseValue { get; private set; }

    public float GetFlatModifier() {
        float finalFlatModifier = 0;
        if (flatModifiers != null) flatModifiers.ForEach(x => finalFlatModifier += x);
        return finalFlatModifier;
    }

    public float GetMultiplierModifier() {
        float finalMultiplier = 100;
        if (percentageModifiers != null) percentageModifiers.ForEach(x => finalMultiplier += x);
        return finalMultiplier / 100;
    }
    public float GetPercentageSumValue() {
        float finalPercentageSum = 0f;
        if (percentageModifiers != null) percentageModifiers.ForEach(x => finalPercentageSum += x);
        return finalPercentageSum;
    }

    public void AddFlatModifier(float flatmodifier) {
        if (flatmodifier == 0) return;
        if (flatModifiers == null) flatModifiers = new List<float>();
        flatModifiers.Add(flatmodifier);
    }

    public void AddPercentageModifier(float percentagemodifier) {
        if (percentagemodifier == 0) return;
        if (percentageModifiers == null) percentageModifiers = new List<float>();
        percentageModifiers.Add(percentagemodifier);
    }

    public void AddPercentageToBaseValueModifier(float percentagetobasevaluemodifier) {
        if (percentagetobasevaluemodifier == 0) return;
        if (percentageToBaseValueModifiers == null) percentageToBaseValueModifiers = new List<float>();
        percentageModifiers.Add(percentagetobasevaluemodifier);
    }

    public void RemoveFlatModifier(float flatmodifier) {
        if (flatmodifier == 0) return;
        if (flatModifiers == null) flatModifiers = new List<float>();
        flatModifiers.Remove(flatmodifier);
        if (percentageModifiers.Count == 0) percentageModifiers = null;
    }

    public void RemovePercentageModifier(float percentagemodifier) {
        if (percentagemodifier == 0) return;
        if (percentageModifiers == null) percentageModifiers = new List<float>();
        percentageModifiers.Remove(percentagemodifier);
        if (percentageModifiers.Count == 0) percentageModifiers = null;
    }

    public void RemovePercentageToBaseValueModifier(float percentagetobasevaluemodifier) {
        if (percentagetobasevaluemodifier == 0) return;
        if (percentageToBaseValueModifiers == null) percentageToBaseValueModifiers = new List<float>();
        percentageModifiers.Remove(percentagetobasevaluemodifier);
        if (percentageModifiers.Count == 0) percentageModifiers = null;
    }
}
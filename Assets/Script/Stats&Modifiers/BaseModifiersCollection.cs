using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class BaseModifiersCollection<T_Id> : IModifiersCollection where T_Id : Enum {
    private AllObjectInformation owner;
    public AllObjectInformation Owner { 
        get { return owner; }
        set { if (owner == null) owner = value; }
    }

    protected List<BaseModifiers<T_Id>> allCurrentModifiers { get; private set; } = new List<BaseModifiers<T_Id>>();

    public virtual void AddFlatModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.FlatValue == 0) return;
        AddFlatIdModifier((T_Id)modifier.IdValue, modifier.FlatValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }

    public virtual void AddPercentageModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.PercentageValue == 0) return;
        AddPercentageIdModifier((T_Id)modifier.IdValue, modifier.PercentageValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }

    public virtual void RemoveFlatModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.FlatValue == 0) return;
        RemoveFlatIdModifier((T_Id)modifier.IdValue, modifier.FlatValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }

    public virtual void RemovePercentageModifier(EquipmentBase.EquipmentModifier modifier) {
        if (modifier.PercentageValue == 0) return;
        RemovePercentageIdModifier((T_Id)modifier.IdValue, modifier.PercentageValue);
        EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
    }

    public float CalculateAddedDmgFor<T_specificId>(T_specificId specificId) where T_specificId : Enum {
        if (allCurrentModifiers == null || allCurrentModifiers.Count == 0) return 0f;
        return allCurrentModifiers.FirstOrDefault(x => x.Id.Equals(specificId))?.GetFlatModifier() ?? 0f;
    }

    //its literally the same function as Added damage, because its being used by subtracting as a defense, just here for clarity
    public float CalculateFlatDamageReductionFor<T_specificId>(T_specificId specificId) where T_specificId : Enum {
        if (allCurrentModifiers == null || allCurrentModifiers.Count == 0) return 0f;
        return allCurrentModifiers.FirstOrDefault(x => x.Id.Equals(specificId))?.GetFlatModifier() ?? 0f;
    }

    public float CalculatePercentageSumFor<T_specificId>(T_specificId specificId) where T_specificId : Enum {
        if (allCurrentModifiers == null || allCurrentModifiers.Count == 0) return 0f;
        return allCurrentModifiers.FirstOrDefault(x => x.Id.Equals(specificId))?.GetMultiplierModifier() - 1 ?? 0f; ;
    }

    public float CalculateMultiFor<T_specificId>(T_specificId specificId) where T_specificId : Enum {
        if (allCurrentModifiers == null || allCurrentModifiers.Count == 0) return 1f;
        return allCurrentModifiers.FirstOrDefault(x => x.Id.Equals(specificId))?.GetMultiplierModifier() ?? 1f; ;
    }

    public float CalculateDefensiveMultiFor<T_specificId>(T_specificId specificId) where T_specificId : Enum {
        if (allCurrentModifiers == null || allCurrentModifiers.Count == 0) return 1f;
        float finalPercentageSum = allCurrentModifiers.FirstOrDefault(x => x.Id.Equals(specificId))?.GetPercentageSumValue() ?? 0f;
        return 1 - finalPercentageSum / 100;
    }

    public void AddFlatIdModifier(T_Id specificId, float flatmodifier) {
        if (flatmodifier != 0) GetOrAddIdModifier(specificId).AddFlatModifier(flatmodifier);
    }

    public void AddPercentageIdModifier(T_Id specificId, float percentageModifier) {
        if (percentageModifier != 0) GetOrAddIdModifier(specificId).AddPercentageModifier(percentageModifier);
    }

    public void RemoveFlatIdModifier(T_Id specificId, float flatModifier) {
        if (flatModifier != 0) LocateToRemoveModifier(specificId).RemoveFlatModifier(flatModifier);
    }

    public void RemovePercentageIdModifier(T_Id specificId, float percentageModifier) {
        if (percentageModifier != 0) LocateToRemoveModifier(specificId).RemoveFlatModifier(percentageModifier);
    }

    [Tooltip("Checks if on the list, if not, adds it, initializes it, and in the end returns that specific list IdModifiers variable")]
    public BaseModifiers<T_Id> GetOrAddIdModifier(T_Id specificId) {
        BaseModifiers<T_Id> specificIdModifiers = allCurrentModifiers.FirstOrDefault(x => x.Id.Equals(specificId));
        if (specificIdModifiers != null) return specificIdModifiers;
        specificIdModifiers = new BaseModifiers<T_Id>(specificId);
        allCurrentModifiers.Add(specificIdModifiers);
        return specificIdModifiers;
    }

    public BaseModifiers<T_Id> LocateToRemoveModifier(T_Id specificId) {
        BaseModifiers<T_Id> specificIdModifiers = allCurrentModifiers.FirstOrDefault(x => x.Id.Equals(specificId));
        if (specificIdModifiers == null) Debug.LogError($"IdModifier for {specificId} should already exist.");
        return specificIdModifiers;
    }
}
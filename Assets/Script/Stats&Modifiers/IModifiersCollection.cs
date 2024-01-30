using System;
using UnityEngine;
public interface IModifiersCollection {
    AllObjectInformation Owner { get; set; }
    public void AddFlatModifier(EquipmentBase.EquipmentModifier modifier);
    public void AddPercentageModifier(EquipmentBase.EquipmentModifier modifier);
    public void RemoveFlatModifier(EquipmentBase.EquipmentModifier modifier);
    public void RemovePercentageModifier(EquipmentBase.EquipmentModifier modifier);

    public float CalculateAddedDmgFor<T_specificId>(T_specificId specificId) where T_specificId : Enum {
        Debug.Log("Do not use this for Stats, use GetStatValueByStatId instead, if its not for Stats, fill it in new code");
        return 0f;
    }

    public float CalculatePercentageSumFor<T_specificId>(T_specificId specificId) where T_specificId : Enum {
        Debug.Log("Do not use this for Stats, use GetStatValueByStatId instead, if its not for Stats, fill it in new code");
        return 0f;
    }

    public float GetStatValueByStatId(StatId id) {
        Debug.Log("Do not use this for for anything else than Stats, use CalculateAddedDmgFor or CalculatePercentageSumFor instead, else, fill it in new code");
        return 0f;
    }
}
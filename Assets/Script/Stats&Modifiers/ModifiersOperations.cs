using System;

public static class ModifiersOperations {
    public static void AddFlatModifier(EquipmentBase.EquipmentModifier modifier, AllObjectInformation allInfo) {
        Switch(modifier, allInfo).AddFlatModifier(modifier);
    }

    public static void AddPercentageModifier(EquipmentBase.EquipmentModifier modifier, AllObjectInformation allInfo) {
        Switch(modifier, allInfo).AddPercentageModifier(modifier);
    }

    public static void RemoveFlatModifier(EquipmentBase.EquipmentModifier modifier, AllObjectInformation allInfo) {
        Switch(modifier, allInfo).RemoveFlatModifier(modifier);
    }

    public static void RemovePercentageModifier(EquipmentBase.EquipmentModifier modifier, AllObjectInformation allInfo) {
        Switch(modifier, allInfo).RemovePercentageModifier(modifier);
    }

    public static IModifiersCollection Switch(EquipmentBase.EquipmentModifier modifier, AllObjectInformation allInfo) {
        switch (modifier.Id) {
            case EnumType.StatId: return allInfo.Stats;

            case EnumType.SizeId:
                if (modifier.isMagicalModifier) return allInfo.SizeMagicalModifiers;
                else if (modifier.isDefensiveModifier) return allInfo.SizeDefense;
                else return allInfo.SizePhysicalModifiers;

            case EnumType.RaceId:
                if (modifier.isMagicalModifier) return allInfo.RaceMagicalModifiers;
                else if (modifier.isDefensiveModifier) return allInfo.RaceDefense;
                else return allInfo.RacePhysicalModifiers;

            case EnumType.RankId:
                if (modifier.isMagicalModifier) return allInfo.RankMagicalModifiers;
                else if (modifier.isDefensiveModifier) return allInfo.RankDefense;
                else return allInfo.RankPhysicalModifiers;

            case EnumType.ElementId:
                if (modifier.isDefensiveModifier) return allInfo.ElementDefense; else return allInfo.ElementModifiers;

            case EnumType.EffectId:
                if (modifier.isDefensiveModifier) return allInfo.EffectDefense; else return allInfo.EffectModifiers;

            case EnumType.ConditionalId:
                if (modifier.isDefensiveModifier) return allInfo.ConditionalDefense; else return allInfo.ConditionalModifiers;

            default:
                throw new ArgumentException($"Unsupported EnumType: {modifier.Id}");
        }
    }
}

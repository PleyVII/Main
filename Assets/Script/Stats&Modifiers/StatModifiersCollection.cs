using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

[Serializable]
public class StatModifiersCollection : IModifiersCollection {

    #region Owner
    private AllObjectInformation owner;
    private bool isOwnerSet = false;
    public AllObjectInformation Owner {
        get { return owner; }
        set {
            if (!isOwnerSet) {
                owner = value;
                isOwnerSet = true;  // Mark as set so it can't be changed again
            } else {
                throw new InvalidOperationException("Owner can only be set once.");
            }
        }
    }
    #endregion

    [HideInInspector]
    private StatModifiers str, agi, vit, spec, crit, _int, dex, def, mDef, atk, aspd, flee, matk, hit, critChance, softMDef, softDef, sPoints, maxHealth, healthRegen, maxMana, manaRegen, weaponAtk, weaponRange;
    private float characterLvl = 1, jobLvl = 1;
    [HideInInspector] public float currentAutoAttackCooldown = 3, currentAbilityCooldown = 3;
    [HideInInspector] private int countDown = 0;
    [HideInInspector] private bool afterAttack = false;
    [HideInInspector] private bool afterAbility = false;
    [HideInInspector]
    public bool AfterAttack {
        get { return afterAttack; }
        set {
            if (value == true && afterAbility == true) {
                if (countDown > 0 && countDown < 5) countDown += 1;
                afterAbility = false;
            } else countDown = 0;
            afterAttack = value;
        }
    }

    public bool AfterAbility {
        get { return afterAbility; }
        set {
            if (value == true && afterAttack == true) {
                if (countDown > 0 && countDown < 5) countDown += 1;
                afterAttack = false;
            } else countDown = 0;
            afterAbility = value;
        }
    }

    [HideInInspector]
    public float AttackCooldown {
        get {
            if (afterAttack) return 1 / AttackRate;
            else return (1 - (0.10f * countDown)) / AttackRate;
        }
    }

    [HideInInspector]
    public float AbilityCooldown {
        get {
            if (afterAbility) return 1 / AttackRate;
            else return (1 - (0.10f * countDown)) / AttackRate;
        }
    }

    [HideInInspector]
    public float AtkPerPointOfStrength = 2f, AtkPer10PointsOfStrength = 1f, AspdPerPointOfAgility = 1f, FleePerPointOfAgility = 1f, MatkPerPointOfInt = 2f, MatkPer10PointsOfInt = 1f,
    HitPerPointOfDex = 1f, CastTimePerPointOfDex = 1f, CritChancePerPointOfCrit = 1f, SoftMDefPerPointOfInt = 3f, SoftDefPerPointOfVit = 3f, MaxManaBaseMultiplier = 1f,
    HealthRegenMaxHealthScaling = 1f, HealthRegenPerPointOfVit = 4f, ManaRegenMaxManaScaling = 1f, ManaRegenPerPointOfInt = 1f;
    [HideInInspector] public int weaponNumberOfHits = 1;
    [SerializeField]
    public float allPointsUsed { get; private set; } = 0;
    [SerializeField]
    public float CharacterLvl {
        get { return characterLvl; }
        set { characterLvl = Mathf.Clamp(value, 0, 80); }
    }

    [SerializeField]
    public float JobLvl {
        get { return jobLvl; }
        set { jobLvl = Mathf.Clamp(value, 0, 80); }
    }

    #region Stat Modifiers functions
    public void AddFlatModifier(Equipment.EquipmentModifier modifier) {
        if (modifier.FlatValue != 0) {
            dictionaryOfAllStatVariables[(StatId)modifier.IdValue].Modifiers.AddFlatModifier(modifier.FlatValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }

    public void AddPercentageModifier(Equipment.EquipmentModifier modifier) {
        if (modifier.PercentageValue != 0) {
            dictionaryOfAllStatVariables[(StatId)modifier.IdValue].Modifiers.AddPercentageModifier(modifier.PercentageValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }

    public void RemoveFlatModifier(Equipment.EquipmentModifier modifier) {
        if (modifier.FlatValue != 0) {
            dictionaryOfAllStatVariables[(StatId)modifier.IdValue].Modifiers.RemoveFlatModifier(modifier.FlatValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }

    public void RemovePercentageModifier(Equipment.EquipmentModifier modifier) {
        if (modifier.PercentageValue != 0) {
            dictionaryOfAllStatVariables[(StatId)modifier.IdValue].Modifiers.RemovePercentageModifier(modifier.PercentageValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }
    public void AddFlatModifier(float flatValue, StatId stat) {
        if (flatValue != 0) {
            dictionaryOfAllStatVariables[stat].Modifiers.AddFlatModifier(flatValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }

    public void AddPercentageModifier(float percentageValue, StatId stat) {
        if (percentageValue != 0) {
            dictionaryOfAllStatVariables[stat].Modifiers.AddPercentageModifier(percentageValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }

    public void RemoveFlatModifier(float flatValue, StatId stat) {
        if (flatValue != 0) {
            dictionaryOfAllStatVariables[stat].Modifiers.RemoveFlatModifier(flatValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }

    public void RemovePercentageModifier(float percentageValue, StatId stat) {
        if (percentageValue != 0) {
            dictionaryOfAllStatVariables[stat].Modifiers.RemovePercentageModifier(percentageValue);
            EquipmentManager.Instance.RecheckingConditionsOnModifierChange(Owner);
        }
    }
    #endregion Stat Modifiers functions

    #region Stat Dictionary and its Initialization
    public Dictionary<StatId, (StatModifiers Modifiers, float Value)> dictionaryOfAllStatVariables;

    public float GetStatValueByStatId(StatId id) {
        return dictionaryOfAllStatVariables[id].Value;
    }

    public StatModifiersCollection() {
        dictionaryOfAllStatVariables = new Dictionary<StatId, (StatModifiers, float)>
        {
            { StatId.Str, (str = new StatModifiers(StatId.Str), Str) },
            { StatId.Agi, (agi = new StatModifiers(StatId.Agi), Agi) },
            { StatId.Vit, (vit = new StatModifiers(StatId.Vit), Vit) },
            { StatId.Spec, (spec = new StatModifiers(StatId.Spec), Spec) },
            { StatId.Crit, (crit = new StatModifiers(StatId.Crit), Crit) },
            { StatId.Int, (_int = new StatModifiers(StatId.Int), Int) },
            { StatId.Dex, (dex = new StatModifiers(StatId.Dex), Dex) },
            { StatId.Def, (def = new StatModifiers(StatId.Def), Def) },
            { StatId.MDef, (mDef = new StatModifiers(StatId.MDef), MDef) },
            { StatId.Atk, (atk = new StatModifiers(StatId.Atk), Atk) },
            { StatId.Aspd, (aspd = new StatModifiers(StatId.Aspd), Aspd) },
            { StatId.Flee, (flee = new StatModifiers(StatId.Flee), Flee) },
            { StatId.Matk, (matk = new StatModifiers(StatId.Matk), Matk) },
            { StatId.Hit, (hit = new StatModifiers(StatId.Hit), Hit) },
            { StatId.CritChance, (critChance = new StatModifiers(StatId.CritChance), CritChance) },
            { StatId.SoftMDef, (softMDef = new StatModifiers(StatId.SoftMDef), SoftMDef) },
            { StatId.SoftDef, (softDef = new StatModifiers(StatId.SoftDef), SoftDef) },
            { StatId.SPoints, (sPoints = new StatModifiers(StatId.SPoints), SPoints) },
            { StatId.MaxHealth, (maxHealth = new StatModifiers(StatId.MaxHealth), MaxHealth) },
            { StatId.HealthRegen, (healthRegen = new StatModifiers(StatId.HealthRegen), HealthRegen) },
            { StatId.MaxMana, (maxMana = new StatModifiers(StatId.MaxMana), MaxMana) },
            { StatId.ManaRegen, (manaRegen = new StatModifiers(StatId.ManaRegen), ManaRegen) },
            { StatId.WeaponAtk, (weaponAtk = new StatModifiers(StatId.WeaponAtk), WeaponAtk) },
            { StatId.WeaponRange, (weaponRange = new StatModifiers(StatId.WeaponRange), WeaponRange) }
            // ... and so on for other stats
        };
        PropertyFormulasInitialization();
    }
    #endregion Stat Dictionary and its Initialization

    #region Stat Properties
    //Do Not use Getter from here for UI, use dictionary and .baseValue instead.
    //This properties are for practical total value, and for adding 
    //Will think about changing it later
    public float Str {
        get { return Mathf.Floor(str.DetectsWhichValueToReturn()); }
        set { TryAddPoints(value, str); }
    }

    public float Agi {
        get { return Mathf.Floor(agi.DetectsWhichValueToReturn()); }
        set { TryAddPoints(value, agi); }
    }

    public float Vit {
        get { return Mathf.Floor(vit.DetectsWhichValueToReturn()); }
        set { TryAddPoints(value, vit); }
    }

    public float HealthRegen {
        get { return healthRegen.baseValue; }
        set { TryAddPoints(value, healthRegen); }
    }

    public float Spec {
        get { return Mathf.Floor(spec.DetectsWhichValueToReturn()); }
        set { TryAddPoints(value, spec); }
    }

    public float Crit {
        get { return Mathf.Floor(crit.DetectsWhichValueToReturn()); }
        set { TryAddPoints(value, crit); }
    }

    public float Int {
        get { return Mathf.Floor(_int.DetectsWhichValueToReturn()); }
        set { TryAddPoints(value, _int); }
    }

    public float ManaRegen {
        get { return manaRegen.baseValue; }
        set { TryAddPoints(value, manaRegen); }
    }

    public float Dex {
        get { return Mathf.Floor(dex.DetectsWhichValueToReturn()); }
        set { TryAddPoints(value, dex); }
    }

    public float SPoints {
        get { return sPoints.DetectsWhichValueToReturn(); }
    }

    public float WeaponAtk {
        get { return Mathf.Floor(weaponAtk.DetectsWhichValueToReturn()); }
    }

    public float WeaponRange {
        get { return Mathf.Floor(weaponRange.DetectsWhichValueToReturn()); }
    }

    public float Atk {
        get { return atk.DetectsWhichValueToReturn(); }
    }

    public float Aspd {
        get { return Mathf.Floor(aspd.DetectsWhichValueToReturn()); }
    }

    public float Flee {
        get { return Mathf.Floor(flee.DetectsWhichValueToReturn()); }
    }

    public float Matk {
        get { return Mathf.Floor(matk.DetectsWhichValueToReturn()); }
    }

    public float Hit {
        get { return Mathf.Floor(hit.DetectsWhichValueToReturn()); }
    }

    public float CritChance {
        get { return Mathf.Floor(critChance.DetectsWhichValueToReturn()); }
    }

    public float SoftMDef {
        get { return Mathf.Floor(softMDef.DetectsWhichValueToReturn()); }
    }

    public float SoftDef {
        get { return Mathf.Floor(softDef.DetectsWhichValueToReturn()); }
    }

    public float MaxHealth {
        get { return maxHealth.DetectsWhichValueToReturn(); }
        set { maxHealth.ChangeBaseValue(value); }
    }

    public float MaxHealthMultiplier {
        get { return GetHealthMultiplier(); }
    }

    public float MaxMana {
        get { return Mathf.Floor(maxMana.DetectsWhichValueToReturn()); }
    }

    public float Def {
        get { return Mathf.Floor(def.DetectsWhichValueToReturn()); }
    }

    public float MDef {
        get { return Mathf.Floor(mDef.DetectsWhichValueToReturn()); }
    }

    private void PropertyFormulasInitialization() {
        healthRegen.AddBaseValueFormula = HealthRegenPer5Formula;
        manaRegen.AddBaseValueFormula = ManaRegenPer5Formula;
        sPoints.AddBaseValueFormula = SPointsFormula;
        atk.AddBaseValueFormula = AtkFormula;
        aspd.AddBaseValueFormula = AspdFormula;
        flee.AddBaseValueFormula = FleeFormula;
        matk.AddBaseValueFormula = MatkFormula;
        hit.AddBaseValueFormula = HitFormula;
        critChance.AddBaseValueFormula = CritChanceFormula;
        softMDef.AddBaseValueFormula = SoftMDefFormula;
        softDef.AddBaseValueFormula = SoftDefFormula;
        maxHealth.AddBaseValueFormula = MaxHealthFormula;
        maxMana.AddBaseValueFormula = MaxManaFormula;
        def.AddFinishingFormula = DefOrMDefFormula;
        mDef.AddFinishingFormula = DefOrMDefFormula;
        if (maxHealth.baseValue == 0) maxHealth.ChangeBaseValue(2000f);
    }
    #endregion Stat Properties

    #region Stat Formulas for Properties
    private float AtkFormula(float Value) {
        return Value + (50 + (AtkPerPointOfStrength * Str + (Mathf.Floor(Str / 10) * AtkPer10PointsOfStrength * 30)));
    }

    private float AspdFormula(float Value) {
        return Value + 50 + (Agi * AspdPerPointOfAgility);
    }

    public float AttackRate {
        get {
            if (Aspd != 0) return Aspd / 100;
            else return 2f;
        }
    }

    private float FleeFormula(float Value) {
        return Value + characterLvl / 4 + (FleePerPointOfAgility * Agi);
    }

    private float MatkFormula(float Value) {
        return Value + 50 + (MatkPerPointOfInt * Int + (Mathf.Floor(Int / 10) * MatkPer10PointsOfInt * 30));
    }

    private float HitFormula(float Value) {
        return Value + characterLvl / 4 + (HitPerPointOfDex * Dex);
    }

    public float CastTimeMultiplier {
        get {
            if (Dex >= 150) return 0f;
            else return 1 - Dex * CastTimePerPointOfDex / 150;
        }
    }
    public float ChargeTimeMultiplier {
        get {
            if (Dex + Agi >= 300) return 0f;
            else return 1 - (Dex + Agi) / 300;
        }
    }

    private float CritChanceFormula(float Value) {
        return Value + Crit * CritChancePerPointOfCrit;
    }

    private float SoftMDefFormula(float Value) {
        return Value + Int * SoftMDefPerPointOfInt;
    }

    private float SoftDefFormula(float Value) {
        return Value + Vit * SoftDefPerPointOfVit;
    }

    private float MaxHealthFormula(float Value) {
        return (Value + Value * 0.01f * characterLvl) * MaxHealthMultiplier * (1 + Vit / 70);
    }

    private float MaxManaFormula(float Value) {
        return (Value + 400f * MaxManaBaseMultiplier + 2f * MaxManaBaseMultiplier * characterLvl) * GetManaMultiplier() * (1 + Int / 70);
    }

    private float HealthRegenPer5Formula(float Value) {
        return 10 * Value + (MaxHealth * HealthRegenMaxHealthScaling / 50) + MaxHealth * HealthRegenMaxHealthScaling / 400 * HealthRegenPerPointOfVit * Mathf.Floor(Vit / 10) / 10 * (1 + Value / 50);
    }

    private float ManaRegenPer5Formula(float Value) {
        return Value + 5 + ((MaxMana * ManaRegenMaxManaScaling / 50) + MaxMana * ManaRegenMaxManaScaling / 50 * ManaRegenPerPointOfInt * Mathf.Floor(Int / 10) / 5) * (1 + Value / 50);
    }

    public float HealthRegenPer5 {
        get { return Mathf.Floor(healthRegen.DetectsWhichValueToReturn()); }
    }

    public float ManaRegenPer5 {
        get { return Mathf.Floor(manaRegen.DetectsWhichValueToReturn()); }
    }

    private float DefOrMDefFormula(float Value) {
        return Value <= 25 ? Value * 2 :
               Value <= 50 ? Value + 25 :
               Value <= 75 ? 75 + Mathf.FloorToInt((Value - 50) / 2) :
               Value <= 100 ? 88 + Mathf.FloorToInt((Value - 76) / 4) :
                              94 + Mathf.FloorToInt((Value - 97) / 8);
    }

    #endregion Stat Formulas for Properties

    #region Weapon and Body Armor functions
    public void AddBaseValuesFromArmour(Armor equipmentModifier) {
        def.AddToBaseValue(equipmentModifier.BaseDef);
        mDef.AddToBaseValue(equipmentModifier.BaseMDef);
        flee.AddToBaseValue(equipmentModifier.BaseFlee);
        ChangeBodyElement(equipmentModifier);
    }

    public void RemoveBaseValuesFromArmour(Armor equipmentModifier) {
        def.AddToBaseValue(-equipmentModifier.BaseDef);
        mDef.AddToBaseValue(-equipmentModifier.BaseMDef);
        flee.AddToBaseValue(-equipmentModifier.BaseFlee);
        RemoveBodyElement(equipmentModifier);
    }

    public void ChangeBaseWeaponAtk(Weapon equipmentModifier) {
        weaponAtk.ChangeBaseValue(equipmentModifier.WeaponDamage);
        Owner.WeaponElement = equipmentModifier.WeaponElement;
        weaponNumberOfHits = equipmentModifier.weaponNumberOfHits;
    }

    public void RemoveBaseWeaponAtk(Weapon equipmentModifier) {
        weaponAtk.ChangeBaseValue(0);
        Owner.WeaponElement = equipmentModifier.WeaponElement;
        weaponNumberOfHits = 1;
    }
    public void ChangeBodyElement(Armor equipmentModifier) {
        if (equipmentModifier.BodyElement is ElementId BodyElement) {
            Owner.CurrentBodyElement = BodyElement;
        }
    }
    public void ChangeBodyElement(Card equipmentModifier) {
        if (equipmentModifier.BodyElement is ElementId BodyElement) {
            Owner.CurrentBodyElement = BodyElement;
        }
    }

    public void RemoveBodyElement(Armor equipmentModifier) {
        if (equipmentModifier.BodyElement is ElementId)
            Owner.CurrentBodyElement = null;
    }

    public void RemoveBodyElement(Card equipmentModifier) {
        if (equipmentModifier.BodyElement is ElementId)
            Owner.CurrentBodyElement = null;
    }
    #endregion Weapon and Body Armor functions

    #region Stat Points functions
    private void TryAddPoints(float valueToAdd, StatModifiers stat) {
        while (valueToAdd > 0) {
            int pointsToAdd = CalculatePointsChange(stat.baseValue);
            if (SPoints - pointsToAdd < 0) break;
            UsePoints(pointsToAdd);
            stat.AddToBaseValue(1);
            valueToAdd--;
        }

        while (valueToAdd < 0) {
            if (stat.baseValue - 1 < 0) break;
            int pointsToRemove = CalculatePointsChange(stat.baseValue - 1);
            UsePoints(-pointsToRemove);
            stat.AddToBaseValue(-1);
            valueToAdd++;
        }
    }

    public void UsePoints(float usedPoints) {
        allPointsUsed += usedPoints;
    }

    private int CalculatePointsChange(float baseStat) {
        if (baseStat < 90) return Mathf.FloorToInt((5 + baseStat) / 5);
        else if (baseStat < 110) return Mathf.FloorToInt((baseStat - 52) / 2);
        else return Mathf.FloorToInt((baseStat - 81) / 1);
    }

    private float SPointsFormula(float Value) {
        float statVariable = 400 + Value;
        for (int i = 0; i < characterLvl; i++) {
            statVariable += (7 + Mathf.Floor(i / 20)) * 4;
        }

        int endValue = Mathf.FloorToInt(statVariable - allPointsUsed);
        if (endValue < 0) Debug.LogError("That shouldn't ever happen, something took away more points that it should");
        return endValue;
    }
    #endregion Stat Points functions

    #region Character Classes and its Health and Mana Multipliers
    public CharacterClass characterClass;
    private float GetHealthMultiplier() {
        switch (characterClass) {
            case CharacterClass.None:
                return 3f;
            case CharacterClass.Novice:
            case CharacterClass.Mage:
            case CharacterClass.Acolyte:
            case CharacterClass.Archimage:
            case CharacterClass.Scholar:
            case CharacterClass.Bishop:
            case CharacterClass.Occultist:
                return 1.4f;
            case CharacterClass.Chemist:
            case CharacterClass.Archer:
            case CharacterClass.Bard:
            case CharacterClass.Sniper:
            case CharacterClass.Hunter:
            case CharacterClass.Creator:
            case CharacterClass.Zoologist:
            case CharacterClass.Enchanter:
                return 1.7f;
            case CharacterClass.Aun:
            case CharacterClass.Assassin:
            case CharacterClass.Scoundrel:
            case CharacterClass.Reaper:
            case CharacterClass.Blacksmith:
            case CharacterClass.Thief:
                return 1.9f;
            case CharacterClass.Swordsman:
            case CharacterClass.Guardian:
            case CharacterClass.Kingsguard:
            case CharacterClass.RoyalTemplar:
                return 2.2f;
            default:
                return 1f;
        }
    }
    private float GetManaMultiplier() {
        switch (characterClass) {
            case CharacterClass.None:
                return 2f;
            case CharacterClass.Novice:
                return 1f;
            case CharacterClass.Mage:
            case CharacterClass.Archimage:
            case CharacterClass.Scholar:
            case CharacterClass.Enchanter:
                return 3f;
            case CharacterClass.Acolyte:
            case CharacterClass.Bishop:
            case CharacterClass.Occultist:
            case CharacterClass.Bard:
            case CharacterClass.Aun:
                return 2f;
            case CharacterClass.Archer:
            case CharacterClass.Sniper:
            case CharacterClass.Hunter:
            case CharacterClass.Creator:
            case CharacterClass.Zoologist:
                return 1.7f;
            case CharacterClass.Chemist:
            case CharacterClass.Assassin:
            case CharacterClass.Scoundrel:
            case CharacterClass.Reaper:
            case CharacterClass.Blacksmith:
            case CharacterClass.Thief:
                return 1.4f;
            case CharacterClass.Swordsman:
            case CharacterClass.Guardian:
            case CharacterClass.Kingsguard:
            case CharacterClass.RoyalTemplar:
                return 1.5f;
            default:
                return 1f;
        }
    }
    #endregion
}

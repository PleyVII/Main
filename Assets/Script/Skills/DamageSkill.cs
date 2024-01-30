using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageSkill", menuName = "Skills/DamageSkill")]
public abstract class DamageSkill : ActiveSkill {
    public bool isMagicDamage, isPhysicalDamage;
    public bool IsMelee, IsRanged;
    public float physicalDamage, magicDamage;
    [NonSerialized] public bool passiveSkill = false;
    [NonSerialized] public bool isABuff = false;
    public float damageMultiplierPerLevel = 1.03f;
    public ElementId? skillElementIfNotUsingWeaponElement;
    public int numberOfHits = 1;
    public float DefaultPhysDmgCalculation(AllObjectInformation attackerInfo) {
        return physicalDamage * attackerInfo.Stats.Atk * Mathf.Pow(damageMultiplierPerLevel, levelCurrent);
    }

    public float DefaultMagicDmgCalculation(AllObjectInformation attackerInfo) {
        return magicDamage * attackerInfo.Stats.Atk * Mathf.Pow(damageMultiplierPerLevel, levelCurrent);
    }
}
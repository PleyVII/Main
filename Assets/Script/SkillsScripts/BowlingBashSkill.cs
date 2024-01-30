using UnityEngine;

[CreateAssetMenu(fileName = "BowlingBash", menuName = "Skills/BowlingBashSkill")]
public class BowlingBashSkill : DamageSkill {
    public float areaOfEffectRadius;

    public float stunChance;

    public float stunDuration;

    public override TimerType_Skill TimerType_Skill { get => TimerType_Skill.NotCastableOrChargeable; }
    public override TargetingType_Skill TargetingType_Skill { get => TargetingType_Skill.TargetedAtCharacter; }

    public override void UseSkillToEdit(AllObjectInformation attacker, AllObjectInformation target = null, Vector3? whereToCast = null) {
        Collider[] hitColliders = Physics.OverlapSphere(target.Owner.transform.position, areaOfEffectRadius, LayerMask.GetMask("Interactable"));
        foreach (Collider collider in hitColliders) {
            AllObjectInformation ColiderInformation = collider.GetComponent<CharacterUpdates>().allInfo;
            ColiderInformation.DealPhysicalDamage(attacker, DefaultPhysDmgCalculation(attackerInformation), numberOfHits);
            ApplyEffect(attacker, target, stunChance * levelCurrent, stunDuration, new StunEffect());
        }
    }
}
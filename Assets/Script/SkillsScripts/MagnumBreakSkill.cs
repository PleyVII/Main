using UnityEngine;

[CreateAssetMenu(fileName = "MagnumBreak", menuName = "Skills/MagnumBreak")]
public class MagnumBreakSkill : DamageSkill {
    public float areaOfEffectRadius;

    public override TimerType_Skill TimerType_Skill { get => TimerType_Skill.NotCastableOrChargeable; }
    public override TargetingType_Skill TargetingType_Skill { get => TargetingType_Skill.NonTargeted; }

    public override void UseSkillToEdit(AllObjectInformation attacker, AllObjectInformation target = null, Vector3? whereToCast = null) {
        Collider[] hitColliders = Physics.OverlapSphere(attacker.Owner.transform.position, areaOfEffectRadius, LayerMask.GetMask("Damagable"));
        foreach (Collider collider in hitColliders) {
            if (collider.gameObject != attacker) {
                AllObjectInformation coliderInformation = collider.GetComponent<CharacterUpdates>().allInfo;
                coliderInformation.DealPhysicalDamage(attacker, DefaultPhysDmgCalculation(attacker));
            }
        }
    }
}

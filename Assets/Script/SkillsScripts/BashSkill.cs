using UnityEngine;


[CreateAssetMenu(fileName = "Bash", menuName = "Skills/Bash")]
public class BashSkill : DamageSkill {
    public float stunChance = 5;
    public float stunDuration;
    public override TimerType_Skill TimerType_Skill { get => TimerType_Skill.NotCastableOrChargeable; }
    public override TargetingType_Skill TargetingType_Skill { get => TargetingType_Skill.TargetedAtCharacter; }

    public override void UseSkillToEdit(AllObjectInformation attacker, AllObjectInformation target = null, Vector3? whereToCast = null) {
        target.DealPhysicalDamage(attacker, DefaultPhysDmgCalculation(attacker), numberOfHits);
        ApplyEffect(attacker, target, stunChance * levelCurrent, stunDuration, new StunEffect());
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Provoke", menuName = "Skills/Provoke")]
public class ProvokeSkill : BuffSkill {
    public override TimerType_Skill TimerType_Skill { get => TimerType_Skill.NotCastableOrChargeable; }
    public override TargetingType_Skill TargetingType_Skill { get => TargetingType_Skill.TargetedAtCharacter; }

    public override void UseSkillToEdit(AllObjectInformation attacker, AllObjectInformation target = null, Vector3? whereToCast = null) {
        UseBuffOfThisSkillName(attacker, target);
    }
}
//ITargetedSkills are made so they automatically just give a buff 
//Override so something else otherwise (will possibly change if there will be to many instance for )
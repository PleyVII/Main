using UnityEngine;

public class Skill : ScriptableObject {
    public string skillName;
    public int levelMax;
    public int levelLearned;
    public int levelCurrent;
    public Sprite icon;
    public SkillRequirement[] skillRequirements;
    public void OnEnable() {
        if (icon == null) icon = Resources.Load<Sprite>("TestSkillImage");
    }
}
[System.Serializable]
public class SkillRequirement {
    public Skill requiredSkill;
    public int requiredSkillLevel;
}
public class PassiveSkill : Skill {

}

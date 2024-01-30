using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkills {
    private Dictionary<Skill, int> learnedSkills = new Dictionary<Skill, int>();

    public void LearnSkill(Skill skillName, int skillLevel) {
        if (learnedSkills.ContainsKey(skillName)) {
            learnedSkills[skillName] = skillLevel; // Update existing skill level
        } else {
            learnedSkills.Add(skillName, skillLevel); // Add new skill
        }
    }

    public bool HasSkill(Skill skillName) {
        return learnedSkills.ContainsKey(skillName);
    }

    public int GetSkillLevel(Skill skillName) {
        if (learnedSkills.TryGetValue(skillName, out int skillLevel)) {
            return skillLevel;
        }
        return 0; // Skill not found, return default level (e.g., 0)
    }
}
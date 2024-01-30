using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour {
    public GameObject skillTree;
    public GameObject skillSlotPrefab;
    public Transform TheParentOfFutureSkillSlots;
    public SkillSlotOnSkillTree[] skillSlots = new SkillSlotOnSkillTree[42]; // Array to store references to the skill slots
    private void Awake() {
        for (int i = 0; i < skillSlots.Length; i++) {
            GameObject skillSlotObject = Instantiate(skillSlotPrefab, TheParentOfFutureSkillSlots);
            SkillSlotOnSkillTree skillSlot = skillSlotObject.GetComponent<SkillSlotOnSkillTree>();
            skillSlots[i] = skillSlot;
        }
        AddSkillToTree(0, "BowlingBashSkill");
        AddSkillToTree(1, "BashSkill");
        AddSkillToTree(2, "MagnumBreakSkill");
        AddSkillToTree(3, "ProvokeSkill");
    }

    public void AddSkillToTree(int slotIndex, string skillAssetName) {
        Skill loadedSkill = Instantiate(Resources.Load<Skill>("Skills/" + skillAssetName));
        Debug.Log($"Loaded Skill to the tree : {loadedSkill}");
        if (slotIndex >= 0 && slotIndex < skillSlots.Length) {
            skillSlots[slotIndex].AddSkillToTree(loadedSkill);
        } else {
            Debug.LogWarning("Invalid slot index: " + slotIndex);
        }
    }

    void Update() {
        if (Input.GetButtonDown("SkillTree")) {
            skillTree.SetActive(!skillTree.activeSelf);
        }
    }
}

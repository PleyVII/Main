using UnityEngine;

public class SkillBars : MonoBehaviour {
    public GameObject skillTree;
    public GameObject skillBarSlotPrefab;
    public Transform TheParentOfFutureSkillBarSlots;
    public SkillBarSlot[] skillBarSlots = new SkillBarSlot[9]; // Array to store references to the skill slots
    private void Awake() {
        for (int i = 0; i < skillBarSlots.Length; i++) {
            GameObject skillBarSlotObject = Instantiate(skillBarSlotPrefab, TheParentOfFutureSkillBarSlots);
            SkillBarSlot skillBarSlot = skillBarSlotObject.GetComponent<SkillBarSlot>();
            skillBarSlots[i] = skillBarSlot;
        }
    }
}

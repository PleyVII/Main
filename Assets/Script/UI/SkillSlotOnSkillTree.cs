using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillSlotOnSkillTree : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public Image icon;
    public Skill skill;
    public TextMeshProUGUI uiText;
    public Button buttonLearnSkill;
    public Button buttonChangeCurrentLevelUp;
    public Button buttonChangeCurrentLevelDown;
    public void AddSkillToTree(Skill newSkill) {
        skill = newSkill;
        icon.sprite = skill.icon;
        icon.enabled = true;
        if (skill.levelLearned > 0) {
            uiText.enabled = true;
            buttonChangeCurrentLevelUp.gameObject.SetActive(true);
            buttonChangeCurrentLevelDown.gameObject.SetActive(true);
        }
        uiText.text = skill.levelCurrent + "/" + skill.levelLearned;
        buttonLearnSkill.gameObject.SetActive(true);
    }

    public void ClearSkillOnTreeSlot() {
        skill = null;
        icon.sprite = null;
        icon.enabled = false;
        buttonLearnSkill.enabled = false;
        uiText.enabled = false;
        buttonLearnSkill.gameObject.SetActive(false);
        buttonChangeCurrentLevelUp.gameObject.SetActive(false);
        buttonChangeCurrentLevelDown.gameObject.SetActive(false);
    }
    public void AddALevel() {
        if (skill.levelLearned < skill.levelMax) {
            if (skill.levelLearned == 0) {
                uiText.enabled = true;
                buttonChangeCurrentLevelUp.gameObject.SetActive(true);
                buttonChangeCurrentLevelDown.gameObject.SetActive(true);
            }
            skill.levelLearned += 1;
            if (skill.levelCurrent == skill.levelLearned - 1) skill.levelCurrent = skill.levelLearned;
            //also check the skill points and if you can add it
            uiText.text = skill.levelCurrent + "/" + skill.levelLearned;
        }
    }
    public void ChangeCurrentLevelUp() {
        if (skill.levelCurrent < skill.levelLearned) {
            skill.levelCurrent += 1;
            uiText.text = skill.levelCurrent + "/" + skill.levelLearned;
        }
    }
    public void ChangeCurrentLevelDown() {
        if (skill.levelCurrent > 1) {
            skill.levelCurrent -= 1;
            uiText.text = skill.levelCurrent + "/" + skill.levelLearned;
        }
    }
    private SkillBarSlot targetSlot; // Reference to the skill slot on the skill bar
    private GameObject draggableSkillObject; // Reference to the instantiated draggable skill object

    public void OnBeginDrag(PointerEventData eventData) {
        if (skill is PassiveSkill) return;
        // Instantiate the draggable skill object
        draggableSkillObject = Instantiate(this.gameObject, transform.parent);

        // Set the position and image of the draggable skill object
        draggableSkillObject.transform.position = transform.position;
        draggableSkillObject.GetComponent<Image>().sprite = GetComponent<Image>().sprite;

        // Disable raycast blocking on the original skill slot
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (skill is PassiveSkill) return;
        // Update the position of the draggable skill object while dragging
        draggableSkillObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (skill is PassiveSkill) return;
        // Destroy the draggable skill object
        Destroy(draggableSkillObject);

        // Enable raycast blocking on the original skill slot
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Check if a valid target slot was found
        if (targetSlot != null) {
            // Assign the skill to the target slot
            targetSlot.AssignSkill(GetComponent<Skill>());
        }
    }

    public void SetTargetSlot(SkillBarSlot slot) {
        targetSlot = slot;
    }
}

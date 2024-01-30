using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillBarSlot : MonoBehaviour, IDropHandler {
    public Image slotImage;
    public Skill currentSkill;
    public TextMeshProUGUI uiText;
    public void OnDrop(PointerEventData eventData) {
        // Get the dragged skill from the event data
        Skill draggedSkill = Instantiate(eventData.pointerDrag.GetComponent<SkillSlotOnSkillTree>().skill);
        if (draggedSkill != null && !(draggedSkill is PassiveSkill)) {
            // Assign the dragged skill to this slot
            AssignSkill(draggedSkill);
            Debug.Log("Right After AssignSkill happens");
        }
    }

    public void AssignSkill(Skill skill) {
        currentSkill = skill;
        slotImage.sprite = skill.icon;
        uiText.text = skill.levelCurrent.ToString();
        // Update the slot's image or any other visual representation
        // based on the assigned skill
        uiText.enabled = true;
    }

    public void RemoveSkill() {
        currentSkill = null;
        // Reset the slot's image or any other visual representation
        slotImage.sprite = Resources.Load<Sprite>("none");
        uiText.enabled = false;
    }
}
using UnityEngine;
public abstract class ActiveSkill : Skill {
    public abstract TimerType_Skill TimerType_Skill { get; }
    public abstract TargetingType_Skill TargetingType_Skill { get; }
    public float Range;
    //     public float Range {
    //     get {
    //         if (targetTransform != null)
    //             return Vector3.Distance(transform.position, targetTransform.position);
    //         else if (targetGround != null) return Vector3.Distance(transform.position, (Vector3)targetGround);
    //         else return null;
    //     }
    // }
    public float manaBaseCost;
    public float manaMultiplierPerLevel = 1.2f;
    public float useTimer;
    public AllObjectInformation attackerInformation;
    public AllObjectInformation targetInfo;

    public void UseSkill(AllObjectInformation attacker, AllObjectInformation target = null, Vector3? whereToCast = null) {
        if (!HasEnoughMana(attacker)) return;
        UseSkillToEdit(attacker, target, whereToCast);
        Debug.Log($"Using Skill {this}");
    }

    public abstract void UseSkillToEdit(AllObjectInformation attacker, AllObjectInformation target = null, Vector3? whereToCast = null);

    protected bool HasEnoughMana(AllObjectInformation Attacker) {
        float manaCost = manaBaseCost + (manaBaseCost * Mathf.Pow(manaMultiplierPerLevel, levelCurrent));
        Debug.Log("Current mana:" + Attacker.CurrentMana);
        if (Attacker.CurrentMana - manaCost >= 0) {
            Attacker.CurrentMana -= manaCost; return true;
        } else {
            Debug.Log("Not enough mana stupid"); return false;
        }
    }

    protected void ApplyEffect(AllObjectInformation attacker, AllObjectInformation target, float percentChance, float duration, Effect effect) {
        if (Random.value <= percentChance) {
            EffectsManager.Instance.AddEffect(effect, attacker, target);
            Debug.Log($"Applied {effect} to target for {duration} seconds");
        } else {
            Debug.Log($"Random chance for {effect} not met.");
        }
    }
}


// State savedState = 0;
// protected void SaveTargetState(AllObjectInformation target) {
//     savedState = target.CurrentState;
// }

// protected void AddStatus<T>(AllObjectInformation attacker, AllObjectInformation target, State interestedState, float chance, int duration, State savedState) where T : Effect, new() {
//     if ((savedState & interestedState) == 0) return;
//     if ((target.CurrentState & interestedState) != 0) return;
//     if (Random.value <= chance) {
//         T effect = new T { Duration = duration };
//         EffectsManager.Instance.AddEffect(effect, attacker, target);                      MAYBE have it for later, to prevent ability to chain freeze somebody etc.
//         Debug.Log($"Applied {typeof(T).Name} to target for {duration} seconds");
//     } else {
//         Debug.Log($"Random chance for {typeof(T).Name} not met.");
//     }
// }



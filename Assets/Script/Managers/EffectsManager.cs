using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {
    #region Singleton
    private static EffectsManager instance;
    public static EffectsManager Instance {
        get {
            if (instance == null) Debug.Log("There is no Singleton of Status Effects Manager Instance Yo");
            return instance;
        }
    }

    void Awake() {
        if (instance != null) Debug.LogWarning("There was more than one Singleton of Status Effects Manager Instance Yo");
        instance = this;
    }

    #endregion
    public Dictionary<AllObjectInformation, List<Effect>> AllEffects = new Dictionary<AllObjectInformation, List<Effect>>();

    void Update() {
        UpdateAndRemoveEffects(Time.deltaTime);
    }

    public void UpdateAndRemoveEffects(float updateSubstraction = 0f) {
        float timePassed = updateSubstraction;
        foreach (KeyValuePair<AllObjectInformation, List<Effect>> entry in AllEffects) {
            foreach (Effect effect in entry.Value) {
                effect.UpdateOrAndRemoveInactiveEffects(AllEffects, timePassed);
            }
        }
    }
    [Tooltip("Create new instance before adding Effect variable")]
    public void AddEffect(Effect effect, AllObjectInformation giver, AllObjectInformation receiver) {
        effect.AddEffect(giver, receiver, AllEffects);
    }

    public void AddEffect(Effect effect, AllObjectInformation giver, AllObjectInformation receiver, int levelOfASkill) {
        effect.AddEffect(giver, receiver, AllEffects, levelOfASkill);
    }

    public void RemoveSpecificEffect<T_Effect>(AllObjectInformation receiver) where T_Effect : Effect {
        if (AllEffects.ContainsKey(receiver)) {
            for (int i = AllEffects[receiver].Count - 1; i >= 0; i--) {
                if (AllEffects[receiver][i] is T_Effect)
                    AllEffects[receiver][i].DeactivateEffect(AllEffects);
            }
        }
    }
}
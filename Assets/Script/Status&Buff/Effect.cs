using System.Collections.Generic;

public abstract class Effect : IEffect {
    public abstract EffectId Id { get; }
    public bool IsActive = true;
    public bool IsCustomDurationEffectUsed = false;
    public float Duration = 5f;
    public float CurrentDuration = 5f;
    public int levelOfAnEffect = 1;
    public float AttackerAddativeEffect;
    public float AttackerIncreasedEffect;
    public float ReceiverEffectFlatReduction;
    public float ReceiverEffectPercentReduction;
    public AllObjectInformation Giver;
    public AllObjectInformation Receiver;
    public Dictionary<AllObjectInformation, List<Effect>> Dictionary;

    public void UpdateOrAndRemoveInactiveEffects(Dictionary<AllObjectInformation, List<Effect>> dictionary, float timePassing = 0f) {
        CurrentDuration -= timePassing;
        if (!IsCustomDurationEffectUsed) CustomDurationEffect();
        if (CurrentDuration <= 0f || IsActive == false) {
            DeactivateEffect(dictionary);
            RemovesItselfFromTheDictionary(dictionary);
        }
    }

    public void AddEffect(AllObjectInformation giver, AllObjectInformation receiver, Dictionary<AllObjectInformation, List<Effect>> dictionary, int levelOfASkill = 1) {
        Giver = giver;
        Receiver = receiver;
        Dictionary = dictionary;
        levelOfAnEffect = levelOfASkill;
        AttackerAddativeEffect = giver.EffectModifiers.CalculateAddedDmgFor(Id);
        AttackerIncreasedEffect = giver.EffectModifiers.CalculateMultiFor(Id);
        ReceiverEffectFlatReduction = receiver.EffectDefense.CalculateFlatDamageReductionFor(Id);
        ReceiverEffectPercentReduction = receiver.EffectDefense.CalculateDefensiveMultiFor(Id);

        AddsItselfToTheDictionary(dictionary);
        ActivateEffect(dictionary);
        CleanUp(dictionary);
    }

    public abstract void ActivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary);

    public abstract void DeactivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary);
    
    protected virtual void CustomDurationEffect() { }

    public void DeactivateDelegate(AllObjectInformation character) {
        DeactivateEffect(Dictionary);
        RemovesItselfFromTheDictionary(Dictionary);
    }

    public void UpdateEffect<T_Effect>(Dictionary<AllObjectInformation, List<Effect>> dictionary, AllObjectInformation receiver, float durationRefresh, int overrideLevel) where T_Effect : Effect {
        foreach (T_Effect effect in dictionary[receiver]) {
            effect.CurrentDuration = durationRefresh;
            effect.levelOfAnEffect = overrideLevel;
            break;
        }
    }

    private void CleanUp(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (dictionary.ContainsKey(Receiver)) {
            foreach (Effect effect in dictionary[Receiver]) {
                effect.UpdateOrAndRemoveInactiveEffects(dictionary);
            }
            if (dictionary[Receiver].Count == 0) dictionary.Remove(Receiver);
        }
    }

    public void AddsItselfToTheDictionary(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (!dictionary.ContainsKey(Receiver)) {
            dictionary[Receiver] = new List<Effect> { this };
        } else dictionary[Receiver].Add(this);
    }

    protected void RemovesItselfFromTheDictionary(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (dictionary.ContainsKey(Receiver)) {
            dictionary[Receiver].Remove(this);
        }
    }

    public bool IsInTheDictionary(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (dictionary.ContainsKey(Receiver))
            return dictionary[Receiver].Contains(this);
        else
            return false;
    }
}


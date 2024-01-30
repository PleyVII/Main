using System;
using UnityEngine;


//Always name Buffs inheriting from this class by purely their name, 
//it ensures finding the right Effect counterpart and EffectId enum
[CreateAssetMenu(fileName = "NewBuffSkill", menuName = "Skills/BuffSkill")]
public abstract class BuffSkill : ActiveSkill {
    #region Initialization of Effect counterpart of a Buff
    private Effect _cachedEffect = null;  // Field to cache the determined effect
    private int countDownJustInCase = 0;

    protected Effect WhatEffectIsThat {
        get {
            if (_cachedEffect == null) // If the effect hasn't been determined yet and the 
            {
                _cachedEffect = FiguringOutTheEffectTypeCounterPart();
                countDownJustInCase -= 1; if (countDownJustInCase < 0) Debug.LogError("That shouldn't be happening, it should happen only once"); //justInCaseCheck
                return _cachedEffect;
            } else return (Effect)Activator.CreateInstance(_cachedEffect.GetType());  // Return the new instance of cached effect
        }
    }

    protected Effect FiguringOutTheEffectTypeCounterPart() {
        string effectClassName = GetType().Name + "Effect";
        Type effectType = Type.GetType(effectClassName);
        if (effectType != null && typeof(Effect).IsAssignableFrom(effectType)) {
            return (Effect)Activator.CreateInstance(effectType);
        } else {
            Debug.LogError($"Cannot find the Effect class equivalent to {GetType().Name}. To fix, create a class named {effectClassName}.");
            return null;
        }
    }
    #endregion Initialization of Effect counterpart of a Buff

    public virtual void UseBuffOfThisSkillName(AllObjectInformation Attacker, AllObjectInformation target) {
        EffectsManager.Instance.AddEffect(WhatEffectIsThat, Attacker, target, levelCurrent);
    }
}

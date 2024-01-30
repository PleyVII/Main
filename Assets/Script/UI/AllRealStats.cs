using UnityEngine;

public class AllRealStats : MonoBehaviour {
    public RealStatChange Strbase;
    public RealStatChange Agibase;
    public RealStatChange Vitbase;
    public RealStatChange Critbase;
    public RealStatChange Specbase;
    public RealStatChange Intbase;
    public RealStatChange Dexbase;
    float usedPoints;
    public PlayerManager playerManager;
    public void Start() {
        playerManager = FindObjectOfType<PlayerManager>();
        // PlayerManger.OnPlayerChange += OnPlayerChangeStats;
    }
    public void SendingStats(CharacterUpdates character) {
        // character?.SendingBaseStat(Strbase.baseStat, Agibase.baseStat, Vitbase.baseStat, Critbase.baseStat, Specbase.baseStat, Intbase.baseStat, Dexbase.baseStat);
    }
    public void OnPlayerChangeStats(CharacterUpdates character) {
        Strbase.CurrentTarget = character;
        Agibase.CurrentTarget = character;
        Vitbase.CurrentTarget = character;
        Critbase.CurrentTarget = character;
        Specbase.CurrentTarget = character;
        Intbase.CurrentTarget = character;
        Dexbase.CurrentTarget = character;
        // Strbase.baseStat = character.str.baseValue;
        // Agibase.baseStat = character.agi.baseValue;
        // Vitbase.baseStat = character.vit.baseValue;
        // Critbase.baseStat = character.crit.baseValue;
        // Specbase.baseStat = character.spec.baseValue;
        // Intbase.baseStat = character._int.baseValue;
        // Dexbase.baseStat = character.dex.baseValue;
        SendingStats(character);
        Strbase.pointsToAddCalculation();
        Agibase.pointsToAddCalculation();
        Vitbase.pointsToAddCalculation();
        Critbase.pointsToAddCalculation();
        Specbase.pointsToAddCalculation();
        Intbase.pointsToAddCalculation();
        Dexbase.pointsToAddCalculation();
        Strbase.BaseStatTextChange();
        Agibase.BaseStatTextChange();
        Vitbase.BaseStatTextChange();
        Critbase.BaseStatTextChange();
        Specbase.BaseStatTextChange();
        Intbase.BaseStatTextChange();
        Dexbase.BaseStatTextChange();
    }
}

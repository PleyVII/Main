using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[Serializable, CreateAssetMenu(fileName = "AllObjectInformation", menuName = "NewCharacter")]
public class AllObjectInformation : ScriptableObject {
    [HideInInspector] public float baseExpToLvlUp, baseExpCurrent, jobExpToLvlUp, jobExpCurrent;
    [SerializeField] private float currentHealth, currentMana;
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = Mathf.Clamp(value, 0, Stats.MaxHealth); } }
    public float CurrentMana { get { return currentMana; } set { currentMana = Mathf.Clamp(value, 0, Stats.MaxMana); } }
    [SerializeField] public StatModifiersCollection Stats = new StatModifiersCollection();
    #region Modifiers Variables
    public BaseModifiersCollection<SizeId> SizePhysicalModifiers = new BaseModifiersCollection<SizeId>(), SizeMagicalModifiers = new BaseModifiersCollection<SizeId>(), SizeDefense = new BaseModifiersCollection<SizeId>();
    public BaseModifiersCollection<RaceId> RacePhysicalModifiers = new BaseModifiersCollection<RaceId>(), RaceMagicalModifiers = new BaseModifiersCollection<RaceId>(), RaceDefense = new BaseModifiersCollection<RaceId>();
    public BaseModifiersCollection<RankId> RankPhysicalModifiers = new BaseModifiersCollection<RankId>(), RankMagicalModifiers = new BaseModifiersCollection<RankId>(), RankDefense = new BaseModifiersCollection<RankId>();
    public ElementModifiersCollection ElementModifiers = new ElementModifiersCollection(), ElementDefense = new ElementModifiersCollection();
    public BaseModifiersCollection<EffectId> EffectModifiers = new BaseModifiersCollection<EffectId>(), EffectDefense = new BaseModifiersCollection<EffectId>();
    public BaseModifiersCollection<ConditionalId> ConditionalModifiers = new BaseModifiersCollection<ConditionalId>(), ConditionalDefense = new BaseModifiersCollection<ConditionalId>();
    #endregion Modifiers Variables
    #region Element Weapon and Body Armor Properties
    private ElementId weaponElement = ElementId.Neutral;
    [SerializeField] private ElementId DefaultBodyElement = ElementId.Neutral;
    private ElementId? previousBodyElement = null, currentBodyElement = null, statusBodyElement = null;
    [HideInInspector] public ElementId? statusWeaponElement = null;
    public ElementId BodyElement {
        get {
            ElementId actualElement;
            if (statusBodyElement is ElementId Element) actualElement = Element;
            else if (CurrentBodyElement is ElementId Element2) actualElement = Element2;
            else actualElement = DefaultBodyElement;
            return actualElement;
        }
    }

    public ElementId? CurrentBodyElement {
        get {
            return currentBodyElement;
        }
        set {
            if (value == null)
                if (previousBodyElement != null) {
                    currentBodyElement = previousBodyElement;
                    previousBodyElement = null;
                } else currentBodyElement = DefaultBodyElement;
            else {
                previousBodyElement = currentBodyElement;
                currentBodyElement = value;
            }
        }
    }
    public void ChangeBodyArmorElementFromStatus(ElementId? elementId = null) {
        statusBodyElement = elementId;
    }

    public ElementId WeaponElement {
        get {
            ElementId actualElement;
            if (statusWeaponElement is ElementId Element) actualElement = Element;
            else actualElement = weaponElement;
            return actualElement;
        }
        set { weaponElement = value; }
    }
    #endregion Element Weapon and Body Armor Properties
    #region IsMelee Property
    public bool IsMelee { get { return IsMeleeWeapon(); } }

    public bool IsMeleeWeapon() {
        int slot = 6;
        if (currentEquipment[slot] != null) {
            if (currentEquipment[slot] is Weapon weapon6) return IsMeleee(weapon6); else return true;
        } else {
            slot = 5;
            if (currentEquipment[slot] != null)
                if (currentEquipment[slot] is Weapon weapon5) return IsMeleee(weapon5); else return true;
            else return true;
        }
    }

    bool IsMeleee(Weapon weapon) {
        return weapon.weaponType != WeaponType.Bow
               && weapon.weaponType != WeaponType.Whip
               && weapon.weaponType != WeaponType.Shuriken;
    }

    #endregion IsMelee Property
    public SizeId OwnerSize = SizeId.Medium;
    public RaceId OwnerRace = RaceId.Human;
    public RankId OwnerRank = RankId.Normal;
    [HideInInspector] public bool canCastDuringMoving = false;
    [HideInInspector] public bool canChargeDuringAttacking = true;
    [HideInInspector] public bool canChargeDuringMoving = false;
    [HideInInspector] public bool canAttackDuringMoving = false;
    [HideInInspector] public bool canAttackDuringCasting = false;
    [HideInInspector] public bool isAutoAttacking = false;
    [HideInInspector] public ActiveSkill AttackSkill { get; set; }
    [HideInInspector] public ActiveSkill CastSkill { get; set; }
    [HideInInspector] public ActiveSkill ChargeSkill { get; set; }
    [HideInInspector] public Status CurrentStatus;
    [HideInInspector] private State currentState;
    public State CurrentState {
        get => currentState;
        private set => currentState = value;
    }

    public void ApplyState(State state) {
        switch (state) {
            case State.Moving:
                if (OwnerRank == RankId.Boss) break;
                if (!canAttackDuringCasting) {
                    currentState &= ~State.AutoAttacking | State.Attacking;
                    isAutoAttacking = false;
                    AttackSkill = null;
                }
                if (!canCastDuringMoving) {
                    currentState &= ~State.Casting;
                    if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                    CastSkill = null;
                }
                if (!canChargeDuringMoving) {
                    currentState &= ~State.Charging;
                    if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                    ChargeSkill = null;
                }
                break;

            case State.AutoAttacking:
                if (!canAttackDuringMoving) {
                    currentState &= ~State.Moving;
                }
                if (OwnerRank == RankId.Boss) break;
                if (!canChargeDuringAttacking) {
                    currentState &= ~State.Charging;
                    if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                    ChargeSkill = null;
                }
                if (!canAttackDuringCasting) {
                    currentState &= ~State.Casting;
                    if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                    CastSkill = null;
                }
                currentState &= ~State.Attacking;
                break;

            case State.Attacking:
                if (!canAttackDuringMoving) {
                    currentState &= ~State.Moving;
                }
                if (OwnerRank == RankId.Boss) break;
                if (!canChargeDuringAttacking) {
                    currentState &= ~State.Charging;
                    if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                    ChargeSkill = null;
                }
                if (!canAttackDuringCasting) {
                    currentState &= ~State.Casting;
                    if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                    CastSkill = null;
                }
                currentState &= ~State.AutoAttacking;
                isAutoAttacking = false;
                break;

            case State.Casting:
                if (OwnerRank == RankId.Boss) break;
                if (!canCastDuringMoving) {
                    currentState &= ~State.Moving;
                    break;
                }
                if (!canAttackDuringCasting) {
                    currentState &= ~(State.AutoAttacking | ~State.Attacking);
                    isAutoAttacking = false;
                    AttackSkill = null;
                }
                currentState &= ~State.Charging;
                if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                ChargeSkill = null;
                break;

            case State.Charging:
                if (OwnerRank == RankId.Boss) break;
                if (!canChargeDuringAttacking) {
                    currentState &= ~(State.AutoAttacking | ~State.Attacking);
                    isAutoAttacking = false;
                    AttackSkill = null;
                }
                if (!canChargeDuringMoving) {
                    currentState &= ~State.Moving;
                }
                currentState &= ~(State.Casting | State.Charging);
                if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                CastSkill = null;
                ChargeSkill = null;
                break;
            case State.Stun:
                if (OwnerRank == RankId.Boss) break;
                currentState &= ~State.MovingOrAttackingx2OrCastingOrCharging;
                isAutoAttacking = false;
                AttackSkill = null;
                if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                CastSkill = null;
                ChargeSkill = null;
                break;

            case State.Freeze:
                if (OwnerRank == RankId.Boss) break;
                currentState &= ~State.MovingOrAttackingx2OrCastingOrCharging;
                isAutoAttacking = false;
                AttackSkill = null;
                if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                CastSkill = null;
                ChargeSkill = null;
                break;

            case State.Silence:
                if (OwnerRank == RankId.Boss) break;
                currentState &= ~State.Casting;
                if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                CastSkill = null;
                break;

            case State.Root:
                currentState &= ~State.Moving;
                break;


            case State.Sleep:
                if (OwnerRank == RankId.Boss) break;
                currentState &= ~State.MovingOrAttackingx2OrCastingOrCharging;
                isAutoAttacking = false;
                AttackSkill = null;
                if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                CastSkill = null;
                ChargeSkill = null;
                break;

            case State.Trapped:
                currentState &= ~State.Moving;
                break;

            case State.PreCurse:
                if (OwnerRank == RankId.Boss) break;
                currentState &= ~State.Attackingx2OrCastingOrCharging;
                isAutoAttacking = false;
                AttackSkill = null;
                if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                CastSkill = null;
                ChargeSkill = null;
                break;

            case State.PostCurse:
                currentState &= ~State.Moving | State.PreCurse;
                if (OwnerRank == RankId.Boss) break;
                currentState &= ~State.Attackingx2OrCastingOrCharging;
                isAutoAttacking = false;
                AttackSkill = null;
                if (CastSkill != null) CastSkill.useTimer = Stats.AbilityCooldown;
                if (ChargeSkill != null) ChargeSkill.useTimer = Stats.AbilityCooldown;
                CastSkill = null;
                ChargeSkill = null;

                break;

            // Default case if needed
            default:
                Debug.LogError("Doesn't have logic for adding specific state");
                break;
        }
        currentState |= state;
    }


    public void RemoveState(State state) {
        currentState &= ~state;
    }


    public bool CanMove { get { return (CurrentState & State.PreventsMovement) == State.CanDoAction; } }
    public bool CanCast { get { return (CurrentState & State.PreventsCasting) == State.CanDoAction; } }
    public bool CanAttack { get { return (CurrentState & State.PreventsAttack) == State.CanDoAction; } }
    public Equipment[] currentEquipment = new Equipment[Enum.GetNames(typeof(EquipmentSlot)).Length];
    [HideInInspector] private GameObject owner;
    [HideInInspector] public GameObject Owner { get { return owner; } set { if (owner == null) owner = value; } }

    public void Initialize(GameObject gameObject) {
        Owner = gameObject;
        Stats.Owner = this;
        ElementModifiers.Owner = this;
        ElementDefense.Owner = this;
        RacePhysicalModifiers.Owner = this;
        RaceMagicalModifiers.Owner = this;
        RaceDefense.Owner = this;
        RankPhysicalModifiers.Owner = this;
        RankMagicalModifiers.Owner = this;
        RankDefense.Owner = this;
        SizePhysicalModifiers.Owner = this;
        SizeMagicalModifiers.Owner = this;
        SizeDefense.Owner = this;
        EffectModifiers.Owner = this;
        EffectDefense.Owner = this;
        ConditionalModifiers.Owner = this;
        ConditionalDefense.Owner = this;
    }

    public void Die() {

    }

    [HideInInspector] public float weaponNumberOfHits = 1;
    public delegate void HitAction(AllObjectInformation character);
    public event HitAction OnAttack;
    public void OnAttackInvoke() { OnAttack.Invoke(this); }
    #region The DealingDamage Functions
    public delegate void BeingHitAction(AllObjectInformation character);
    public event BeingHitAction OnBeingHit;

    public void DealPhysicalDamage(AllObjectInformation attacker, float physicaldamage, float numberOfHits = 1, ElementId? overrideWeaponElement = null, bool includeRaceModifiers = true, bool includeSizeModifiers = true, bool includeRankModifiers = true) {
        bool isHit = ConditionalCalculations.IsPhysicallyHit(attacker, this, attacker.Stats.Hit, Stats.Flee);
        //Do the possible status application check before that
        if (!isHit) { Debug.Log("Missed"); return; }
        ConditionMessenger messenger = ConditionalCalculations.ProcessDamageModifiers(attacker, this, true);

        float addedDamage = CalculateAddedDamage(attacker, false, includeRaceModifiers, includeSizeModifiers, includeRankModifiers, overrideWeaponElement) + messenger.addition;
        float damageMultiplier = CalculateMultiplierDamage(attacker, false, includeRaceModifiers, includeSizeModifiers, includeRankModifiers, overrideWeaponElement) * messenger.multiplier;
        float damageReduction = CalculateDefensiveMultiplier(attacker, overrideWeaponElement) * messenger.reduction;
        float flatDamageReduction = CalculateFlatDefensive(attacker, overrideWeaponElement) + messenger.subtraction;

        float totalPhysicalDamage = (physicaldamage + addedDamage) * damageMultiplier;
        float FinalPhysicalDamage;
        if (damageMultiplier <= 0) {
            FinalPhysicalDamage = Mathf.Clamp(totalPhysicalDamage, Mathf.NegativeInfinity, 0);
            CurrentHealth -= FinalPhysicalDamage;
        } else {
            float damageAfterDefenseReduction = totalPhysicalDamage * damageReduction * (100 - Stats.Def) / 100 - flatDamageReduction - Stats.SoftDef;
            FinalPhysicalDamage = Mathf.Clamp(damageAfterDefenseReduction * numberOfHits, 0, Mathf.Infinity);
            CurrentHealth -= FinalPhysicalDamage;
        }

        if (messenger.isCriticallyHit) Debug.Log($"Hit for {FinalPhysicalDamage} with critical damage, {CurrentHealth} hp left"); // possibly some different text/audio when critically hit
        else Debug.Log($"Hit for {FinalPhysicalDamage} damage , {CurrentHealth} hp left");
        if (FinalPhysicalDamage > 0) OnBeingHit.Invoke(this);
    }

    public void DealMagicalDamage(AllObjectInformation attacker, float magicaldamage, float numberOfHits = 1, ElementId overrideElementId = ElementId.Neutral, bool includeRaceModifiers = true, bool includeSizeModifiers = true, bool includeRankModifiers = true) {
        ConditionMessenger messenger = ConditionalCalculations.ProcessDamageModifiers(attacker, this, true);
        //Do the possible status application check

        float addedDamage = CalculateAddedDamage(attacker, true, includeRaceModifiers, includeSizeModifiers, includeRankModifiers, overrideElementId) + messenger.addition;
        float damageMultiplier = CalculateMultiplierDamage(attacker, true, includeRaceModifiers, includeSizeModifiers, includeRankModifiers, overrideElementId) * messenger.multiplier;
        float damageReduction = CalculateDefensiveMultiplier(attacker, overrideElementId) * messenger.reduction;
        float flatDamageReduction = CalculateFlatDefensive(attacker, overrideElementId) + messenger.subtraction;
        float totalMagicalDamage = (magicaldamage + addedDamage) * damageMultiplier;
        float FinalMagicalDamage;
        if (damageMultiplier <= 0) {
            FinalMagicalDamage = Mathf.Clamp(totalMagicalDamage, Mathf.NegativeInfinity, 0);
            CurrentHealth -= totalMagicalDamage;
        } else {
            float damageAfterDefenseReduction = totalMagicalDamage * damageReduction * (100 - Stats.MDef) / 100 - flatDamageReduction - Stats.SoftMDef;
            FinalMagicalDamage = Mathf.Clamp(damageAfterDefenseReduction * numberOfHits, 0, Mathf.Infinity);
            CurrentHealth -= FinalMagicalDamage;
        }
        if (messenger.isCriticallyHit) Debug.Log($"Hit for {FinalMagicalDamage} with critical damage, {CurrentHealth} hp left"); // possibly some different text/audio when critically hit
        else Debug.Log($"Hit for {FinalMagicalDamage} damage , {CurrentHealth} hp left");
        if (FinalMagicalDamage > 0) OnBeingHit.Invoke(this);
    }

    private float CalculateAddedDamage(AllObjectInformation Attacker, bool isMagicalDamage, bool includeRaceModifiers = true, bool includeSizeModifiers = true, bool includeRankModifiers = true, ElementId? overrideElementId = null) {

        ElementId elementIdToUse = overrideElementId ?? Attacker.WeaponElement;
        float addedDamage = Attacker.ElementModifiers.CalculateAddedDmgFor(elementIdToUse, BodyElement);

        if (includeSizeModifiers) addedDamage += isMagicalDamage ? Attacker.SizeMagicalModifiers.CalculateAddedDmgFor(OwnerSize)
                                                                 : Attacker.SizePhysicalModifiers.CalculateAddedDmgFor(OwnerSize);

        if (includeRaceModifiers) addedDamage += isMagicalDamage ? Attacker.RaceMagicalModifiers.CalculateAddedDmgFor(OwnerRace)
                                                                 : Attacker.RacePhysicalModifiers.CalculateAddedDmgFor(OwnerRace);

        if (includeRankModifiers) addedDamage += isMagicalDamage ? Attacker.RankMagicalModifiers.CalculateAddedDmgFor(OwnerRank)
                                                                 : Attacker.RankPhysicalModifiers.CalculateAddedDmgFor(OwnerRank);

        return addedDamage;
    }

    private float CalculateMultiplierDamage(AllObjectInformation Attacker, bool isMagicalDamage, bool includeRaceModifiers = true, bool includeSizeModifiers = true, bool includeRankModifiers = true, ElementId? overrideElementId = null) {

        ElementId elementIdToUse = overrideElementId ?? Attacker.WeaponElement;
        float multiDamage = Attacker.ElementModifiers.CalculateMultiFor(elementIdToUse, BodyElement);

        if (includeSizeModifiers) multiDamage *= isMagicalDamage ? Attacker.SizeMagicalModifiers.CalculateMultiFor(OwnerSize)
                                                                 : Attacker.SizePhysicalModifiers.CalculateMultiFor(OwnerSize);

        if (includeRaceModifiers) multiDamage *= isMagicalDamage ? Attacker.RaceMagicalModifiers.CalculateMultiFor(OwnerRace)
                                                                 : Attacker.RacePhysicalModifiers.CalculateMultiFor(OwnerRace);

        if (includeRankModifiers) multiDamage *= isMagicalDamage ? Attacker.RankMagicalModifiers.CalculateMultiFor(OwnerRank)
                                                                 : Attacker.RankPhysicalModifiers.CalculateMultiFor(OwnerRank);

        return multiDamage;
    }

    private float CalculateDefensiveMultiplier(AllObjectInformation Attacker, ElementId? overrideAttackerElementId = null) {
        ElementId attackerElementIdToUse = overrideAttackerElementId ?? Attacker.WeaponElement;

        float multiDefensive = ElementDefense.CalculateDefensiveMultiFor(attackerElementIdToUse, BodyElement) *
                               SizeDefense.CalculateDefensiveMultiFor(Attacker.OwnerSize) *
                               RaceDefense.CalculateDefensiveMultiFor(Attacker.OwnerRace) *
                               RankDefense.CalculateDefensiveMultiFor(Attacker.OwnerRank);
        return multiDefensive;
    }

    private float CalculateFlatDefensive(AllObjectInformation Attacker, ElementId? overrideAttackerElementId = null) {
        ElementId attackerElementIdToUse = overrideAttackerElementId ?? Attacker.WeaponElement;

        float flatDefensive = ElementDefense.CalculateFlatDamageReductionFor(BodyElement, attackerElementIdToUse) +
                              SizeDefense.CalculateFlatDamageReductionFor(Attacker.OwnerSize) +
                              RaceDefense.CalculateFlatDamageReductionFor(Attacker.OwnerRace) +
                              RankDefense.CalculateFlatDamageReductionFor(Attacker.OwnerRank);

        return flatDefensive;
    }
    #endregion The DealingDamage Functions

    [HideInInspector] public AllObjectInformation targetInfo, targetToCastSkillOn;
    [HideInInspector] public Interactable targetInteractable, interactable;
    [HideInInspector] public Transform targetTransform, transform;
    [HideInInspector] public Vector3? targetGround = null;
    [HideInInspector]
    public float? DistanceToTarget {
        get {
            if (targetTransform != null)
                return Vector3.Distance(transform.position, targetTransform.position);
            else if (targetGround != null) return Vector3.Distance(transform.position, (Vector3)targetGround);
            else return null;
        }
    }
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public float animationCancelWindow = 0f, castingTimer = 0f, chargingTimer = 0f;
    [HideInInspector] public ActiveSkill whenInRange_Skill = null;

    public void StoppingDistanceCalculate() {
        if (targetInteractable == null) agent.stoppingDistance = 0f;
        else if (targetInteractable is not InteractableItem item) agent.stoppingDistance = Stats.WeaponRange;
        else agent.stoppingDistance = item.pickUpRadius;
    }

    public void StoppingDistanceCalculate(float skillRange) {
        agent.stoppingDistance = skillRange;
    }

    public void PushAgent(AllObjectInformation targetInfo, float pushDistance) {
        float PushDistance = pushDistance;
        if (OwnerRank == RankId.Miniboss) PushDistance = pushDistance / 2;
        if (OwnerRank == RankId.Boss) PushDistance = pushDistance / 4;
        Vector3 startPosition = transform.position;
        Vector3 direction = (targetInfo.transform.position - startPosition).normalized;
        Vector3 endPosition = startPosition + direction * PushDistance;

        if (NavMesh.SamplePosition(endPosition, out NavMeshHit navHit, PushDistance, NavMesh.GetAreaFromName("Walkable"))) {
            targetInfo.agent.Warp(navHit.position);
        }
    }














    #region Editor code
    [CustomEditor(typeof(AllObjectInformation))]
    public class AllObjectInformationEditor : Editor {
        private string activeField = null;
        private float newBaseValue;
        public bool FirstLabel = true;
        float anotherVariableEhhh;
        float distance = 150f;
        public override void OnInspectorGUI() {
            serializedObject.Update();
            // Cast the target object to the correct type
            AllObjectInformation allObjectInfo = (AllObjectInformation)target;


            // Use the default inspector for most fields
            DrawDefaultInspector();
            EditorGUIUtility.labelWidth = 145;

            // Add a section label
            EditorGUILayout.LabelField("Character Stats", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            StringWithButton(() => allObjectInfo.Stats.CharacterLvl, value => allObjectInfo.Stats.CharacterLvl = value, "Character Level", "Set");
            StringWithButton(() => allObjectInfo.Stats.JobLvl, value => allObjectInfo.Stats.JobLvl = value, "Job Level", "Set");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            // modifier.FlatValue = EditorGUILayout.FloatField(modifier.FlatValue, GUILayout.Width(50)); // Keep the field short
            EditorGUILayout.LabelField("Stat Points", allObjectInfo.Stats.SPoints.ToString(), GUILayout.Width(260), GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.LabelField("All Points Used", allObjectInfo.Stats.allPointsUsed.ToString(), GUILayout.Width(270), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            StringWithButton(() => allObjectInfo.Stats.Str, value => allObjectInfo.Stats.Str = value, "Strength");
            StringWithButton(() => allObjectInfo.Stats.Agi, value => allObjectInfo.Stats.Agi = value, "Agility");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            StringWithButton(() => allObjectInfo.Stats.Vit, value => allObjectInfo.Stats.Vit = value, "Vitality");
            StringWithButton(() => allObjectInfo.Stats.HealthRegen, value => allObjectInfo.Stats.HealthRegen = value, "Health Regen");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            StringWithButton(() => allObjectInfo.Stats.Spec, value => allObjectInfo.Stats.Spec = value, "Special");
            StringWithButton(() => allObjectInfo.Stats.Crit, value => allObjectInfo.Stats.Crit = value, "Critical");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            StringWithButton(() => allObjectInfo.Stats.Int, value => allObjectInfo.Stats.Int = value, "Intelligence");
            StringWithButton(() => allObjectInfo.Stats.ManaRegen, value => allObjectInfo.Stats.ManaRegen = value, "Mana Regen");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            StringWithButton(() => allObjectInfo.Stats.Dex, value => allObjectInfo.Stats.Dex = value, "Dexterity");
            StringWithButton(() => allObjectInfo.Stats.MaxHealth, value => allObjectInfo.Stats.MaxHealth = value, "Max Health", "Set");
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 120;

            // Group 1
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max Mana", allObjectInfo.Stats.MaxMana.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Health Regen/5", allObjectInfo.Stats.HealthRegenPer5.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mana Regen/5", allObjectInfo.Stats.ManaRegenPer5.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Weapon Attack", allObjectInfo.Stats.WeaponAtk.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Magic Attack", allObjectInfo.Stats.Matk.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attack", allObjectInfo.Stats.Atk.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Attack Cooldown", allObjectInfo.Stats.AttackRate.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Cast Time Multi", allObjectInfo.Stats.CastTimeMultiplier.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Flee", allObjectInfo.Stats.Flee.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Weapon Range", allObjectInfo.Stats.WeaponRange.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Hit", allObjectInfo.Stats.Hit.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Critical Chance", allObjectInfo.Stats.CritChance.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Soft Magic Defense", allObjectInfo.Stats.SoftMDef.ToString(), GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Soft Defense", allObjectInfo.Stats.SoftDef.ToString());
            EditorGUILayout.EndHorizontal();


            void StringWithButton(Func<float> getter, Action<float> setter, string fieldName, string buttonName = "Add") {
                if (FirstLabel) EditorGUILayout.BeginHorizontal();
                float currentValue = getter();
                if (currentValue == allObjectInfo.Stats.MaxHealth) { distance -= 13; anotherVariableEhhh += 13; }
                GUILayout.Label($"{fieldName}", GUILayout.Width(distance));
                EditorGUILayout.LabelField("", currentValue.ToString(), GUILayout.Width(anotherVariableEhhh + 40), GUILayout.ExpandWidth(false));
                if (currentValue == allObjectInfo.Stats.MaxHealth) { distance += 13; anotherVariableEhhh -= 13; }

                // Check if this field is the active one
                if (activeField == fieldName) {
                    float GUILayoutWidth;
                    if (getter() == allObjectInfo.Stats.MaxHealth) GUILayoutWidth = 60f; else GUILayoutWidth = 30f;
                    newBaseValue = EditorGUILayout.FloatField("", newBaseValue, GUILayout.Width(GUILayoutWidth), GUILayout.ExpandWidth(false));
                    if (GUILayout.Button(buttonName, GUILayout.Width(35), GUILayout.ExpandWidth(false))) {
                        setter(newBaseValue);
                        activeField = null;
                    }

                    if (GUILayout.Button("Reset", GUILayout.Width(45), GUILayout.ExpandWidth(false))) {
                        if (getter() == allObjectInfo.Stats.MaxHealth) setter(2000);
                        else setter(-currentValue);
                        activeField = null;
                    }

                } else {
                    // If this field isn't active, show the Change button
                    if (GUILayout.Button("Change", GUILayout.Width(60), GUILayout.ExpandWidth(false))) {
                        activeField = fieldName;
                    }
                }
                if (!FirstLabel) { EditorGUILayout.EndHorizontal(); }
                if (FirstLabel && activeField == fieldName) distance = 94; else distance = 150;
                FirstLabel = !FirstLabel;
            }


            // If there are any changes, mark the object as dirty so Unity knows to save the changes
            if (GUI.changed) {
                EditorUtility.SetDirty(allObjectInfo);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
    #endregion Editor code
}
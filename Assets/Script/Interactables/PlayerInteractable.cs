using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;


public class InteractablePlayer : Interactable {
    public SkillBars skillBars;
    [SerializeField] private ActiveSkill selectedSkill;
    [SerializeField] private KeyCode key;
    [SerializeField] private LayerMask movementMask, interactableMask;
    int combinedMask;
    public Dictionary<KeyCode, int> keyToSlotIndexMap;
    CharacterMovement movement;
    [SerializeField] private AllObjectInformation allInfo;
    private float AttackCooldown { get { return allInfo.Stats.AttackCooldown; } }
    private float AbilityCooldown { get { return allInfo.Stats.AbilityCooldown; } }
    private float preAttackCooldown { get { return allInfo.Stats.AttackCooldown * 0.8f; } }
    private float preAbilityCooldown { get { return allInfo.Stats.AbilityCooldown * 0.8f; } }


    private void InitializeKeyToSlotIndexMap() {
        keyToSlotIndexMap = new Dictionary<KeyCode, int>();
        // Add key-slot index associations here
        keyToSlotIndexMap[KeyCode.Alpha1] = 0;
        keyToSlotIndexMap[KeyCode.Alpha2] = 1;
        keyToSlotIndexMap[KeyCode.Alpha3] = 2;
        keyToSlotIndexMap[KeyCode.Alpha4] = 3;
        keyToSlotIndexMap[KeyCode.Alpha5] = 4;
        keyToSlotIndexMap[KeyCode.Alpha6] = 5;
        keyToSlotIndexMap[KeyCode.Alpha7] = 6;
        keyToSlotIndexMap[KeyCode.Alpha8] = 7;
        keyToSlotIndexMap[KeyCode.Alpha9] = 8;
    }

    private void Start() {
        allInfo = GetComponent<CharacterUpdates>().allInfo;
        movement = GetComponent<CharacterMovement>();
        combinedMask = interactableMask | movementMask;
        InitializeKeyToSlotIndexMap();
    }

    public void Update() {
        ActivateSkillsInSlots();
        if (PlayerManager.Instance.FocusedCharacter == allInfo) {
            if (Input.GetMouseButtonDown(0)) {
                if (RaySomethingWithUICheck(combinedMask, out RaycastHit hit)) {
                    HandleRaycastHit(hit);
                } else {
                    ClearSkill();
                }
                if (selectedSkill != null && (selectedSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtCharacter && allInfo.targetInteractable != null || selectedSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtGround && allInfo.targetGround != null)) {
                    SkillLoaded();
                }
            } else if (selectedSkill != null && selectedSkill.TargetingType_Skill == TargetingType_Skill.NonTargeted) SkillLoaded();
        }

        StartSkillsWhenInDistance();
        UseSkills();
        AutoAttack();
        PickUpItem();
    }

    void HandleRaycastHit(RaycastHit hit) {
        LayerMask layerMask = 1 << hit.collider.gameObject.layer;

        if ((layerMask & interactableMask) != 0) {
            AllObjectInformation hitAllInfo = hit.collider.GetComponent<CharacterUpdates>().allInfo;
            Interactable hitInteractable = hit.collider.GetComponent<Interactable>();
            HandleInteractableMaskHit(hitAllInfo, hitInteractable);
        } else if ((layerMask & movementMask) != 0) {
            HandleMovementMaskHit(hit.point);
        }
    }

    void HandleInteractableMaskHit(AllObjectInformation hitAllInfo, Interactable hitInteractable) {
        if (hitInteractable is InteractableItem item) {
            if (selectedSkill == null) {
                movement.FollowItemTarget(item);
                ClearSkill();
            }
        } else if (selectedSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtCharacter) {
            movement.FollowTargetWithSkill(hitAllInfo, selectedSkill.Range);
        } else {
            movement.FollowTargetWithoutSkill(hitAllInfo);
            ClearSkill();
            allInfo.isAutoAttacking = true;
            allInfo.Stats.currentAutoAttackCooldown = AttackCooldown;
        }
    }

    void HandleMovementMaskHit(Vector3 hitPoint) {
        if (selectedSkill != null && selectedSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtGround) {
            movement.TargetGroundWithSkill(hitPoint, selectedSkill.Range);
        } else {
            if (selectedSkill == null) movement.MoveToGround(hitPoint);
            ClearSkill();
        }
    }

    void StartSkillsWhenInDistance() {
        if ((((allInfo.targetTransform != null) || (allInfo.targetGround != null)) && allInfo.DistanceToTarget <= allInfo.agent.stoppingDistance) || (selectedSkill != null && selectedSkill.TargetingType_Skill == TargetingType_Skill.NonTargeted)) {
            if (allInfo.isAutoAttacking && 0 <= AttackCooldown) {
                allInfo.ApplyState(State.AutoAttacking);
            }

            if ((allInfo.whenInRange_Skill != null) && 0 <= AbilityCooldown) {
                switch (allInfo.whenInRange_Skill.TimerType_Skill) {
                    case TimerType_Skill.NotCastableOrChargeable:
                        allInfo.AttackSkill = allInfo.whenInRange_Skill;
                        allInfo.ApplyState(State.Attacking);
                        break;

                    case TimerType_Skill.Castable:
                        allInfo.whenInRange_Skill.useTimer *= allInfo.Stats.CastTimeMultiplier;
                        allInfo.CastSkill = allInfo.whenInRange_Skill;
                        allInfo.ApplyState(State.Casting);
                        break;

                    case TimerType_Skill.Chargable:
                        allInfo.whenInRange_Skill.useTimer *= allInfo.Stats.ChargeTimeMultiplier;
                        allInfo.ChargeSkill = allInfo.whenInRange_Skill;
                        allInfo.ApplyState(State.Charging);
                        break;
                }

            }
        }
    }

    void UseSkills() {
        if ((allInfo.CurrentState & State.AttackingOrCastingOrCharging) != 0) {

            if (allInfo.AttackSkill != null) {
                Attacking();
            }

            if (allInfo.CastSkill != null) {
                Casting();
            }

            if (allInfo.ChargeSkill != null) {
                Charging();
            }
        }
    }

    private void AutoAttack() {
        if (allInfo.targetInfo && allInfo.CurrentState.HasFlag(State.AutoAttacking)) {
            allInfo.Stats.currentAutoAttackCooldown -= Time.deltaTime;
            if (preAttackCooldown <= allInfo.Stats.currentAutoAttackCooldown) {
                allInfo.animationCancelWindow = allInfo.Stats.AttackCooldown * 0.3f;
                allInfo.Stats.AfterAttack = true;
                allInfo.Stats.currentAutoAttackCooldown = AttackCooldown;
                allInfo.RemoveState(State.AutoAttacking);
            }
        }
    }

    private void Attacking() {
        allInfo.AttackSkill.useTimer -= Time.deltaTime;
        if (preAttackCooldown <= allInfo.AttackSkill.useTimer) {
            if (allInfo.AttackSkill is ActiveSkill skill1) {
                if (allInfo.AttackSkill.TargetingType_Skill == TargetingType_Skill.NonTargeted) skill1.UseSkill(allInfo);
                else if (allInfo.AttackSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtCharacter) skill1.UseSkill(allInfo, allInfo.targetInfo);
                else if (allInfo.AttackSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtGround) skill1.UseSkill(allInfo, null, (Vector3)allInfo.targetGround);
            }
            allInfo.animationCancelWindow = allInfo.Stats.AbilityCooldown * 0.3f;
            allInfo.Stats.AfterAbility = true;
            allInfo.AttackSkill = null;
            allInfo.RemoveState(State.Attacking);
        }
    }

    private void Casting() {
        allInfo.CastSkill.useTimer -= Time.deltaTime;
        if (allInfo.CastSkill.useTimer <= 0) {
            if (allInfo.CastSkill is ActiveSkill skill1) {
                if (allInfo.CastSkill.TargetingType_Skill == TargetingType_Skill.NonTargeted) skill1.UseSkill(allInfo);
                else if (allInfo.CastSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtCharacter) skill1.UseSkill(allInfo, allInfo.targetInfo);
                else if (allInfo.CastSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtGround) skill1.UseSkill(allInfo, null, (Vector3)allInfo.targetGround);
            }
            allInfo.animationCancelWindow = allInfo.Stats.AbilityCooldown * 0.3f;
            allInfo.Stats.AfterAbility = true;
            allInfo.CastSkill = null;
            allInfo.RemoveState(State.Casting);
        }
    }

    private void Charging() {
        allInfo.ChargeSkill.useTimer -= Time.deltaTime;
        if (allInfo.ChargeSkill.useTimer <= 0) {
            if (allInfo.ChargeSkill is ActiveSkill skill1) {
                if (allInfo.ChargeSkill.TargetingType_Skill == TargetingType_Skill.NonTargeted) skill1.UseSkill(allInfo);
                else if (allInfo.ChargeSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtCharacter) skill1.UseSkill(allInfo, allInfo.targetInfo);
                else if (allInfo.ChargeSkill.TargetingType_Skill == TargetingType_Skill.TargetedAtGround) skill1.UseSkill(allInfo, null, (Vector3)allInfo.targetGround);
            }
            allInfo.animationCancelWindow = allInfo.Stats.AbilityCooldown * 0.3f;
            allInfo.Stats.AfterAbility = true;
            allInfo.ChargeSkill = null;
            allInfo.RemoveState(State.Charging);
        }
    }

    void PickUpItem() {
        if (allInfo.targetInteractable is InteractableItem item && allInfo.DistanceToTarget <= allInfo.agent.stoppingDistance) {
            Debug.Log("Picks up item, need to implement");
        }
    }

    private bool RaySomethingWithUICheck(LayerMask mask, out RaycastHit hit) {
        if (EventSystem.current.IsPointerOverGameObject()) { hit = new RaycastHit(); return false; }
        Ray ray = PlayerManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        bool didHit = Physics.Raycast(ray, out hit, 100, mask);
        if (hit.collider != null && hit.collider.gameObject == gameObject) {
            didHit = false;
        }
        return didHit;
    }



    void SkillLoaded() {
        allInfo.whenInRange_Skill = selectedSkill;
        selectedSkill = null;
    }

    private void ActivateSkillInSlot(int slotIndex) {
        if (skillBars == null) return;
        if (slotIndex >= 0 && slotIndex < skillBars.skillBarSlots.Length) {
            if (skillBars.skillBarSlots[slotIndex] != null) {
                if (skillBars.skillBarSlots[slotIndex].currentSkill is ActiveSkill Skill) {
                    selectedSkill = Skill;
                }
            }
        }
    }

    private void ActivateSkillsInSlots() {
        foreach (var kvp in keyToSlotIndexMap) {
            if (Input.GetKeyDown(kvp.Key)) {
                ActivateSkillInSlot(kvp.Value);
            }
        }
    }

    public void TargetedAttack(AllObjectInformation targetStats) {
        {
            //Enemy is getting attacked with myStats
            if (AttackCooldown <= 0) {
                Debug.Log(targetStats);
                targetStats.DealPhysicalDamage(targetStats, allInfo.Stats.Atk, allInfo.weaponNumberOfHits);
                allInfo.OnAttackInvoke();
            }
        }
    }

    void ClearSkill() {
        selectedSkill = null;
        allInfo.whenInRange_Skill = null;
    }

    public void Pickup(InteractableItem Item) {
        float distance = Vector3.Distance(transform.position, Item.transform.position);
        if (distance <= Item.pickUpRadius) {
            bool wasPickedUp = Inventory.Instance.AddToInventory(Item.item);
            if (wasPickedUp) {
                Debug.Log("Picking up" + Item.name);
                movement.StopFollowingTarget();
                Destroy(Item.gameObject);
            }
        }
    }
}
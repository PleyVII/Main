using UnityEngine;

public class ConditionalCalculations {


    public static ConditionMessenger ProcessDamageModifiers(AllObjectInformation attacker, AllObjectInformation defender, bool isPhysicalDamage) {

        //this uses ConditionMessenger, so it contains multiple variables that require possibly additional checks
        //then gets sent back to PhysicallyAttacked or MagicallyAttacked, and spreads those variables up

        ConditionMessenger messenger = new ConditionMessenger();

        Range_Frontstab_Backstab_Checks(messenger, attacker, defender);

        addingValues(messenger, attacker, defender, ConditionalId.AllSources); // just pure damage or damage reduction with no checks.

        if (IsCriticallyHit(attacker, defender, attacker.Stats.Hit, defender.Stats.Flee, attacker.Stats.CritChance, isPhysicalDamage)) {
            messenger.isCriticallyHit = true;
            addingValues(messenger, attacker, defender, ConditionalId.Critical, 1.2f);
        }

        if (defender.CurrentStatus.HasFlag(Status.Poison)) {
            addingValues(messenger, attacker, defender, ConditionalId.Poison);
        }

        if (defender.CurrentStatus.HasFlag(Status.FastCasting)) {
            addingValues(messenger, attacker, defender, ConditionalId.FastCasting);
        }

        if (defender.CurrentStatus.HasFlag(Status.SlowCasting)) {
            addingValues(messenger, attacker, defender, ConditionalId.SlowCasting);
        }

        if (defender.CurrentStatus.HasFlag(Status.Provoke)) {
            addingValues(messenger, attacker, defender, ConditionalId.Provoke);
        }

        if (defender.CurrentStatus.HasFlag(Status.LexAeterna)) {
            addingValues(messenger, attacker, defender, ConditionalId.LexAeterna, 1.5f);
        }

        if (defender.CurrentStatus.HasFlag(Status.Blind)) {
            addingValues(messenger, attacker, defender, ConditionalId.Blind);                                                                                        //I know, dont judge me
        }                                                                                                                                     //if you get better idea with enum flags, let me know, because I dont

        if (defender.CurrentStatus.HasFlag(Status.Stealth)) {
            addingValues(messenger, attacker, defender, ConditionalId.Stealth);
        }

        if (defender.CurrentStatus.HasFlag(Status.Combo)) {
            addingValues(messenger, attacker, defender, ConditionalId.Combo);
        }

        if (defender.CurrentStatus.HasFlag(Status.LinkGiver)) {
            addingValues(messenger, attacker, defender, ConditionalId.LinkGiver);
        }

        if (defender.CurrentStatus.HasFlag(Status.LinkReciever)) {
            addingValues(messenger, attacker, defender, ConditionalId.LinkReciever);
        }

        if (defender.CurrentStatus.HasFlag(Status.LowLife)) {
            addingValues(messenger, attacker, defender, ConditionalId.LowLife);
        }

        if (defender.CurrentStatus.HasFlag(Status.FullLife)) {
            addingValues(messenger, attacker, defender, ConditionalId.FullLife);
        }

        if (defender.CurrentState.HasFlag(State.Attacking)) {
            addingValues(messenger, attacker, defender, ConditionalId.Attacking);
        }

        if (defender.CurrentState.HasFlag(State.Casting)) {
            addingValues(messenger, attacker, defender, ConditionalId.Casting);
        }

        if (defender.CurrentState.HasFlag(State.Stun)) {
            addingValues(messenger, attacker, defender, ConditionalId.Stun);
        }

        if (defender.CurrentState.HasFlag(State.Freeze)) {
            addingValues(messenger, attacker, defender, ConditionalId.Freeze);
        }

        if (defender.CurrentState.HasFlag(State.Silence)) {
            addingValues(messenger, attacker, defender, ConditionalId.Silence);
        }

        if (defender.CurrentState.HasFlag(State.Root)) {
            addingValues(messenger, attacker, defender, ConditionalId.Root);
        }

        if (defender.CurrentState.HasFlag(State.Sleep)) {
            addingValues(messenger, attacker, defender, ConditionalId.Sleep);
        }

        if (defender.CurrentState.HasFlag(State.Trapped)) {
            addingValues(messenger, attacker, defender, ConditionalId.Trapped);
        }

        if (defender.CurrentState.HasFlag(State.PostCurse)) {
            addingValues(messenger, attacker, defender, ConditionalId.PostCurse);
        }
        return messenger;
    }

    public static bool IsPhysicallyHit(AllObjectInformation attacker, AllObjectInformation defender, float hit, float flee) {
        float hitChance = CalculateHitChance(attacker, defender, hit, flee);
        if (hitChance == 100f) return true;
        return Random.value <= hitChance / 100.0f;
    }

    public static bool IsCriticallyHit(AllObjectInformation attacker, AllObjectInformation defender, float hit, float flee, float critChance, bool isPhysicalDamage) {
        float hitChance = CalculateHitChance(attacker, defender, hit, flee);
        if (isPhysicalDamage || hitChance == 100.0f) {
            if (Random.value <= hitChance / 100.0f) {
                return Random.value <= critChance / 100.0f;
            } else return false;
        } else return Random.value <= critChance / 100.0f;
    }

    public static float CalculateHitChance(AllObjectInformation attacker, AllObjectInformation defender, float hit, float flee) {
        if (defender.CurrentState.HasFlag(State.Stun)) return 100f;
        float hitvalue = hit;
        if (attacker.CurrentStatus == Status.Blind) hitvalue /= 2;
        float difference = Mathf.Clamp(flee - hitvalue, 0, 90);
        float hitChance = 100.0f - difference;
        return hitChance;
    }

    public static void addingValues(ConditionMessenger messenger, AllObjectInformation attacker, AllObjectInformation defender, ConditionalId conditionalId, float additionalOptionalMultiplier = 1f) {
        messenger.addition += attacker.ConditionalModifiers.CalculateAddedDmgFor(conditionalId);
        messenger.multiplier *= additionalOptionalMultiplier * attacker.ConditionalModifiers.CalculateMultiFor(conditionalId);
        messenger.subtraction += defender.ConditionalModifiers.CalculateAddedDmgFor(conditionalId);
        messenger.reduction *= (100 - defender.ConditionalModifiers.CalculatePercentageSumFor(conditionalId)) / 100;
    }

    public static void Range_Frontstab_Backstab_Checks(ConditionMessenger messenger, AllObjectInformation attacker, AllObjectInformation defender) {
        bool isFrontstab = false;
        bool isBackstab = false;
        Vector3 directionToAttacker = attacker.Owner.transform.position - defender.Owner.transform.position;

        // Calculate the angle between the defender's forward direction and the direction to the attacker
        float angle = Vector3.Angle(defender.Owner.transform.forward, directionToAttacker);

        float frontAngleRange = 100f; // 100 degrees in front
        float backAngleStart = 180f - (frontAngleRange / 2); // Starting angle for backstab
        float backAngleEnd = 180f + (frontAngleRange / 2); // Ending angle for backstab
        if (angle <= frontAngleRange) {
            isFrontstab = true; addingValues(messenger, attacker, defender, ConditionalId.Frontstab);
        } else if (angle >= backAngleStart && angle <= backAngleEnd) {
            isBackstab = true; addingValues(messenger, attacker, defender, ConditionalId.Backstab);
        }
        bool isMelee = attacker.IsMelee; //here to not repeat the multiple checks, just one

        if (isMelee) addingValues(messenger, attacker, defender, ConditionalId.Melee);
        if (isFrontstab) {
            if (isMelee) {
                addingValues(messenger, attacker, defender, ConditionalId.MeleeFrontstab);
            } else {
                addingValues(messenger, attacker, defender, ConditionalId.RangedFrontstab);
            }
        } else if (isBackstab) {
            if (isMelee) {
                addingValues(messenger, attacker, defender, ConditionalId.MeleeBackstab);
            } else {
                addingValues(messenger, attacker, defender, ConditionalId.RangedBackstab);
            }
        }
    }
}

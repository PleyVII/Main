using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAiTypeActionDamage {

}
public enum EnemyActionType {
    Damage,
    Charge,
    Buff,
    CrowdControl
}

public class EnemyAI : MonoBehaviour {
    public List<EnemyActionType> actionsQueue = new List<EnemyActionType>();
    public float movementSpeed = 5f;
    public float actionCooldown = 2f;
    public float rangeThreshold = 2f;
    public float healthThreshold = 0.3f;

    private Transform target;
    private bool isMoving;
    private bool isAttacking;
    private float currentActionCooldown;

    private void Start() {
        // Start with a basic attack action
        actionsQueue.Add(EnemyActionType.Damage);
    }

    private void Update() {
        if (target == null) {
            FindTarget();
            return;
        }

        if (currentActionCooldown > 0) {
            currentActionCooldown -= Time.deltaTime;
            return;
        }

        if (isMoving) {
            MoveToTarget();
            return;
        }

        if (isAttacking) {
            AttackTarget();
            return;
        }

        if (actionsQueue.Count > 0) {
            PerformNextAction();
        } else {
            // If no actions left, perform a basic attack
            PerformBasicAttack();
        }
    }

    private void PerformNextAction() {
        EnemyActionType nextAction = actionsQueue[0];
        actionsQueue.RemoveAt(0);

        switch (nextAction) {
            case EnemyActionType.Damage:
                PerformDamageAction();
                break;
            case EnemyActionType.Charge:
                PerformChargeAction();
                break;
            case EnemyActionType.Buff:
                PerformBuffAction();
                break;
            case EnemyActionType.CrowdControl:
                PerformCrowdControlAction();
                break;
        }
    }

    private void PerformDamageAction() {
        // Perform damage action logic here
        Debug.Log("Performing damage action");
        isAttacking = true;
        currentActionCooldown = actionCooldown;
    }

    private void PerformChargeAction() {
        // Perform charge action logic here
        Debug.Log("Performing charge action");
        isMoving = true;
        currentActionCooldown = actionCooldown;
    }

    private void PerformBuffAction() {
        // Perform buff action logic here
        Debug.Log("Performing buff action");
        isAttacking = true;
        currentActionCooldown = actionCooldown;
    }

    private void PerformCrowdControlAction() {
        // Perform crowd control action logic here
        Debug.Log("Performing crowd control action");
        isAttacking = true;
        currentActionCooldown = actionCooldown;
    }

    private void PerformBasicAttack() {
        // Perform basic attack logic here
        Debug.Log("Performing basic attack");
        isAttacking = true;
        currentActionCooldown = actionCooldown;
    }

    private void MoveToTarget() {
        // Move towards the target
        transform.LookAt(target);
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) <= rangeThreshold) {
            isMoving = false;
        }
    }

    private void AttackTarget() {
        // Attack logic here
        // ...
        // When done attacking, set isAttacking to false
        isAttacking = false;
    }

    private void FindTarget() {
        // Logic to find the target (e.g., find the closest vulnerable enemy)
        // Set the target to the found enemy transform
        // ...
    }
}
public class EnemyAII : MonoBehaviour {
    private List<EnemyAction> actionsQueue = new List<EnemyAction>();

    private void Update() {
        if (actionsQueue.Count > 0) {
            EnemyAction currentAction = actionsQueue[0];
            currentAction.DoAction();
            // currentAction.InitializedAttackSpeedValue=

            if (currentAction.IsDone) {
                actionsQueue.RemoveAt(0);
            }
        }
    }

    public void AddActionToQueue(EnemyAction action) {
        actionsQueue.Add(action);
    }
}

public abstract class EnemyAction {
    public bool IsDone { get; protected set; }
    public float InitializedAttackSpeedValue = 9999;
    public float InitializedCastSpeedValue = 9999;
    public float cooldown = 9999;
    public abstract void DoAction();
}

public class DamageAction : EnemyAction {
    public override void DoAction() {
        //if (cooldown<0) {} do section one, then section_one_completed = true;
        //if (cooldown<0 && section_one_completed== true) {} do section two, then section_two_completed = true;
        // Perform damaging action

        IsDone = true;
    }
}

public class ChargeAction : EnemyAction {
    public override void DoAction() {
        // Perform charging action

        IsDone = true;
    }
}

public class BuffAction : EnemyAction {
    public override void DoAction() {
        // Perform buffing action

        IsDone = true;
    }
}

public class CrowdControlAction : EnemyAction {
    public override void DoAction() {
        // Perform crowd control action

        IsDone = true;
    }
}

public class DefendAction : EnemyAction {
    public override void DoAction() {
        // Perform defending action

        IsDone = true;
    }
}
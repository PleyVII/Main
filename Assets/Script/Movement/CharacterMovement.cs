using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour {
    AllObjectInformation allInfo;
    NavMeshAgent Agent { get { return allInfo.agent; } set { allInfo.agent = value; } }
    Transform TargetTransform { get { return allInfo.targetTransform; } set { allInfo.targetTransform = value; } }
    Interactable TargetInteractable { get { return allInfo.targetInteractable; } set { allInfo.targetInteractable = value; } }
    // Queue<Destination> pathQueue = new Queue<Destination>();
    bool usingQueue = false;

    void Start() {
        allInfo = GetComponent<CharacterUpdates>().allInfo;
    }
    void Update() {
        if (allInfo.animationCancelWindow > 0f) allInfo.animationCancelWindow -= Time.deltaTime;

        if (allInfo.CanMove == Agent.isStopped) {
            Agent.isStopped = !allInfo.CanMove;

            if (!Agent.isStopped && Agent.hasPath && allInfo.animationCancelWindow <= 0f) {
                FaceTarget(Agent.destination);
            }
        }

        if (TargetInteractable != null && allInfo.animationCancelWindow <= 0f && !usingQueue) {
            Agent.SetDestination(TargetTransform.position);
            FaceTarget(TargetTransform.position);
        }
        if (Agent.velocity.magnitude < 0.02f) {
            allInfo.RemoveState(State.Moving);
        }
    }


    //     void Update() {
    //     if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)) {
    //         if (pathQueue.Count > 0) {
    //             Destination nextDestination = pathQueue.Dequeue();

    // switch (nextDestination.Type) {
    //     case DestinationType.Point:
    //         agent.SetDestination(nextDestination.Point);
    //         // Handle point-specific logic
    //         break;
    //     case DestinationType.GameObject:
    //         if (nextDestination.TargetObject != null) {
    //             agent.SetDestination(nextDestination.TargetObject.transform.position);
    //             // Handle GameObject-specific logic
    //         }
    //         break;
    //             }
    //         }
    //     }
    // }

    private void MoveToPoint(Vector3 point) {
        Agent.SetDestination(point);
        // usingQueue = false;
        // ResetPathQueue();
        allInfo.animationCancelWindow = 0f;
    }





    #region FollowTarget Overloads
    public void FollowItemTarget(InteractableItem item) {
        if (allInfo.targetInteractable == item) return;
        FaceTarget(item.transform.position);
        AssignTargetVariables(item);
        allInfo.StoppingDistanceCalculate();
        allInfo.ApplyState(State.Moving);
    }

    public void FollowTargetWithoutSkill(AllObjectInformation newTarget) {
        if (allInfo.targetInfo == newTarget) return;
        FollowTargetRepeated(newTarget);
        allInfo.StoppingDistanceCalculate();
    }

    public void FollowTargetWithSkill(AllObjectInformation newTarget, float skillRange) {
        if (allInfo.targetInfo == newTarget) return;
        FollowTargetRepeated(newTarget);
        allInfo.StoppingDistanceCalculate(skillRange);
    }

    public void TargetGroundWithSkill(Vector3 Point, float skillRange) {
        MoveToGroundRepeated(Point);
        allInfo.StoppingDistanceCalculate(skillRange);
    }

    public void MoveToGround(Vector3 Point) {
        MoveToGroundRepeated(Point);
        allInfo.StoppingDistanceCalculate();
    }

    public void FollowTargetRepeated(AllObjectInformation newTarget) {
        allInfo.animationCancelWindow = 0f;
        FaceTarget(newTarget.transform.position);
        AssignTargetVariables(newTarget);
        allInfo.ApplyState(State.Moving);
    }

    public void MoveToGroundRepeated(Vector3 Point) {
        Vector3 position = new Vector3(Mathf.Round(Point.x * 2) / 2, Point.y, Mathf.Round(Point.z * 2) / 2);
        allInfo.animationCancelWindow = 0f;
        FaceTarget(position);
        AssignTargetVariables(position);
        allInfo.ApplyState(State.Moving);
        Agent.SetDestination(position);
    }

    #endregion FollowTarget Overloads

    #region AssignTargetVariables Overloads
    public void AssignTargetVariables(AllObjectInformation target) {
        allInfo.targetInfo = target;
        TargetInteractable = target.interactable;
        TargetTransform = target.transform;
        allInfo.targetGround = null;
        if (TargetInteractable == null) Debug.LogError("Doesn't have the Interactable component on focused target");
        if (allInfo.targetInfo == null) Debug.LogError("Doesn't have the allInfo data on focused target");
    }

    public void AssignTargetVariables(InteractableItem item) {
        allInfo.targetInfo = null;
        TargetInteractable = item;
        TargetTransform = item.transform;
        allInfo.targetGround = null;
        if (TargetInteractable == null) Debug.LogError("Doesn't have the Interactable component on focused target");
    }

    public void AssignTargetVariables(Vector3 target) {
        allInfo.targetInfo = null;
        TargetInteractable = null;
        TargetTransform = null;
        allInfo.targetGround = target;
        if (allInfo.targetGround == null) Debug.LogError("Doesn't have the ground point assigned");
    }

    #endregion AssignTargetVariables Overloads

    public void StopFollowingTarget() {
        NullTargetVariables();
    }

    public void NullTargetVariables() {
        allInfo.targetInfo = null;
        TargetInteractable = null;
        TargetTransform = null;
        allInfo.targetGround = null;
    }

    public void FaceTarget(Vector3 Target) {
        if (Agent.isStopped) return;
        if (Vector3.Distance(transform.position, Target) < 0.1f) return;
        Vector3 direction = (Target - transform.position).normalized;
        if (direction.magnitude > 0f) {
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        }
    }

    public void StopCharacter() {
        Agent.ResetPath(); // Stop the character's movement
    }

    public void PushAgent(AllObjectInformation targetInfo, float pushDistance = 5f) {
        Vector3 startPosition = allInfo.transform.position;
        Vector3 direction = (targetInfo.transform.position - startPosition).normalized;
        Vector3 endPosition = startPosition + direction * pushDistance;

        if (NavMesh.SamplePosition(endPosition, out NavMeshHit navHit, pushDistance, NavMesh.GetAreaFromName("Walkable"))) {
            targetInfo.agent.Warp(navHit.position);
        }
    }
}

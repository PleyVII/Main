// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public enum DestinationType {
//     Point,
//     Interactable
// }

// public class Destination {
//     public Vector3 Point { get; set; }
//     public IInteractable TargetObject { get; set; }
//     public DestinationType Type { get; set; }

//     // Constructor for a point destination
//     public Destination(Vector3 point) {
//         Point = point;
//         Type = DestinationType.Point;
//     }

//     // Constructor for a GameObject destination
//     public Destination(IInteractable targetObject) {
//         TargetObject = targetObject;
//         Type = DestinationType.Interactable;
//     }



    // void Update() {
    //     if (pathQueue.Count == 0) usingQueue = false;
    //     else if (!allInfo.agent.pathPending && allInfo.animationCancelWindow <= 0f && (!allInfo.agent.hasPath || allInfo.agent.remainingDistance <= allInfo.agent.stoppingDistance)) {
    //         usingQueue = true;
    //         Destination nextDestination = pathQueue.Dequeue();

    //         switch (nextDestination.Type) {
    //             case DestinationType.Point:
    //                 allInfo.agent.SetDestination(nextDestination.Point);
    //                 FaceTarget(nextDestination.Point);
    //                 // Handle point-specific logic
    //                 break;
    //             case DestinationType.Interactable:
    //                 if (nextDestination.TargetObject != null) {

    //                 }
    //                 break;
    //         }
    //     }
    // }










    // public void EnqueueDestination(Vector3 point) {
    //     pathQueue.Enqueue(new Destination(point));
    // }

    // public void EnqueueDestination(Interactable targetObject) {
    //     pathQueue.Enqueue(new Destination(targetObject));
    // }

    //     private void ResetPathQueue() {
    //     pathQueue.Clear();
    //     if (allInfo.agent.hasPath) {
    //         allInfo.agent.ResetPath();
    //     }
    // }
// }

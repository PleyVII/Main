// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class StunEffect : StatusEffects
// {
//     private List<float> stunDurations = new List<float>();

//     public void ApplyToStatus(AllObjectInformation targetResistancesInfo, float duration)
//     {
//         stunDurations.Add(duration);
//         targetResistancesInfo.HasStatus = true;
//     }

//     public void UpdateStatus(AllObjectInformation targetResistancesInfo)
//     {
//         if (stunDurations.Count > 0)
//         {
//             float deltaTime = Time.deltaTime;

//             for (int i = stunDurations.Count - 1; i >= 0; i--)
//             {
//                 stunDurations[i] -= deltaTime;

//                 if (stunDurations[i] <= 0f)
//                 {
//                     stunDurations.RemoveAt(i);
//                 }
//             }

//             if (stunDurations.Count == 0)
//             {
//                 targetResistancesInfo.HasStatus = false;
//             }
//         }
//     }
// }public class StatusEffects : MonoBehaviour
// {
//     public Dictionary<Status, StatusEffects> statusDurations = new Dictionary<Status, StatusEffects>();
//     private Dictionary<Status, bool> WasRecentlyHitWithStatus = new Dictionary<Status, bool>();
//     private Coroutine statusCoroutine;

//     public void ApplyToStatus(AllObjectInformation targetResistancesInfo, Status status, float duration)
//     {
//         if (!statusDurations.ContainsKey(status))
//         {
//             statusDurations.Add(status, duration);
//             statusCoroutine = StartCoroutine(StatusCoroutine(targetResistancesInfo, status, duration));
//         }
//         else
//         {
//             statusDurations[status] = duration;
//         }
//     }

//     public bool HasStatus(Status status)
//     {
//         return statusDurations.ContainsKey(status);
//     }

//     public Dictionary<Status, float> AllStatuses
//     {
//         get { return statusDurations; }
//     }

//     public void RemoveStatus(Status status)
//     {
//         statusDurations.Remove(status);
//     }

//     // IEnumerator StatusCoroutine(AllObjectInformation targetResistancesInfo, Status status, float duration)
//     // {
//     //         yield return new WaitForSeconds(duration);
//     //         statusDurations.Remove(status);
//     //         WasRecentlyHitWithStatus[status] = true;
//     //         yield return new WaitForSeconds(duration);
//     //         statusDurations.Remove(status);
//     //         WasRecentlyHitWithStatus[status] = false;
//     // }

//     public void StopCoroutine()
//     {
//         if (statusCoroutine != null)
//         {
//             StopCoroutine(statusCoroutine);
//             statusCoroutine = null;
//         }
//     }
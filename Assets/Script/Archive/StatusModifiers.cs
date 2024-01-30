// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public enum Status
// {
//     None,
//     Stun,
//     Freeze,
//     Silence,
//     Blind,
//     Curse,
//     LexAeterna,
//     Poison,
//     Sleep,
//     SlowCasting,
// }
// public class StatusCoroutine
// {
//     public Coroutine coroutine;
//     public bool HasStatus;
//     public IEnumerator AddStatusCoroutine(AllObjectInformation targetResistancesInfo, Status status, float duration)
//     {
//         HasStatus = true;
//         yield return new WaitForSeconds(duration); //needs target resistancesinfo added
//         HasStatus = false;
//     }
// }
// public class StatusEffects : MonoBehaviour
// {
//     private Dictionary<Status, float> statusDurations = new Dictionary<Status, float>();
//     private Dictionary<Status, bool> WasRecentlyHitWithStatus = new Dictionary<Status, bool>();
//     private Coroutine statusCoroutine;

//     public void ApplyStatus(AllObjectInformation targetResistancesInfo, Status status, float duration)
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
// }

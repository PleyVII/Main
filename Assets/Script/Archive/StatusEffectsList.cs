// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class StatusEffectsList : MonoBehaviour
// {
//     private Dictionary<Status, System.Type> statusDurations = new Dictionary<Status, System.Type>();
//     public Dictionary<Status, StatusEffect> statuses = new Dictionary<Status, StatusEffect>();
//     public List<Status> WhatStatusesIHave = new List<Status>();

//     private void Awake()
//     {
//         InitializeStatusEffects();
//     }

//     private void InitializeStatusEffects()
//     {
//         statusDurations[Status.Stun] = typeof(StunEffect);
//         statusDurations[Status.Freeze] = typeof(FreezeEffect);
//         // Add other status effects to the dictionary here
//     }

//     public void ApplyStatus(AllObjectInformation attackerInformation, AllObjectInformation targetResistancesInfo, Status status, float duration)
//     {
//         if (statusDurations.TryGetValue(status, out System.Type statusEffectType))
//         {
//             StatusEffect newStatusEffect = GetComponent(statusEffectType) as StatusEffect;
//             if (statuses.TryGetValue(status, out StatusEffect statusEffect) == false)
//             {
//                 newStatusEffect = gameObject.AddComponent(statusEffectType) as StatusEffect;
//             }
//             if (newStatusEffect != null)
//             {
//                 statuses.Add(status, newStatusEffect);
//                 WhatStatusesIHave.Add(status);
//                 newStatusEffect.HasStatus = true;
//                 newStatusEffect.ApplyStatus(attackerInformation, targetResistancesInfo, duration);
//             }
//         }
//     }

//     public void RemoveStatus(Status status)
//     {
//         if (statuses.TryGetValue(status, out StatusEffect statusEffect))
//         {
//             Destroy(statusEffect);
//             statuses.Remove(status);
//             WhatStatusesIHave.Remove(status);
//         }
//     }

//     public bool HasStatus(Status status)
//     {
//         return statuses.ContainsKey(status);
//     }
// }
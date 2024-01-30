// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(PlayerStats))]

// public class CharacterCombat : MonoBehaviour
// {
//     CharacterStats myStats;
//     AllObjectInformation objectInformation;
//     public float attackCooldown = 1;
//     public float abilityCooldown = 1;
//     public event System.Action OnAttack;
//     void Start()
//     {
//         objectInformation = GetComponent<NewCharacterStats>().objectInformation;
//     }
//     void Update()
//     {
//         attackCooldown -= Time.deltaTime;
//         abilityCooldown -= Time.deltaTime;
//     }
//     public void TargetedAttack(AllObjectInformation targetStats, bool attack)
//     {
//         {
//             //Enemy is getting attacked with myStats
//             if (attack & attackCooldown<=0) 
//             {
//                 targetStats.GetAttacked(myStats.newatk, 0, 1, Element.Neutral);
//                 attackCooldown = 1/(myStats.newaspd/100);
//                 abilityCooldown = 1/(myStats.newaspd/60);
//                 OnAttack?.Invoke();
//             }
//             if (!attack & abilityCooldown<=0)
//             {
//                 attackCooldown = 1/(myStats.newaspd/60);
//                 abilityCooldown = 1/(myStats.newaspd/100);
//             }
//         }
//     }
// }

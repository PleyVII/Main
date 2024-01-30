// using System.Linq;

// public class EffectModifiersCollection : BaseModifiersCollection<EffectId> {
//     public float CalculateAddedDmgForEveryModifier() {
//         if (allCurrentModifiers.Count == 0) return 0f;
//         return allCurrentModifiers.Sum(x => x.GetFlatModifier());
//     }

//     public float CalculateMultiForEveryModifier() {
//         if (allCurrentModifiers.Count == 0) return 1f;
//         return allCurrentModifiers.Aggregate(1f, (total, next) => total * next.GetMultiplierModifier());
//     }
// }

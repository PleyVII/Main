using System;
using UnityEngine;

namespace DefaultElementMultipliers {
    public static class ElementMultipliers {
        private static float[,] defaultElementMultipliers = new float[Enum.GetNames(typeof(ElementId)).Length, Enum.GetNames(typeof(ElementId)).Length];
        public static float[,] DefaultElementMapCopy {
            get {
                int length0 = defaultElementMultipliers.GetLength(0); // Number of rows
                int length1 = defaultElementMultipliers.GetLength(1); // Number of columns

                float[,] copy = new float[length0, length1];
                Array.Copy(defaultElementMultipliers, copy, defaultElementMultipliers.Length);

                return copy;
            }
        }

        private static void SpecificDamageMultiplierInitialization(ElementId attackingElement, ElementId defendingElement, float multiplier) {
            defaultElementMultipliers[(int)attackingElement, (int)defendingElement] = multiplier;
        }

        public static float GetDefaultElementMultiplier(ElementId attackingElement, ElementId defendingElement) {
            return defaultElementMultipliers[(int)attackingElement, (int)defendingElement];
        }

        [Tooltip("Gives you default Damage Multiplier to Element, filling with defending Element")]
        public static float MultiplierAgainst(this ElementId attackingElement, ElementId defendingElement) {
            return defaultElementMultipliers[(int)attackingElement, (int)defendingElement];
        }

        [Tooltip("Gives you default Damage Multiplier from Element, filling with attacking Element")]
        public static float MultiplierFrom(this ElementId defendingElement, ElementId attackingElement) {
            return defaultElementMultipliers[(int)attackingElement, (int)defendingElement];
        }

        static ElementMultipliers() {
            // Initialize the damage multipliers for each element combination
            InitializeDamageMultipliers();
        }

        #region The initialization for static elements multipliers
        private static void InitializeDamageMultipliers() {
            // I want manual control over multipliers
            // Neutral
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Neutral, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Water, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Earth, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Fire, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Wind, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Holy, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Shadow, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Ghost, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Neutral, ElementId.Undead, 1f);

            // Water
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Neutral, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Water, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Earth, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Fire, 1.5f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Wind, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Holy, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Shadow, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Ghost, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Water, ElementId.Undead, 1f);

            // Earth
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Neutral, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Water, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Earth, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Fire, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Wind, 1.5f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Holy, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Shadow, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Ghost, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Earth, ElementId.Undead, 1f);
            // Add damage multipliers for Earth element

            // Fire
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Neutral, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Water, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Earth, 1.5f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Fire, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Wind, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Holy, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Shadow, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Ghost, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Fire, ElementId.Undead, 1.5f);
            // Add damage multipliers for Fire element

            // Wind
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Neutral, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Water, 1.5f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Earth, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Fire, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Wind, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Holy, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Shadow, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Ghost, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Wind, ElementId.Undead, 1f);
            // Add damage multipliers for Wind element

            // Holy
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Neutral, -0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Water, -1f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Earth, -1f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Fire, -1f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Wind, -1f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Holy, -2f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Shadow, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Ghost, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Holy, ElementId.Undead, 2f);
            // Add damage multipliers for Holy element

            // Shadow
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Neutral, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Water, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Earth, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Fire, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Wind, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Holy, 1.5f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Shadow, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Ghost, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Shadow, ElementId.Undead, 0.75f);
            // Add damage multipliers for Shadow element

            // Ghost
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Neutral, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Water, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Earth, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Fire, 1.5f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Wind, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Holy, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Shadow, 1.5f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Ghost, 2f);
            SpecificDamageMultiplierInitialization(ElementId.Ghost, ElementId.Undead, 1f);
            // Add damage multipliers for Ghost element

            // Undead
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Neutral, 1f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Water, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Earth, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Fire, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Wind, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Holy, 1.25f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Shadow, 0.5f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Ghost, 0.75f);
            SpecificDamageMultiplierInitialization(ElementId.Undead, ElementId.Undead, -2f);
            // Add damage multipliers for Undead element
        }
        #endregion
    }
}
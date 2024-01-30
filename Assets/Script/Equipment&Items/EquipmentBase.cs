using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EquipmentBase : Item {

    [HideInInspector] public List<EquipmentModifier> EquipmentModifiersList = new List<EquipmentModifier>();
    public override void Use(AllObjectInformation allInfo) {
        Debug.Log("Equiping " + name);
        EquipmentManager.Instance.Equip(allInfo, this);
        RemoveFromInventory();
        // equip and remove from inventory
    }

    [Serializable]
    public class EquipmentModifier {
        public bool isDefensiveModifier, isMagicalModifier;
        public EnumType Id;
        public StatId StatId;
        public SizeId SizeId;
        public RaceId RaceId;
        public RankId RankId;
        public ElementId ElementId, IdValue2;
        public EffectId EffectId;
        public ConditionalId ConditionalId;
        public bool isUsingValue2 = false, isUsingOnlyValue2 = false;

        [SerializeField] private float flatValue, percentageValue;

        public float FlatValue {
            get { return flatValue; }
            set { flatValue = Mathf.Clamp(value, -1000, 1000); }
        }

        public float PercentageValue {
            get { return percentageValue; }
            set { percentageValue = Mathf.Clamp(value, -1000, 1000); }
        }

        public bool IsThereAFlatModifier {
            get { return FlatValue != 0; }
        }

        public bool IsThereAPercentageModifier {
            get { return PercentageValue != 0; }
        }

        public Enum IdValue {
            get {
                switch (Id) {
                    case EnumType.StatId: return StatId;
                    case EnumType.SizeId: return SizeId;
                    case EnumType.RaceId: return RaceId;
                    case EnumType.RankId: return RankId;
                    case EnumType.ElementId: return ElementId;
                    case EnumType.EffectId: return EffectId;
                    case EnumType.ConditionalId: return ConditionalId;
                    default: throw new InvalidOperationException("Invalid Type");
                }
            }
            set {
                switch (Id) {
                    case EnumType.StatId: StatId = (StatId)value; break;  // Cast and assign to the specific enum
                    case EnumType.SizeId: SizeId = (SizeId)value; break;  // Cast and assign to the specific enum
                    case EnumType.RaceId: RaceId = (RaceId)value; break;  // Cast and assign to the specific enum
                    case EnumType.RankId: RankId = (RankId)value; break;  // Cast and assign to the specific enum
                    case EnumType.ElementId: ElementId = (ElementId)value; break;  // Cast and assign to the specific enum
                    case EnumType.EffectId: EffectId = (EffectId)value; break;  // Cast and assign to the specific enum
                    case EnumType.ConditionalId: ConditionalId = (ConditionalId)value; break;  // Cast and assign to the specific enum
                    default: throw new InvalidOperationException("Invalid Type");
                }
            }
        }

        [SerializeField] public List<ConditionalModifier> conditions = new List<ConditionalModifier>();

        public bool CheckingConditions(AllObjectInformation character) {
            if (conditions == null) return true;
            foreach (ConditionalModifier condition in conditions) {
                if (!condition.IsConditionMet(character)) return false;
            }
            return true;
        }

        public bool CheckingConditionsWiththisAlreadyHappen(AllObjectInformation character) {
            if (conditions == null) return true;
            foreach (ConditionalModifier condition in conditions) {
                if (!condition.IsConditionMetWithCondition(character)) return false;
            }
            return true;
        }

        public bool CheckingConditionsOnModifierChange(AllObjectInformation character) {
            if (conditions == null) return true;
            foreach (ConditionalModifier condition in conditions) {
                if (condition.ConditionCheckOnModifiersChange(character)) return false;
            }
            return true;
        }

        public void ApplyingthisAlreadyHappenTrue() {
            if (conditions == null) return;
            foreach (ConditionalModifier condition in conditions) {
                condition.UseAfterApplyingAllModifiers();
            }
        }
        public void ApplyingthisAlreadyHappenFalse() {
            if (conditions == null) return;
            foreach (ConditionalModifier condition in conditions) {
                condition.UseAfterRemovingAllModifiers();
            }
        }
    }

    [CustomEditor(typeof(EquipmentBase), true)]
    public class EquipmentEditor : Editor {
        public override void OnInspectorGUI() {
            EquipmentBase equipment = (EquipmentBase)target;

            // Calling the base method to draw the default inspector
            base.OnInspectorGUI();

            // Custom UI for SpecificEquipmentModifiersList
            if (GUILayout.Button("Add Modifier")) {
                equipment.EquipmentModifiersList.Add(new EquipmentModifier());
            }

            for (int i = 0; i < equipment.EquipmentModifiersList.Count; i++) {
                EquipmentModifier modifier = equipment.EquipmentModifiersList[i];

                EditorGUILayout.BeginHorizontal();
                if (modifier.Id != EnumType.StatId) modifier.isDefensiveModifier = EditorGUILayout.Toggle($"Is it a Defensive modifier?", modifier.isDefensiveModifier);
                if ((modifier.Id == EnumType.RaceId || modifier.Id == EnumType.SizeId || modifier.Id == EnumType.RankId) && modifier.isDefensiveModifier == false) modifier.isMagicalModifier = EditorGUILayout.Toggle($"Is it a Magical Modifier?", modifier.isMagicalModifier);
                EditorGUILayout.EndHorizontal();
                // ... Add other properties as needed ...

                // Assuming you have a method to handle the different EnumType selections
                ShowEnumIDField(modifier);

                // Flat and Percentage Values
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Flat Value", GUILayout.Width(70)); // Adjust width as needed
                modifier.FlatValue = EditorGUILayout.FloatField(modifier.FlatValue, GUILayout.Width(50)); // Keep the field short

                // Add flexible space to push the next elements to the right

                GUILayout.Label("Percentage Value", GUILayout.Width(110)); // Adjust width as needed
                modifier.PercentageValue = EditorGUILayout.FloatField(modifier.PercentageValue, GUILayout.Width(50)); // Keep the field short
                EditorGUILayout.EndHorizontal();


                // Field to add new condition
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"Add new Conditions by Drag&Drop", GUILayout.Width(200));
                ConditionalModifier newCondition = (ConditionalModifier)EditorGUILayout.ObjectField(
                    "",
                    null,
                    typeof(ConditionalModifier),
                    false
                );
                EditorGUILayout.EndHorizontal();
                if (newCondition != null) {
                    modifier.conditions.Add(newCondition);
                }
                // Conditions List
                for (int j = 0; j < modifier.conditions.Count; j++) {
                    modifier.conditions[j] = (ConditionalModifier)EditorGUILayout.ObjectField(
                        modifier.conditions[j],
                        typeof(ConditionalModifier),
                        false
                    );
                }
                if (modifier.conditions.Count > 0 && GUILayout.Button("Remove Last Condition")) {
                    modifier.conditions.RemoveAt(modifier.conditions.Count - 1);
                }

                // Provide a button to remove this modifier
                if (GUILayout.Button("Remove Modifier " + (i + 1))) {
                    equipment.EquipmentModifiersList.RemoveAt(i);
                    break; // Break to prevent modifying the list while iterating
                }

                EditorGUILayout.Space(); // Add some spacing between modifiers
            }
        }

        private void ShowEnumIDField(EquipmentModifier modifier) {
            switch (modifier.Id) {
                case EnumType.StatId:
                    DisplayEnumFieldWithLabel(modifier, $"Stat Modifiers to: ", 100);
                    break;
                case EnumType.SizeId:
                    DisplayEnumFieldWithLabel(modifier, $"{SomeText(modifier.isDefensiveModifier, modifier.isMagicalModifier)} Size: ", 220);
                    break;
                case EnumType.RaceId:
                    DisplayEnumFieldWithLabel(modifier, $"{SomeText(modifier.isDefensiveModifier, modifier.isMagicalModifier)} Race: ", 222);
                    break;
                case EnumType.RankId:
                    DisplayEnumFieldWithLabel(modifier, $"{SomeText(modifier.isDefensiveModifier, modifier.isMagicalModifier)} Rank: ", 222);
                    break;
                case EnumType.ElementId:
                    HandleElementIdCase(modifier);
                    break;
                case EnumType.EffectId:
                    DisplayEnumFieldWithLabel(modifier, $"{SomeEffectText(modifier.isDefensiveModifier)} Effect: ", 180);
                    break;
                case EnumType.ConditionalId:
                    DisplayEnumFieldWithLabel(modifier, $"{SomeConditionalText(modifier.isDefensiveModifier)}", 270);
                    break;
                default:
                    break;
            }
        }

        private void DisplayEnumFieldWithLabel(EquipmentModifier modifier, string label, int labelWidth) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelWidth));
            modifier.IdValue = EditorGUILayout.EnumPopup(modifier.IdValue);
            modifier.Id = (EnumType)EditorGUILayout.EnumPopup(modifier.Id);
            EditorGUILayout.EndHorizontal();
        }

        private void HandleElementIdCase(EquipmentModifier modifier) {
            DisplayElementToggleButtons(modifier);
            DisplayElementIdFields(modifier);
        }

        private void DisplayElementToggleButtons(EquipmentModifier modifier) {
            EditorGUILayout.BeginHorizontal();
            modifier.isUsingValue2 = !EditorGUILayout.Toggle($"From All Elements?", !modifier.isUsingValue2);
            if (!modifier.isUsingValue2 && modifier.isUsingOnlyValue2) {
                modifier.isUsingOnlyValue2 = false;
            }
            modifier.isUsingOnlyValue2 = EditorGUILayout.Toggle($"To all Elements?", modifier.isUsingOnlyValue2);
            if (!modifier.isUsingValue2 && modifier.isUsingOnlyValue2) {
                modifier.isUsingValue2 = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayElementIdFields(EquipmentModifier modifier) {

            if (!modifier.isUsingOnlyValue2) {
                DisplayEnumFieldWithLabel(modifier, $"{SomeElementText(modifier.isDefensiveModifier)}", 180);
            }

            if (modifier.isUsingValue2) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{SomeOtherElementText(modifier.isDefensiveModifier)}", GUILayout.Width(195));
                modifier.IdValue2 = (ElementId)EditorGUILayout.EnumPopup(modifier.IdValue2);
                if (modifier.isUsingOnlyValue2) modifier.Id = (EnumType)EditorGUILayout.EnumPopup(modifier.Id);
                EditorGUILayout.EndHorizontal();
            }
        }
        private string SomeConditionalText(bool isDefensiveModifier) {
            if (isDefensiveModifier) return "Damage reduction when you are hit by/during:"; else return "Increased damage when Enemy is hit by/during:";
        }
        private string SomeEffectText(bool isDefensiveModifier) {
            if (isDefensiveModifier) return "Resistance to"; else return "Effectiveness to";
        }
        private string SomeText(bool isDefensiveModifier, bool isMagicalModifier) {
            if (isDefensiveModifier) return "Resistance from all Enemies with"; else if (isMagicalModifier) return "Magical Dmg to Enemies with"; else return "Physical Dmg to Enemies with";
        }
        private string SomeElementText(bool isDefensiveModifier) {
            if (isDefensiveModifier) return "Resistance with Body Element: "; else return "Damage to enemies that are: ";
        }
        private string SomeOtherElementText(bool isDefensiveModifier) {
            if (isDefensiveModifier) return "Applies when being attacked with: "; else return "Applies when attacking with: ";
        }
    }
}


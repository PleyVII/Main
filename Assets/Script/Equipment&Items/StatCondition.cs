using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "StatCondition", menuName = "ModifiersConditions/StatCondition")]
public class StatCondition : ConditionalModifier {
    public List<StatRequirement> requirements = new List<StatRequirement>();

    public enum ConditionType {
        // Fixed-value comparisons
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        EqualTo,
        NotEqualTo,
        TimesGreaterThan,
        TimesLesserThan,
        TimesGreaterThanOrEqualTo,
        TimesLesserThanOrEqualTo,
        TimesEqualTo,
        TimesNotEqualTo
    }

    [System.Serializable]
    public class StatRequirement {
        public ConditionType conditionType;
        public float value;
        public float multiplier = 1f;
        public EnumType enumType1, enumType2;
        public StatId statId1, statId2;
        public SizeId sizeId1, sizeId2;
        public RaceId raceId1, raceId2;
        public RankId rankId1, rankId2;
        public ElementId elementId1, elementId2;
        public EffectId effectId1, effectId2;
        public ConditionalId conditionalId1, conditionalId2;
        public bool isPercentageModifier1 = false, isPercentageModifier2 = false;
        public bool isDefensiveModifier1 = false, isDefensiveModifier2 = false;
        public bool isMagicalModifier1 = false, isMagicalModifier2 = false;
    }

    public override bool IsConditionMet(AllObjectInformation character) {
        return ComparisonCalculations(character) || isAlreadyHappen;
    }

    public override bool IsConditionMetWithCondition(AllObjectInformation character) {
        otherCheck = ComparisonCalculations(character);

        if (otherCheck) isAlreadyHappen = true;
        bool result = isAlreadyHappen || otherCheck;
        if (result) isAlreadyHappen = false;

        return result;
    }

    public override bool ConditionCheckOnModifiersChange(AllObjectInformation character) {
        return ComparisonCalculations(character);
    }

    private bool ComparisonCalculations(AllObjectInformation character) {
        foreach (StatRequirement requirement in requirements) {
            float firstValue = FigureOutFirstEnumTypeValue(requirement.enumType1, requirement, character);
            //SecondValue function is here just because its not always used, so using a function when use it when needed, 
            //and because its gonna be only one case, its gonna be used maximum one time
            float SecondValue() {
                return FigureOutSecondEnumTypeValue(requirement.enumType2, requirement, character);
            }

            switch (requirement.conditionType) {
                case ConditionType.GreaterThan:
                    if (!(firstValue > requirement.value))
                        return false;
                    break;
                case ConditionType.LessThan:
                    if (!(firstValue < requirement.value))
                        return false;
                    break;
                case ConditionType.GreaterThanOrEqualTo:
                    if (!(firstValue >= requirement.value))
                        return false;
                    break;
                case ConditionType.LessThanOrEqualTo:
                    if (!(firstValue <= requirement.value))
                        return false;
                    break;
                case ConditionType.EqualTo:
                    if (!(firstValue == requirement.value))
                        return false;
                    break;
                case ConditionType.NotEqualTo:
                    if (!(firstValue != requirement.value))
                        return false;
                    break;
                case ConditionType.TimesGreaterThan:
                    if (!(firstValue * requirement.multiplier > SecondValue()))
                        return false;
                    break;
                case ConditionType.TimesLesserThan:
                    if (!(firstValue < SecondValue() * requirement.multiplier))
                        return false;
                    break;
                case ConditionType.TimesGreaterThanOrEqualTo:
                    if (!(firstValue * requirement.multiplier >= SecondValue()))
                        return false;
                    break;
                case ConditionType.TimesLesserThanOrEqualTo:
                    if (!(firstValue <= SecondValue() * requirement.multiplier))
                        return false;
                    break;
                case ConditionType.TimesEqualTo:
                    if (!(firstValue == SecondValue()))
                        return false;
                    break;
                case ConditionType.TimesNotEqualTo:
                    if (!(firstValue != SecondValue()))
                        return false;
                    break;
                    // ... any other condition types ...
            }
        }
        return true;
    }

    private float FigureOutFirstEnumTypeValue(EnumType enumType, StatRequirement modifier, AllObjectInformation allInfo) {
        bool isPercentageValue = modifier.isPercentageModifier1;
        bool isDefensiveModifier = modifier.isDefensiveModifier1;
        bool isMagicalModifier = modifier.isMagicalModifier1;

        switch (enumType) {
            case EnumType.StatId:
                return allInfo.Stats.GetStatValueByStatId(modifier.statId1);

            case EnumType.SizeId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.SizeDefense.CalculatePercentageSumFor(modifier.sizeId1)
                                             : allInfo.SizeDefense.CalculateAddedDmgFor(modifier.sizeId1);
                else
                    return isMagicalModifier ? (isPercentageValue ? allInfo.SizeMagicalModifiers.CalculatePercentageSumFor(modifier.sizeId1)
                                                                  : allInfo.SizeMagicalModifiers.CalculateAddedDmgFor(modifier.sizeId1))
                                             : (isPercentageValue ? allInfo.SizePhysicalModifiers.CalculatePercentageSumFor(modifier.sizeId1)
                                                                  : allInfo.SizePhysicalModifiers.CalculateAddedDmgFor(modifier.sizeId1));
            case EnumType.RaceId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.RaceDefense.CalculatePercentageSumFor(modifier.raceId1)
                                             : allInfo.RaceDefense.CalculateAddedDmgFor(modifier.raceId1);
                else
                    return isMagicalModifier ? (isPercentageValue ? allInfo.RaceMagicalModifiers.CalculatePercentageSumFor(modifier.raceId1)
                                                                  : allInfo.RaceMagicalModifiers.CalculateAddedDmgFor(modifier.raceId1))
                                             : (isPercentageValue ? allInfo.RacePhysicalModifiers.CalculatePercentageSumFor(modifier.raceId1)
                                                                  : allInfo.RacePhysicalModifiers.CalculateAddedDmgFor(modifier.raceId1));
            case EnumType.RankId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.RankDefense.CalculatePercentageSumFor(modifier.rankId1)
                                             : allInfo.RankDefense.CalculateAddedDmgFor(modifier.rankId1);
                else
                    return isMagicalModifier ? (isPercentageValue ? allInfo.RankMagicalModifiers.CalculatePercentageSumFor(modifier.rankId1)
                                                                  : allInfo.RankMagicalModifiers.CalculateAddedDmgFor(modifier.rankId1))
                                             : (isPercentageValue ? allInfo.RankPhysicalModifiers.CalculatePercentageSumFor(modifier.rankId1)
                                                                  : allInfo.RankPhysicalModifiers.CalculateAddedDmgFor(modifier.rankId1));
            case EnumType.ElementId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.ElementDefense.CalculatePercentageSumFor(modifier.elementId1)
                                             : allInfo.ElementDefense.CalculateAddedDmgFor(modifier.elementId1);
                else
                    return isPercentageValue ? allInfo.ElementModifiers.CalculatePercentageSumFor(modifier.elementId1)
                                             : allInfo.ElementModifiers.CalculateAddedDmgFor(modifier.elementId1);

            case EnumType.EffectId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.EffectDefense.CalculatePercentageSumFor(modifier.effectId1)
                                             : allInfo.EffectDefense.CalculateAddedDmgFor(modifier.effectId1);
                else
                    return isPercentageValue ? allInfo.EffectModifiers.CalculatePercentageSumFor(modifier.effectId1)
                                             : allInfo.EffectModifiers.CalculateAddedDmgFor(modifier.effectId1);

            case EnumType.ConditionalId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.EffectDefense.CalculatePercentageSumFor(modifier.conditionalId1)
                                             : allInfo.EffectDefense.CalculateAddedDmgFor(modifier.conditionalId1);
                else
                    return isPercentageValue ? allInfo.EffectModifiers.CalculatePercentageSumFor(modifier.conditionalId1)
                                             : allInfo.EffectModifiers.CalculateAddedDmgFor(modifier.conditionalId1);
            default:
                return 0f; // Default return for undefined EnumType
        }
    }

    private float FigureOutSecondEnumTypeValue(EnumType enumType, StatRequirement modifier, AllObjectInformation allInfo) {
        bool isPercentageValue = modifier.isPercentageModifier2;
        bool isDefensiveModifier = modifier.isDefensiveModifier2;
        bool isMagicalModifier = modifier.isMagicalModifier2;

        switch (enumType) {
            case EnumType.StatId:
                return allInfo.Stats.GetStatValueByStatId(modifier.statId2);

            case EnumType.SizeId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.SizeDefense.CalculatePercentageSumFor(modifier.sizeId2)
                                             : allInfo.SizeDefense.CalculateAddedDmgFor(modifier.sizeId2);
                else
                    return isMagicalModifier ? (isPercentageValue ? allInfo.SizeMagicalModifiers.CalculatePercentageSumFor(modifier.sizeId2)
                                                                  : allInfo.SizeMagicalModifiers.CalculateAddedDmgFor(modifier.sizeId2))
                                             : (isPercentageValue ? allInfo.SizePhysicalModifiers.CalculatePercentageSumFor(modifier.sizeId2)
                                                                  : allInfo.SizePhysicalModifiers.CalculateAddedDmgFor(modifier.sizeId2));
            case EnumType.RaceId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.RaceDefense.CalculatePercentageSumFor(modifier.raceId2)
                                             : allInfo.RaceDefense.CalculateAddedDmgFor(modifier.raceId2);
                else
                    return isMagicalModifier ? (isPercentageValue ? allInfo.RaceMagicalModifiers.CalculatePercentageSumFor(modifier.raceId2)
                                                                  : allInfo.RaceMagicalModifiers.CalculateAddedDmgFor(modifier.raceId2))
                                             : (isPercentageValue ? allInfo.RacePhysicalModifiers.CalculatePercentageSumFor(modifier.raceId2)
                                                                  : allInfo.RacePhysicalModifiers.CalculateAddedDmgFor(modifier.raceId2));
            case EnumType.RankId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.RankDefense.CalculatePercentageSumFor(modifier.rankId2)
                                             : allInfo.RankDefense.CalculateAddedDmgFor(modifier.rankId2);
                else
                    return isMagicalModifier ? (isPercentageValue ? allInfo.RankMagicalModifiers.CalculatePercentageSumFor(modifier.rankId2)
                                                                  : allInfo.RankMagicalModifiers.CalculateAddedDmgFor(modifier.rankId2))
                                             : (isPercentageValue ? allInfo.RankPhysicalModifiers.CalculatePercentageSumFor(modifier.rankId2)
                                                                  : allInfo.RankPhysicalModifiers.CalculateAddedDmgFor(modifier.rankId2));
            case EnumType.ElementId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.ElementDefense.CalculatePercentageSumFor(modifier.elementId2)
                                             : allInfo.ElementDefense.CalculateAddedDmgFor(modifier.elementId2);
                else
                    return isPercentageValue ? allInfo.ElementModifiers.CalculatePercentageSumFor(modifier.elementId2)
                                             : allInfo.ElementModifiers.CalculateAddedDmgFor(modifier.elementId2);

            case EnumType.EffectId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.EffectDefense.CalculatePercentageSumFor(modifier.effectId2)
                                             : allInfo.EffectDefense.CalculateAddedDmgFor(modifier.effectId2);
                else
                    return isPercentageValue ? allInfo.EffectModifiers.CalculatePercentageSumFor(modifier.effectId2)
                                             : allInfo.EffectModifiers.CalculateAddedDmgFor(modifier.effectId2);

            case EnumType.ConditionalId:
                if (isDefensiveModifier)
                    return isPercentageValue ? allInfo.EffectDefense.CalculatePercentageSumFor(modifier.conditionalId2)
                                             : allInfo.EffectDefense.CalculateAddedDmgFor(modifier.conditionalId2);
                else
                    return isPercentageValue ? allInfo.EffectModifiers.CalculatePercentageSumFor(modifier.conditionalId2)
                                             : allInfo.EffectModifiers.CalculateAddedDmgFor(modifier.conditionalId2);

            default:
                return 0f; // Default return for undefined EnumType
        }
    }







    [CustomEditor(typeof(StatCondition))]
    public class StatConditionEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            StatCondition statCondition = (StatCondition)target;

            // Button to add new requirements
            if (GUILayout.Button("Add Requirement")) {
                statCondition.requirements.Add(new StatRequirement());
            }


            // Create a UI for each requirement
            for (int i = 0; i < statCondition.requirements.Count; i++) {
                StatRequirement requirement = statCondition.requirements[i];
                ShowingUpToggles1(requirement.enumType1);
                ForEnumType1(requirement.enumType1);
                ConditionSwitch();






                void ConditionSwitch() {
                    switch (requirement.conditionType) {
                        case ConditionType.GreaterThan:
                        case ConditionType.LessThan:
                        case ConditionType.GreaterThanOrEqualTo:
                        case ConditionType.LessThanOrEqualTo:
                        case ConditionType.EqualTo:
                        case ConditionType.NotEqualTo:
                            IdToValueCases();
                            break;
                        case ConditionType.TimesGreaterThan:
                        case ConditionType.TimesGreaterThanOrEqualTo:
                        case ConditionType.TimesLesserThan:
                        case ConditionType.TimesLesserThanOrEqualTo:
                        case ConditionType.TimesEqualTo:
                        case ConditionType.TimesNotEqualTo:
                            ForEnumType2(requirement.enumType2);
                            ShowingUpToggles2(requirement.enumType2);
                            break;
                    }
                }

                void IdToValueCases() {
                    GUILayout.BeginHorizontal();
                    requirement.conditionType = (ConditionType)EditorGUILayout.EnumPopup(requirement.conditionType);
                    requirement.value = EditorGUILayout.FloatField(requirement.value);
                    GUILayout.EndHorizontal();
                }

                void ForEnumType1(EnumType enumType) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(ConditionTypeLabel(true), GUILayout.Width(200));
                    switch (enumType) {
                        case EnumType.StatId:
                            requirement.statId1 = (StatId)EditorGUILayout.EnumPopup(requirement.statId1);
                            break;

                        case EnumType.SizeId:
                            requirement.sizeId1 = (SizeId)EditorGUILayout.EnumPopup(requirement.sizeId1);
                            break;

                        case EnumType.RaceId:
                            requirement.raceId1 = (RaceId)EditorGUILayout.EnumPopup(requirement.raceId1);
                            break;

                        case EnumType.RankId:
                            requirement.rankId1 = (RankId)EditorGUILayout.EnumPopup(requirement.rankId1);
                            break;

                        case EnumType.ElementId:
                            requirement.elementId1 = (ElementId)EditorGUILayout.EnumPopup(requirement.elementId1);
                            break;

                        case EnumType.EffectId:
                            requirement.effectId1 = (EffectId)EditorGUILayout.EnumPopup(requirement.effectId1);
                            break;

                        case EnumType.ConditionalId:
                            requirement.conditionalId1 = (ConditionalId)EditorGUILayout.EnumPopup(requirement.conditionalId1);
                            break;

                        // ... add cases for any other EnumTypes ...

                        default:
                            EditorGUILayout.LabelField("Select an Enum Type");
                            break;
                    }
                    requirement.enumType1 = (EnumType)EditorGUILayout.EnumPopup(requirement.enumType1);
                    GUILayout.EndHorizontal();
                }

                void ForEnumType2(EnumType enumType) {
                    GUILayout.BeginHorizontal();
                    requirement.multiplier = EditorGUILayout.FloatField(requirement.multiplier, GUILayout.Width(50));
                    requirement.conditionType = (ConditionType)EditorGUILayout.EnumPopup(requirement.conditionType);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(ConditionTypeLabel(false), GUILayout.Width(200));
                    switch (enumType) {
                        case EnumType.StatId:
                            requirement.statId2 = (StatId)EditorGUILayout.EnumPopup(requirement.statId2);
                            break;

                        case EnumType.SizeId:
                            requirement.sizeId2 = (SizeId)EditorGUILayout.EnumPopup(requirement.sizeId2);
                            break;

                        case EnumType.RaceId:
                            requirement.raceId2 = (RaceId)EditorGUILayout.EnumPopup(requirement.raceId2);
                            break;

                        case EnumType.RankId:
                            requirement.rankId2 = (RankId)EditorGUILayout.EnumPopup(requirement.rankId2);
                            break;

                        case EnumType.ElementId:
                            requirement.elementId2 = (ElementId)EditorGUILayout.EnumPopup(requirement.elementId2);
                            break;

                        case EnumType.EffectId:
                            requirement.effectId2 = (EffectId)EditorGUILayout.EnumPopup(requirement.effectId2);
                            break;

                        case EnumType.ConditionalId:
                            requirement.conditionalId2 = (ConditionalId)EditorGUILayout.EnumPopup(requirement.conditionalId2);
                            break;


                        // ... add cases for any other EnumTypes ...

                        default:
                            EditorGUILayout.LabelField("Select an Enum Type");
                            break;
                    }
                    requirement.enumType2 = (EnumType)EditorGUILayout.EnumPopup(requirement.enumType2);
                    GUILayout.EndHorizontal();
                }

                void ShowingUpToggles1(EnumType enumType) {

                    if (enumType == EnumType.StatId) return;
                    GUILayout.BeginHorizontal();
                    requirement.isDefensiveModifier1 = EditorGUILayout.Toggle("Is it a Defensive Modifier?", requirement.isDefensiveModifier1);
                    if ((enumType == EnumType.RaceId || enumType == EnumType.SizeId || enumType == EnumType.RankId) && requirement.isDefensiveModifier1 == false) {
                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); requirement.isMagicalModifier1 = EditorGUILayout.Toggle("Is it a Magical Modifier?", requirement.isMagicalModifier1);
                    }
                    requirement.isPercentageModifier1 = EditorGUILayout.Toggle("Is it a Percentage Modifier?", requirement.isPercentageModifier1); GUILayout.EndHorizontal();
                }
                void ShowingUpToggles2(EnumType enumType) {
                    if (enumType == EnumType.StatId) return;
                    GUILayout.BeginHorizontal(); requirement.isDefensiveModifier2 = EditorGUILayout.Toggle("Is it a Defensive Modifier?", requirement.isDefensiveModifier2);
                    if ((enumType == EnumType.RaceId || enumType == EnumType.SizeId || enumType == EnumType.RankId) && requirement.isDefensiveModifier2 == false) {
                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); requirement.isMagicalModifier2 = EditorGUILayout.Toggle("Is it a Magical Modifier?", requirement.isMagicalModifier2);
                    }
                    requirement.isPercentageModifier2 = EditorGUILayout.Toggle("Is it a Percentage Modifier?", requirement.isPercentageModifier2); GUILayout.EndHorizontal();
                }

                string ConditionTypeLabel(bool isFirstEnumType) {
                    EnumType enumType;
                    if (isFirstEnumType) enumType = requirement.enumType1; else enumType = requirement.enumType2;
                    bool isDefensiveModifier = isFirstEnumType ? requirement.isDefensiveModifier1 : requirement.isDefensiveModifier2;
                    bool isMagicalModifier = isFirstEnumType ? requirement.isMagicalModifier1 : requirement.isMagicalModifier2;
                    bool isPercentageModifier = isFirstEnumType ? requirement.isPercentageModifier1 : requirement.isPercentageModifier2;
                    string label = "";
                    if (isDefensiveModifier) label = "Damage Reduction to:";
                    else {
                        label += "Damage Modifier to:";
                        if (enumType == EnumType.SizeId || enumType == EnumType.RaceId || enumType == EnumType.RankId) {
                            if (isMagicalModifier) label = "Magical Damage Modifier to:"; else label = "Physical Damage Modifier to:";
                        }
                    }
                    if (isPercentageModifier) label = "Percent " + label; else label = "Flat " + label;
                    return label;
                }

                // Provide a button to remove this requirement
                if (GUILayout.Button("Remove Requirement " + (i + 1))) {
                    statCondition.requirements.RemoveAt(i);
                    break; // Break to prevent modifying the list while iterating
                }

                EditorGUILayout.Space(); // Add some spacing between requirements
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

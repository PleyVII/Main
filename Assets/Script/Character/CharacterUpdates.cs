using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI; // Needed for List

[RequireComponent(typeof(CharacterMovement))]
public class CharacterUpdates : MonoBehaviour {
    bool onRegenHealth = false;
    bool onRegenMana = false;
    public string playerName = "Test";
    public EquipmentWrapper dataWrapper;
    public AllObjectInformation allInfo;

    void Awake() {
        allInfo = ScriptableObject.CreateInstance<AllObjectInformation>();
        allInfo.Initialize(gameObject);
        if (allInfo == null) Debug.LogError("Failed to create an instance of AllObjectInformation at the startup.");
        if (allInfo.Owner == null) Debug.LogError("The Owner of the GameObject is null");
        allInfo.agent = GetComponent<NavMeshAgent>();
        allInfo.transform = allInfo.agent.transform;
        allInfo.agent.updateRotation = false;
        allInfo.interactable = GetComponent<Interactable>();



        InitializeDataWrapper();
    }

    void InitializeDataWrapper() {
        if (dataWrapper == null) { Debug.LogWarning("Need to put datawrapper"); return; }
        allInfo = ScriptableObject.CreateInstance<AllObjectInformation>();
        allInfo.Initialize(gameObject);

        HashSet<Equipment> processedEquipment = new HashSet<Equipment>();

        for (int i = 0; i < dataWrapper.currentEquipment.Length; i++) {
            Equipment originalEquipment = dataWrapper.currentEquipment[i];

            // Check if the equipment has already been processed
            if (originalEquipment != null && !processedEquipment.Contains(originalEquipment)) {
                Equipment temporaryEquipment = Instantiate(originalEquipment);
                processedEquipment.Add(originalEquipment);

                // Iterate over the card list for this equipment and add cards to the temporary equipment
                for (int j = 0; j < dataWrapper.cardLists[i].Count; j++) {
                    Card temporaryCard = Instantiate(dataWrapper.cardLists[i][j]);
                    temporaryEquipment.AddCard(temporaryCard, false);
                }
                temporaryEquipment.Use(allInfo);
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            allInfo.DealPhysicalDamage(allInfo, 1000);
        }
        if (allInfo.CurrentHealth < allInfo.Stats.MaxHealth && onRegenHealth == false) {
            onRegenHealth = true;
            StartCoroutine("RegenHealth");
        }
        if (allInfo.CurrentMana < allInfo.Stats.MaxMana && onRegenMana == false) {
            onRegenMana = true;
            StartCoroutine("RegenMana");
        }
    }

    IEnumerator RegenHealth() {
        yield return new WaitForSeconds(5);
        onRegenHealth = false;
        allInfo.CurrentHealth += allInfo.Stats.HealthRegenPer5;
    }

    IEnumerator RegenMana() {
        yield return new WaitForSeconds(5);
        onRegenMana = false;
        allInfo.CurrentMana += allInfo.Stats.ManaRegenPer5;
    }




    #region Equiping
    void Start() {
        EquipmentManager.Instance.onEquipmentChanged += OnEquipmentChanged;
        EquipmentManager.Instance.onModifiersChangeCheck += CheckingConditionsOnModifierChange;
    }
    public void OnEquipmentChanged(AllObjectInformation character, EquipmentBase newItem, EquipmentBase oldItem) {
        if (character != allInfo.Owner) return;
        if (oldItem != null) {
            RemoveModifiersWithConditionSimulation(oldItem.EquipmentModifiersList);
            if (oldItem is Armor) allInfo.Stats.RemoveBaseValuesFromArmour((Armor)oldItem);
            if (oldItem is Card) allInfo.Stats.RemoveBodyElement((Card)oldItem);
            if (oldItem is Weapon) allInfo.Stats.RemoveBaseWeaponAtk((Weapon)oldItem);
        }

        if (newItem != null) {
            AddModifiersWithConditionSimulation(newItem.EquipmentModifiersList);
            if (newItem is Armor) allInfo.Stats.AddBaseValuesFromArmour((Armor)oldItem);
            if (newItem is Card) allInfo.Stats.ChangeBodyElement((Card)oldItem);
            if (newItem is Weapon) allInfo.Stats.ChangeBaseWeaponAtk((Weapon)oldItem);
        }
    }

    private void CheckingConditionsOnModifierChange(AllObjectInformation character, List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null && listOfModifiers.Count == 0) return;
        foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {

            if (!specificModifier.CheckingConditionsOnModifierChange(character)) ChecksAndRemovesModifier(specificModifier);
        }
    }

    private void AddModifiersWithConditionSimulation(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null && listOfModifiers.Count == 0) return;
        //Tries to see if the conditions will be meet with adding stats, if no, reverts and adds it one by one instead
        //Made so that if people would meet their requirements after equiping, they will now.
        //But unfortunetly it works only if all conditions will go through, won't work if there is other condition from one item that is failing.
        AddModifierLoopWithoutCondition(listOfModifiers);
        if (ConditionsForAddingLoop(listOfModifiers)) return;
        else {
            RemoveModifierLoopWithoutCondition(listOfModifiers);
            AddModifierLoopWithCondition(listOfModifiers);
        }
    }

    private void RemoveModifiersWithConditionSimulation(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null || listOfModifiers.Count == 0) return;
        //Tries to see if the conditions will be meet with removing stats, if no, reverts and adds it one by one instead,
        // probably kinda pointless check for the removal part, but shouldnt hurt.
        RemoveModifierLoopWithoutCondition(listOfModifiers);
        if (ConditionsForRemovingLoop(listOfModifiers)) return;
        else {
            AddModifierLoopWithoutCondition(listOfModifiers);
            RemoveModifiersWithisAlreadyHappen(listOfModifiers);
        }
    }

    private bool ConditionsForAddingLoop(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null || listOfModifiers.Count == 0) {
            foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
                if (!specificModifier.CheckingConditions(allInfo)) return false;
            }
            foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
                specificModifier.ApplyingthisAlreadyHappenTrue();
            }
        }
        return true;
    }

    private bool ConditionsForRemovingLoop(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null || listOfModifiers.Count == 0) {
            foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
                if (!specificModifier.CheckingConditions(allInfo)) return false;
            }
            foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
                specificModifier.ApplyingthisAlreadyHappenFalse();
            }
        }
        return true;
    }

    private void AddModifierLoopWithoutCondition(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null || listOfModifiers.Count == 0) return;
        foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
            AddModifier(specificModifier);
        }
    }

    private void RemoveModifierLoopWithoutCondition(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null || listOfModifiers.Count == 0) return;
        foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
            RemoveModifier(specificModifier);
        }
    }
    private void AddModifierLoopWithCondition(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null || listOfModifiers.Count == 0) return;
        foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
            if (specificModifier.CheckingConditionsWiththisAlreadyHappen(allInfo)) AddModifier(specificModifier);
        }
    }

    private void RemoveModifiersWithisAlreadyHappen(List<EquipmentBase.EquipmentModifier> listOfModifiers) {
        if (listOfModifiers == null || listOfModifiers.Count == 0) return;
        foreach (EquipmentBase.EquipmentModifier specificModifier in listOfModifiers) {
            if (specificModifier.CheckingConditionsWiththisAlreadyHappen(allInfo)) RemoveModifier(specificModifier);
        }
    }

    private void ChecksAndAddsModifier(EquipmentBase.EquipmentModifier specificModifier) //Not needed for now, or maybe never will, because I am just doing the Remove counterpart for condition checking 
    {
        if (specificModifier == null) return;
        if (specificModifier.CheckingConditionsWiththisAlreadyHappen(allInfo)) AddModifier(specificModifier);
    }

    private void ChecksAndRemovesModifier(EquipmentBase.EquipmentModifier specificModifier) {
        if (specificModifier == null) return;
        if (specificModifier.CheckingConditionsWiththisAlreadyHappen(allInfo)) RemoveModifier(specificModifier);
    }

    private void AddModifier(EquipmentBase.EquipmentModifier specificModifier) {
        if (specificModifier == null) return;
        if (specificModifier.IsThereAFlatModifier) ModifiersOperations.AddFlatModifier(specificModifier, allInfo);
        if (specificModifier.IsThereAPercentageModifier) ModifiersOperations.AddPercentageModifier(specificModifier, allInfo);
    }

    private void RemoveModifier(EquipmentBase.EquipmentModifier specificModifier) {
        if (specificModifier == null) return;
        if (specificModifier.IsThereAFlatModifier) ModifiersOperations.RemoveFlatModifier(specificModifier, allInfo);
        if (specificModifier.IsThereAPercentageModifier) ModifiersOperations.RemovePercentageModifier(specificModifier, allInfo);
    }
    #endregion Equiping
}

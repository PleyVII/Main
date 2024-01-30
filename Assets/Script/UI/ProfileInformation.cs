using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileInformation : MonoBehaviour {
    #region GameObjectsAndVariables
    public GameObject textPlayerCharacterName;
    public GameObject textPlayerClass;
    public GameObject textCurrentHP;
    public GameObject textMaxHP;
    public GameObject textCurrentMP;
    public GameObject textMaxMP;
    public GameObject textHPPercent;
    public Slider textHPPercentBar;
    public GameObject textMPPercent;
    public Slider textMPPercentBar;
    public GameObject textBaseLVL;
    public GameObject textBaseExpCurrent;
    public GameObject textBaseExpToLVL;
    public Slider textBaseExpBar;
    public GameObject textJobLVL;
    public GameObject textJobExpCurrent;
    public GameObject textJobExpToLVL;
    public Slider textJobExpBar;
    public string playerCharacterName;
    public string playerClass;
    public float currentHP;
    public float maxHP;
    public float currentMP;
    public float maxMP;
    public float hPPercent = 100;
    public float mPPercent = 100;
    public float baseLVL;
    public float baseExpCurrent;
    public float baseExpToLVL;
    public float baseExpBar = 100;
    public float jobLVL;
    public float jobExpCurrent;
    public float jobExpToLVL;
    public float jobExpBar = 100;
    #endregion
    public void Start() {
        hPPercent = Mathf.Round(currentHP / maxHP * 100);
        mPPercent = Mathf.Round(currentMP / maxMP * 100);
        baseExpBar = baseExpCurrent / baseExpToLVL * 100;
        jobExpBar = jobExpCurrent / jobExpToLVL * 100;
    }
    public void ChangingStatText(GameObject Object, float Number) {
        Object.GetComponent<TextMeshProUGUI>().text = Number.ToString("0");
    }
    public void GiveMeYourStats2(string playerCharacterNameT, string playerClassT, float maxHPT, float maxMPT, float currentHPT, float currentMPT) {
        playerCharacterName = playerCharacterNameT;
        playerClass = playerClassT;
        maxHP = maxHPT;
        maxMP = maxMPT;
        currentHP = currentHPT;
        currentMP = currentMPT;

        textPlayerCharacterName.GetComponent<TextMeshProUGUI>().text = playerCharacterName;
        textPlayerClass.GetComponent<TextMeshProUGUI>().text = playerClass;
        ChangingStatText(textCurrentHP, currentHP);
        ChangingStatText(textCurrentMP, currentMP);
        ChangingStatText(textMaxHP, maxHP);
        ChangingStatText(textMaxMP, maxMP);
        ChangingStatText(textHPPercent, hPPercent);
        ChangingStatText(textMPPercent, mPPercent);
        textHPPercentBar.value = currentHP / maxHP;
        textMPPercentBar.value = currentMP / maxMP;
    }
    public void UpdateHealthUpdateManaUIUpdate(float HP, float MP) {
        currentHP = HP;
        currentMP = MP;
        hPPercent = Mathf.Round(currentHP / maxHP * 100);
        mPPercent = Mathf.Round(currentMP / maxMP * 100);
        ChangingStatText(textCurrentHP, currentHP);
        ChangingStatText(textCurrentMP, currentMP);
        ChangingStatText(textHPPercent, hPPercent);
        ChangingStatText(textMPPercent, mPPercent);
        textHPPercentBar.value = currentHP / maxHP;
        textMPPercentBar.value = currentMP / maxMP;
    }
    public void LvLUpUIUpdate(float maxHPT, float maxMPT, float currentHPT, float currentMPT, float baseLeveL, float jobLevel, float baseCurrent, float jobCurrent, float baseToLVL, float jobToLVL) {
        baseLVL = baseLeveL;
        baseExpCurrent = baseCurrent;
        baseExpToLVL = baseToLVL;
        jobLVL = jobLevel;
        jobExpCurrent = jobCurrent;
        jobExpToLVL = jobToLVL;
        maxHP = maxHPT;
        maxMP = maxMPT;
        currentHP = currentHPT;
        currentMP = currentMPT;
        ChangingStatText(textHPPercent, hPPercent);
        ChangingStatText(textMPPercent, mPPercent);
        ChangingStatText(textBaseLVL, baseLVL);
        ChangingStatText(textBaseExpCurrent, baseExpCurrent);
        ChangingStatText(textJobLVL, jobLVL);
        ChangingStatText(textJobExpCurrent, jobExpCurrent);
        ChangingStatText(textMaxHP, maxHP);
        ChangingStatText(textMaxMP, maxMP);
        ChangingStatText(textCurrentHP, currentHP);
        ChangingStatText(textCurrentMP, currentMP);
        textHPPercentBar.value = currentHP / maxHP;
        textMPPercentBar.value = currentMP / maxMP;
        hPPercent = Mathf.Round(currentHP / maxHP * 100);
        mPPercent = Mathf.Round(currentMP / maxMP * 100);
        baseExpBar = baseExpCurrent / baseExpToLVL;
        jobExpBar = jobExpCurrent / jobExpToLVL;
    }
    public void KillMonsterExpUIUpdate(float baseLVLT, float baseExpT, float baseExpMaxT, float jobLVLT, float jobExpT, float jobExpMaxT) {
        baseLVL = baseLVLT;
        baseExpCurrent = baseExpT;
        baseExpToLVL = baseExpMaxT;
        jobLVL = jobLVLT;
        jobExpCurrent = jobExpT;
        jobExpToLVL = jobExpMaxT;
        ChangingStatText(textJobLVL, jobLVL);
        ChangingStatText(textJobExpCurrent, jobExpCurrent);
        ChangingStatText(textBaseLVL, baseLVL);
        ChangingStatText(textBaseExpCurrent, baseExpCurrent);
        ChangingStatText(textBaseExpToLVL, baseExpToLVL);
        ChangingStatText(textJobExpToLVL, jobExpToLVL);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OtherStatsInfo : MonoBehaviour {
    #region GameObjectsAndVariables
    public float atkInfo;
    public GameObject textatkInfo;
    public float atkWeaponUpgradeDamageInfo;
    public GameObject textatkWeaponUpgradeDamageInfo;
    public float mAtkInfo;
    public GameObject textmAtkInfo;
    public float hitInfo;
    public GameObject texthitInfo;
    public float critInfo;
    public GameObject textcritInfo;
    public float defInfo;
    public GameObject textdefInfo;
    public float softDefInfo;
    public GameObject textsoftDefInfo;
    public float mDefInfo;
    public GameObject textmDefInfo;
    public float softMDefInfo;
    public GameObject textsoftMDefInfo;
    public float fleeInfo;
    public GameObject textfleeInfo;
    public float aspdInfo;
    public GameObject textaspdInfo;
    public float statPointsInfo;
    public GameObject textstatPointsInfo;
    #endregion
    public void ChangingStatText(GameObject Object, float Number) {
        Object.GetComponent<TextMeshProUGUI>().text = Number.ToString();
    }
    public void GiveMeYourStats(float atk, float atkW, float mAtk, float hit, float crit, float def, float softDef, float mDef, float softMDef, float flee, float aspd, float statPoints) {
        atkInfo = atk;
        atkWeaponUpgradeDamageInfo = atkW;
        mAtkInfo = mAtk;
        hitInfo = hit;
        critInfo = crit;
        defInfo = def;
        softDefInfo = softDef;
        mDefInfo = mDef;
        softMDefInfo = softMDef;
        fleeInfo = flee;
        aspdInfo = aspd;
        statPointsInfo = statPoints;
        ChangingStatText(textatkInfo, atkInfo);
        ChangingStatText(textatkWeaponUpgradeDamageInfo, atkWeaponUpgradeDamageInfo);
        ChangingStatText(textmAtkInfo, mAtkInfo);
        ChangingStatText(texthitInfo, hitInfo);
        ChangingStatText(textcritInfo, critInfo);
        ChangingStatText(textdefInfo, defInfo);
        ChangingStatText(textsoftDefInfo, softDefInfo);
        ChangingStatText(textmDefInfo, mDefInfo);
        ChangingStatText(textsoftMDefInfo, softMDefInfo);
        ChangingStatText(textfleeInfo, fleeInfo);
        ChangingStatText(textaspdInfo, aspdInfo);
        ChangingStatText(textstatPointsInfo, statPointsInfo);
    }
}

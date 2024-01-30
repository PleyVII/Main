using UnityEngine;
using TMPro;

public class RealStatChange : MonoBehaviour {
    public float baseStat;
    public float pointsToAdd;
    public GameObject baseTextStat;
    public GameObject baseTextAdd;
    public float statPointsUsed;
    public CharacterUpdates CurrentTarget;
    public AllRealStats allRealStats;
    void Start() {
        allRealStats = FindObjectOfType<AllRealStats>();
        baseStat = 0;
        pointsToAdd = 1;
        statPointsUsed = 0;
        BaseStatTextChange();
    }

    public void StatButtonClick() {
        allRealStats = FindObjectOfType<AllRealStats>();
        if (pointsToAdd <= CurrentTarget.allInfo.Stats.SPoints) {
            baseStat++;
            statPointsUsed += pointsToAdd;
            // CurrentTarget.PointsUsed(pointsToAdd);
            pointsToAddCalculation();
        }
        BaseStatTextChange();
        allRealStats.SendingStats(CurrentTarget);
    }
    public void BaseStatTextChange() {
        baseTextStat.GetComponent<TextMeshProUGUI>().text = baseStat.ToString();
        baseTextAdd.GetComponent<TextMeshProUGUI>().text = pointsToAdd.ToString();
    }
    public void pointsToAddCalculation() {
        if (baseStat < 90) {
            pointsToAdd = Mathf.Floor(baseStat + 5 / 5);
        } else {
            pointsToAdd = Mathf.Floor((baseStat - 54) / 2);
        }
    }
    // public void TextChange(GameObject Object, float number)
    // {
    //     Object.GetComponent<TextMeshProUGUI>().text = number.ToString();
    // }

}
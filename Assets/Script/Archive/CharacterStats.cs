// using UnityEngine;
// using System.Collections;

// public class CharacterStats : MonoBehaviour
// {
//     #region Stats
//     public Stat str;
//     public Stat agi;
//     public Stat vit;
//     public Stat spec;
//     public Stat crit;
//     public Stat _int;
//     public Stat dex;
//     public Stat atk;
//     public Stat WeaponAtk;
//     public Stat matk;
//     public Stat hit;
//     public Stat critc;
//     public Stat def;
//     public Stat sdef;
//     public Stat mdef;
//     public Stat smdef;
//     public Stat flee;
//     public Stat aspd;
//     public Stat mana;
//     public Stat mpregen;
//     public Stat hpregen;
//     public Stat spoints;
//     public Stat WeaponRange;
//     #endregion
//     #region newStats
//     public float nonbowatk;
//     public float bowatk;
//     public float newatk;
//     public float newmatk;
//     public float newhit;
//     public float newcritc;
//     public float newdef;
//     public float newsdef;
//     public float newmdef;
//     public float newsmdef;
//     public float newflee;
//     public float newaspd;
//     public float maxMP;
//     public float newspoints;
//     #endregion
//     public float allPointsUsed;
//     #region OtherScriptsComunication
//     public CharacterClass _class;
//     public OtherStatsInfo sendingBitch;
//     public ProfileInformation sendingBitch2;

//     #endregion
//     bool onRegenHealth = false;
//     bool onRegenMana = false;
//     public float basicMaxHealth = 10000;
//     public float maxHealth;
//     public float currentHealth;
//     public float currentMana;
//     public float hPReg;
//     public float mPReg;
//     public string playerName = "Pley";
//     public float characterLVL = 80;
//     public float jobLVL = 70;
//     public float statvarible = 0;
//     public float baseExpToLVLUP = 100;
//     public float baseExpCurrent = 1;
//     public float jobExpToLVLUP = 100;
//     public float jobExpCurrent = 1;
//     EquipmentManager additionalOnEquip;
//     public bool isFocused = false;
//     public static event System.Action OnGettingAttacked;
//     void Awake() 
//     {
//         _class = CharacterClass.novice;
//         ReCalculateStats();
//     }
//     void Update()
//     {
//         if (sendingBitch == null) sendingBitch = GameObject.FindObjectOfType<OtherStatsInfo>();
//         if (sendingBitch2 == null) sendingBitch2 = GameObject.FindObjectOfType<ProfileInformation>();
//         if (Input.GetKeyDown(KeyCode.T))
//         {
//             TakeDamageAndRemoveMana(1000, 50);
//         }
//         if(currentHealth < maxHealth && onRegenHealth == false )
//         {
//             onRegenHealth = true;
//             StartCoroutine("RegenHealth");
//         }
//         if(currentMana < maxMP && onRegenMana == false )
//         {
//             onRegenMana = true;
//             StartCoroutine("RegenMana");
//         }
//         if (isFocused) sendingBitch2.UpdateHealthUpdateManaUIUpdate(currentHealth, currentMana);
//     }
//     IEnumerator RegenHealth()
//     {
//         yield return new WaitForSeconds(5);
//         onRegenHealth = false;
//         currentHealth += hPReg;
//         if (currentHealth >= maxHealth) currentHealth = maxHealth;
//     }
//     IEnumerator RegenMana()
//     {
//         yield return new WaitForSeconds(5);
//         onRegenMana = false;
//         currentMana += mPReg;
//         if (currentMana >=maxMP) currentMana = maxMP;
//     }
//     public void TakeDamageAndRemoveMana(float damagee, float manadamage)
//     {
//         float damage = 0;
//         damage = ((damagee * (100- newdef) )/100) - newsdef;
//         currentHealth -= damage;
//         damage = Mathf.Clamp(damage, 0, int.MaxValue);
//         Debug.Log(transform.name + " takes " + damage + " damage ");
//         currentMana -= manadamage;
//         if (currentMana < 0) currentMana = 0;
//         if (currentHealth <= 0)
//         {
//             Die();
//         }
//     }
//     public void GetAttackedAutoAttack(float physical)
//     {
//         float physicalDamage = Mathf.Clamp((((physical * (100 - newdef) )/100) - newsdef),1 , float.MaxValue);
//         currentHealth -= physicalDamage;
//         Debug.Log("Hit for " + physicalDamage + " damage" + ", " + currentHealth + "hp left");
//         OnGettingAttacked?.Invoke();
//     }
//     // public void SendingBaseStat(float Strbase, float Agibase, float Vitbase, float Critbase, float Specbase, float Intbase, float Dexbase)
//     // {
//     //     str.baseValue = Strbase;
//     //     agi.baseValue = Agibase;
//     //     vit.baseValue = Vitbase;
//     //     crit.baseValue = Critbase;
//     //     spec.baseValue = Specbase;
//     //     _int.baseValue = Intbase;
//     //     dex.baseValue = Dexbase;
//     //     ReCalculateStats();
//     //     sendingBitch = GameObject.FindObjectOfType<OtherStatsInfo>();
//     //     sendingBitch2 = GameObject.FindObjectOfType<ProfileInformation>();
//     //     SendingStats();
//     // }
//     public void PointsUsed(float usedPoints)
//     {
//         allPointsUsed += usedPoints;
//     }
//     public void istatgain(float LVL)
//     {
//         statvarible = 0;
//         for (var i = 0; i < LVL; i++)
//         {
//             statvarible += (5+(Mathf.Floor(i/10)))*4;
//         }
//     }
//     public virtual void Die()
//     {
//         Debug.Log("Die Bitch");
//     }
//     #region Don't look
//     public void ReCalculateStats()
//     {
//         mana.baseValue = 300;
//         maxMP = mana.baseValue + (2*mana.GetValue()/100 * _int.GetValue());
//         maxHealth = basicMaxHealth + (4*basicMaxHealth/100 * vit.GetValue());
//         hPReg = characterLVL + (maxHealth/100) + hpregen.GetValue() + (6*(characterLVL + (maxHealth/100) + hpregen.GetValue())/100)*(vit.GetValue());
//         mPReg = 2 + (maxMP/100) + mpregen.GetValue() + (6*(2 + (maxMP/100) + mpregen.GetValue())/100)*(_int.GetValue());
//         nonbowatk = 2*str.GetValue() + ((Mathf.Floor(str.GetValue()/10))*30);
//         bowatk = 2*dex.GetValue() + ((Mathf.Floor(dex.GetValue()/10))*30);
//         newatk = 50 + WeaponAtk.GetValue() + nonbowatk; //if bow equiped then bowatk + atk.GetValue() else nonbowatk + atk.GetValue()
//         newmatk = 10 * _int.GetValue() + matk.GetValue();
//         newhit = characterLVL + 4*dex.GetValue() + hit.GetValue();
//         newcritc = crit.GetValue() + critc.GetValue();
//         newdef = def.GetValue();
//         if (newdef<=50)
//             newdef = (def.GetValue())*2;
//             if (newdef>50)
//                 newdef = (def.GetValue())+25;
//                 if (newdef>75)
//                 newdef = ((def.GetValue())+25)-((def.GetValue() - 50)/2);
//         newsdef = sdef.GetValue() + vit.GetValue()*3;
//         newmdef = 0 + mdef.GetValue();
//         newsmdef = mdef.GetValue() + _int.GetValue()*3;
//         newflee = characterLVL + 4*agi.GetValue() + flee.GetValue();
//         newaspd = 50 + agi.GetValue()*2 + aspd.GetValue();
//         istatgain(characterLVL);
//         newspoints = statvarible + spoints.GetValue() - allPointsUsed;
//     }
//     public void SendingStats()
//     {
//         sendingBitch.GiveMeYourStats(newatk, WeaponAtk.GetValue(), newmatk, newhit, newcritc, newdef, newsdef, newmdef, newsmdef, newflee, newaspd, newspoints);
//         sendingBitch2.GiveMeYourStats2(playerName, _class.ToString(), maxHealth, maxMP, currentHealth, currentMana);
//         sendingBitch2.LvLUpUIUpdate(maxHealth, maxMP, currentHealth, currentMana, characterLVL, jobLVL, baseExpToLVLUP, baseExpCurrent, jobExpToLVLUP, jobExpCurrent);
//         sendingBitch2.KillMonsterExpUIUpdate(characterLVL, baseExpCurrent, baseExpToLVLUP, jobLVL, jobExpCurrent, jobExpToLVLUP);
//     }
//     #endregion
//     public enum CharacterClass
//     {
//         novice, swordsman, mage, cartboy, acolyte, thief, archer, Kingsguard, RoyalTemplar, Guardian, Archimage, Scholar, Enchanter, Creator, Blacksmith, Zoologist,
//         Aun, Bishop, Occultist, Assassin, Scoundrel, Reaper, Sniper, Bard, Hunter
//     }
// }

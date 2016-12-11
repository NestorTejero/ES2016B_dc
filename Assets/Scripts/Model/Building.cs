﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, CanUpgrade, CanReceiveDamage, HUDSubject
{

    enum UBTextureHPIndex { Full, Medium, Low}
    enum HPThreshold  { Full = 100, Medium = 50, Low = 25} 
    public float baseHealth;

    public HealthComponent health;
    public float repairCost;
    public float repairQuantity;
    public float upgradeCost;
    public float upgradeFactor;

    //level values min = 1, max = 3
    private int minLevel;
    private int currentLevel;
    private int maxLevel;


    private GameObject textureModel;
    private MeshRenderer skin;
    //textures to apply on each level
    public List<Texture> textures;
    public Texture[][] tx;
    private GameObject smokeEffect;
    private ParticleSystem smoke;
    // Receive damage by weapon
    public void ReceiveDamage(float damage)
    {
        try
        {
            health.LoseHealth(damage);
            NotifyHUD();
            Debug.Log("BUILDING RECEIVED DAMAGE: " + damage + " - CURRENT_HEALTH: " + health.GetCurrentHealth());
            ApplyMainTexture();
            ApplySmokeEffect();
        }
        catch (Exception)
        {
            NotifyHUD();
            GameController.instance.notifyDeath(this);
        }
    }

    public GameObject getGameObject()
    {
        return gameObject;
    }

    public bool IsUpgradeable(int numCoins)
    {
        
        return (upgradeCost <= numCoins) & (currentLevel < maxLevel);

    }

    // To upgrade when there are enough coins
    public void Upgrade()
    {
        GameObject.FindGameObjectWithTag("Human").GetComponent<Player>().SpendCoins((int)upgradeCost);
        health.Upgrade(upgradeFactor);
        currentLevel++;
        NotifyHUD();
        ApplyMainTexture();
        ApplySmokeEffect();
        Debug.Log("BUILDING UPGRADED, TOTAL HP: " + health.GetTotalHealth());
    }

	//To check if have enough money to buy the tower
	public bool canBuild(){
		int currentMoney = GameObject.FindGameObjectWithTag ("Human").GetComponent<Player> ().GetNumCoins ();
		int towerCost = GameObject.Find ("Towers").GetComponentInChildren<Tower> ().buildCost;
		if (currentMoney >= towerCost) {
			Debug.Log("You have money to build");
			return true;
		} else {
			return false;
		}
	}

	//To spend the money in order to pay for the tower
	public void buyTower(){
		int towerCost = GameObject.Find ("Towers").GetComponentInChildren<Tower> ().buildCost;
		GameObject.FindGameObjectWithTag ("Human").GetComponent<Player> ().SpendCoins (towerCost);
	}

    public void NotifyHUD()
    {
        var updateInfo = new HUDInfo
        {
            CurrentHealth = health.GetCurrentHealth(),
            TotalHealth = health.GetTotalHealth(),
			VisibleUpgradeButton = IsUpgradeable(GameObject.FindGameObjectWithTag("Human").GetComponent<Player>().GetNumCoins()),
			VisibleRepairButton = IsRepairable(GameObject.FindGameObjectWithTag("Human").GetComponent<Player>().GetNumCoins())
        };

        APIHUD.instance.notifyChange(this, updateInfo);
    }

    // Use this for initialization
    private void Start()
    {
        minLevel = 1;
        maxLevel = 3;
        currentLevel = minLevel;

        health = new HealthComponent(baseHealth);
        Debug.Log("BUILDING CREATED with HP: " + baseHealth);


        textureModel = this.transform.FindChild("Model").gameObject;
        skin = textureModel.GetComponent<MeshRenderer>();
        skin.material.mainTexture = textures[(int)UBTextureHPIndex.Full];

        //particle data
        smokeEffect = transform.FindChild("WhiteSmoke").gameObject;
        smoke = transform.GetComponent<ParticleSystem>();
        smokeEffect.SetActive(false);
    }

    public bool IsRepairable(int numCoins)
    {
        return (repairCost <= numCoins) && !health.IsHealthFull();
    }

    // Repair the building
    public void Repair()
    {
        GameObject.FindGameObjectWithTag("Human").GetComponent<Player>().SpendCoins((int)repairCost);
        health.AddHealth(repairQuantity);
        NotifyHUD();
        ApplyMainTexture();
        ApplySmokeEffect();
        Debug.Log("BUILDING REPAIRED, CURRENT HP: " + health.GetCurrentHealth());
    }

    /*
     * Applies texture to ub depending on its Health and Level
     * 
     */
    public void ApplyMainTexture()
    {
        var hp = health.GetCurrentHealthPercentage();
        var text = skin.material.mainTexture; // to avoid null values

        if (hp > (float)HPThreshold.Medium)
        {
            text = textures[3 * (currentLevel - 1) + (int)UBTextureHPIndex.Full];
                    
        }
        else if(hp > (float)HPThreshold.Low && hp < (float)HPThreshold.Medium)
        {
            text = textures[3 * (currentLevel - 1) + (int)UBTextureHPIndex.Medium];
        }
        else
        {
            text = textures[3 * (currentLevel - 1) + (int)UBTextureHPIndex.Low];
        }
                    
        skin.material.mainTexture = text;

    }
    private void ApplySmokeEffect()
    {
        if(health.GetCurrentHealthPercentage() > (float)HPThreshold.Medium)
        {
            smokeEffect.SetActive(false);

        }
        else if (health.GetCurrentHealthPercentage() > (float)HPThreshold.Low && health.GetCurrentHealthPercentage() < (float)HPThreshold.Medium)
        {
            smokeEffect.SetActive(true);
            smoke.startSize = 20.0f;
        }else
        {
            smokeEffect.SetActive(true);
            smoke.startSize = 40.0f;
        }
    }
}
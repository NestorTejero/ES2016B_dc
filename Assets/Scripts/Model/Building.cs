﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Building : MonoBehaviour, CanUpgrade, CanReceiveDamage
{
    private float currentLevel;
    public float baseHealth;
    private float totalHealth;
    private float currentHealth;

	public float upgradeFactor;
    public float upgradeCost;
	public float repairQuantity;
    public float repairCost;


	// Use this for initialization
	void Start ()
	{
	    this.currentLevel = 0;
		this.currentHealth = this.baseHealth;
	    this.totalHealth = this.baseHealth;


        Debug.Log ("BUILDING CREATED with HP: " + this.baseHealth);
	}

	// Update is called once per frame
	void Update ()
	{
    }

	// To upgrade when there are enough coins
	public void Upgrade ()
	{
		this.totalHealth *= this.upgradeFactor;
		this.currentHealth *= this.upgradeFactor;
	    this.currentLevel++;
		Debug.Log ("BUILDING UPGRADED, now it has HP: " + this.totalHealth);
	}

	// Repair the building
	public void Repair ()
	{
		this.currentHealth += this.repairQuantity;
		if (this.currentHealth > this.totalHealth) {
			this.currentHealth = this.totalHealth;
		}
		Debug.Log ("BUILDING REPAIRED, HP: " + this.currentHealth);
	}

	// Receive damage by weapon
	public void ReceiveDamage (float damage)
	{
		this.currentHealth -= damage;
		Debug.Log ("Building's currentHealth: " + this.currentHealth);
		//Debug.Log ("BUILDING DAMAGED by HP: " + proj.getDamage());

		if (this.currentHealth <= 0.0) {
			GameController.instance.notifyDeath (this);
		}
	}

    public float getMissingHealth ()
    {
        return this.totalHealth - this.currentHealth;
    }

	public GameObject getGameObject(){
		return this.gameObject;
	}
}

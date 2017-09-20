﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour, IDamageable {

    [SerializeField] int enemyLayer = 9;
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float damagePerHit = 2f;
    [SerializeField] float minTimeBetweenHits = 1f;
    [SerializeField] float maxMeleeRange = 2f;
    [SerializeField] Weapon weaponInUse;    

    float currentHealthPoints;
    CameraRaycaster cameraRaycaster;
    float lastHitTime = 0f;

    public float healthAsPercentage
    {
        get { return currentHealthPoints / maxHealthPoints; }
    }

    void Start()
    {
        RegisterForMouseClick();
        currentHealthPoints = maxHealthPoints;
        PutWeaponInHand();
    }

    private void PutWeaponInHand()
    {
        var weaponPrefab = weaponInUse.GetWeaponPrefab();
        GameObject dominantHand = RequestDominantHand();
        var weapon = Instantiate(weaponPrefab, dominantHand.transform);
        weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
        weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
    }

    private GameObject RequestDominantHand()
    {
        var dominantHands = GetComponentsInChildren<DominantHand>();
        int numberOfDominantHands = dominantHands.Length;
        Assert.IsFalse(numberOfDominantHands <= 0, "No dominant hand found on player. Please add one.");
        Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominant hand scripts on player. Please remove one.");
        return dominantHands[0].gameObject;
    }

    private void RegisterForMouseClick()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }

    // TODO Refactor to reduce number of lines
    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if(layerHit == enemyLayer)
        {
            var enemy = raycastHit.collider.gameObject;            
            // Check that enemy is in range for melee
            if((enemy.transform.position - transform.position).magnitude > maxMeleeRange)
            {
                return;
            }
            var enemyComponent = enemy.GetComponent<Enemy>();
            if(Time.time - lastHitTime > minTimeBetweenHits)
            {
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }            
        }
    }

    // Damage interface
    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
    }

}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] Image energyOrb = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 10f;
        
        public float currentEnergyPoints;
 
        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
            //UpdateEnergyOrb();
        }

        void Update()
        {
            AddEnergyPoints();            
        }

        private void AddEnergyPoints()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
                currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
                UpdateEnergyOrb();
            }
        }

        public bool IsEnergyAvailable(float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyOrb();
        }

        private void UpdateEnergyOrb()
        {
            energyOrb.fillAmount = EnergyAsPercent();
        }

        float EnergyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}

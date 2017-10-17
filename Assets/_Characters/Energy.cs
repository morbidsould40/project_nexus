using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 10f;


        public float currentEnergyPoints;
 

        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;            
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
                UpdateEnergyBar();
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
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            float xValue = -(EnergyAsPercent() / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        float EnergyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}

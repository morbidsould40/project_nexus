using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar;
        [SerializeField] AudioClip outOfEnergy;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 10f;
        
        public float currentEnergyPoints;
        AudioSource audioSource;

        float energyAsPercent { get { return currentEnergyPoints / maxEnergyPoints; } }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            currentEnergyPoints = maxEnergyPoints;
            AttachInitialAbilities();            
        }

        void Update()
        {
            AddEnergyPoints();            
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if (energyCost <= currentEnergyPoints)
            {
                ConsumeEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        public float SpecialAbilityCoolDownTimer(int abilityIndex)
        {
            return abilities[abilityIndex].GetCoolDown();
        }

        void AddEnergyPoints()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
                currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
                UpdateEnergyBar();
            }
        }
        
        void UpdateEnergyBar()
        {
            if (energyBar)
            {
                energyBar.fillAmount = energyAsPercent;
            }            
        }

        void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }
    }
}

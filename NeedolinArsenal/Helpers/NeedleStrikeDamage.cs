using DanielSteginkUtils.Helpers;
using GlobalEnums;
using UnityEngine;

namespace NeedolinArsenal.Helpers
{
    public class NeedleStrikeDamage : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            // Verify that the collider exists and that its an enemy
            if (other == null ||
                other.gameObject.layer != (int)PhysLayers.ENEMIES)
            {
                return;
            }

            // The attack uses sharpened silk, making it a spell attack
            HealthManager enemy = other.gameObject.GetComponent<HealthManager>();
            DamageEnemy.DealDamage(enemy, PlayerData.instance.nailDamage / 4, AttackTypes.Spell, gameObject);
        }
    }
}
using UnityEngine;

namespace VampireDrama
{
    public class Construct
    {
        public PossibleConstruct Template;
        private float SpawnerCooldownTimer = 0;

        public Construct(PossibleConstruct construct)
        {
            Template = new PossibleConstruct();
            Template.LoadFrom(construct);
        }

        public bool TimeToSpawn()
        {
            SpawnerCooldownTimer += Time.deltaTime;
            return (SpawnerCooldownTimer >= Template.SpawnerCooldown);
        }

        public void ResetSpawnTimer()
        {
            SpawnerCooldownTimer = 0;
        }

        public Construct Clone()
        {
            var clone = new Construct(Template);
            return clone;
        }
    }
}

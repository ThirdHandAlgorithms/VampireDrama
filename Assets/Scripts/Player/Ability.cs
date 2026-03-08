namespace VampireDrama
{
    [System.Serializable]
    public class Ability
    {
        public string Name;
        public string Description;
        public float CooldownDuration;
        public float CooldownRemaining;
        public float ActiveDuration;
        public float ActiveRemaining;
        public int BloodCost;

        public bool IsOnCooldown { get { return CooldownRemaining > 0f; } }
        public bool IsActive { get { return ActiveRemaining > 0f; } }

        public Ability(string name, string description, float cooldownDuration, float activeDuration, int bloodCost)
        {
            Name = name;
            Description = description;
            CooldownDuration = cooldownDuration;
            ActiveDuration = activeDuration;
            BloodCost = bloodCost;
            CooldownRemaining = 0f;
            ActiveRemaining = 0f;
        }

        public bool TryActivate(VampirePlayer player)
        {
            if (IsOnCooldown || IsActive) return false;
            if (player.Stats.Bloodfill < BloodCost) return false;

            if (Execute(player))
            {
                player.Stats.Bloodfill -= BloodCost;
                ActiveRemaining = ActiveDuration;

                if (ActiveDuration <= 0f)
                {
                    CooldownRemaining = CooldownDuration;
                }

                return true;
            }

            return false;
        }

        public virtual bool Execute(VampirePlayer player)
        {
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (ActiveRemaining > 0f)
            {
                ActiveRemaining = System.Math.Max(0f, ActiveRemaining - deltaTime);

                if (ActiveRemaining <= 0f)
                {
                    CooldownRemaining = CooldownDuration;
                }
            }
            else if (CooldownRemaining > 0f)
            {
                CooldownRemaining = System.Math.Max(0f, CooldownRemaining - deltaTime);
            }
        }
    }
}

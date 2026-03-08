namespace VampireDrama
{
    using System.Collections.Generic;

    [System.Serializable]
    public class AbilitySet
    {
        public static AbilitySet Current;

        public List<Ability> Abilities;
        public int SelectedIndex;

        public AbilitySet()
        {
            Abilities = new List<Ability>();
            SelectedIndex = 0;
        }

        public Ability Selected
        {
            get
            {
                if (Abilities.Count == 0) return null;
                return Abilities[SelectedIndex];
            }
        }

        public void CycleNext()
        {
            if (Abilities.Count == 0) return;
            SelectedIndex = (SelectedIndex + 1) % Abilities.Count;
        }

        public void Unlock(Ability ability)
        {
            Abilities.Add(ability);
        }

        public bool ActivateSelected(VampirePlayer player)
        {
            var ability = Selected;
            if (ability == null) return false;
            return ability.TryActivate(player);
        }

        public void Tick(float deltaTime)
        {
            foreach (var ability in Abilities)
            {
                ability.Tick(deltaTime);
            }
        }
    }
}

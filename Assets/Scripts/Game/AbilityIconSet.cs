namespace VampireDrama
{
    using UnityEngine;
    using System.Collections.Generic;

    public class AbilityIconSet : MonoBehaviour
    {
        [System.Serializable]
        public struct AbilityIconEntry
        {
            public string AbilityName;
            public Sprite Icon;
        }

        public AbilityIconEntry[] Icons;

        public Sprite GetIcon(string abilityName)
        {
            if (Icons == null) return null;

            foreach (var entry in Icons)
            {
                if (entry.AbilityName == abilityName)
                {
                    return entry.Icon;
                }
            }

            return null;
        }
    }
}

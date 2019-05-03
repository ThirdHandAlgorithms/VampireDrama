using System.Collections.Generic;
using UnityEngine;

namespace VampireDrama
{
    public enum PossibleEffects {
        Holy = 1,
        SelfHealing = 2,
        Sun = 3
    }

    public class PossibleEffectsUtils
    {
        public static List<PossibleEffects> ListAll()
        {
            var list = new List<PossibleEffects>();
            list.Add(PossibleEffects.Holy);
            list.Add(PossibleEffects.SelfHealing);
            list.Add(PossibleEffects.Sun);

            return list;
        }

        public static Component GetComponentFor(PossibleEffects effect, GameObject gameObject)
        {
            switch (effect)
            {
                case PossibleEffects.Holy:
                    return gameObject.GetComponent<HolyAura>();
                case PossibleEffects.SelfHealing:
                    return gameObject.GetComponent<SelfHealing>();
                default:
                    return null;
            }
        }

        public static Component AddComponentFor(PossibleEffects effect, GameObject gameObject, int strength)
        {
            switch (effect)
            {
                case PossibleEffects.Holy:
                    var compHoly = gameObject.AddComponent<HolyAura>();
                    compHoly.Strength = strength;
                    return compHoly;
                case PossibleEffects.SelfHealing:
                    var compSelfHeal = gameObject.AddComponent<SelfHealing>();
                    compSelfHeal.Strength = strength;
                    return compSelfHeal;
                default:
                    return null;
            }
        }
    }
}

namespace VampireDrama
{
    using UnityEngine;

    public abstract class AuraEffect
    {
        public abstract void Affect(Human obj);
        public abstract void Affect(VampirePlayer obj);
    }
}

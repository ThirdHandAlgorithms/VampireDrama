namespace VampireDrama
{
    using UnityEngine;

    public abstract class AuraEffect : MonoBehaviour
    {
        public abstract void Affect(Human obj);
        public abstract void Affect(VampirePlayer obj);
    }
}

namespace VampireDrama
{
	using UnityEngine;

	public class OverlayManager : MonoBehaviour
	{
        private Animator buffAnim = null;
        private Animator healthbarAnim = null;
        private Human human = null;
        private SpriteRenderer buffRenderer = null;

        public void Start()
		{
            Animator[] overlayAnimators = GetComponentsInChildren<Animator>();
            foreach (var anim in overlayAnimators)
            {
                if (anim.gameObject.name == "Buff") buffAnim = anim;
                else if (anim.gameObject.name == "Healthbar") healthbarAnim = anim;
            }

            buffRenderer = buffAnim.gameObject.GetComponent<SpriteRenderer>();

            human = (gameObject.transform.parent as Transform).GetComponent<Human>();
        }

        public void Update()
        {
            if (human != null)
            {
                updateHealthbar();
                updateBuff();
            }
        }

        private void updateHealthbar()
        {
            var percentage = human.LitresOfBlood / human.MaxBlood * 100f;
            if (percentage > 90) healthbarAnim.Play("Hp100");
            else if (percentage > 80) healthbarAnim.Play("Hp90");
            else if (percentage > 70) healthbarAnim.Play("Hp80");
            else if (percentage > 60) healthbarAnim.Play("Hp70");
            else if (percentage > 50) healthbarAnim.Play("Hp60");
            else if (percentage > 40) healthbarAnim.Play("Hp50");
            else if (percentage > 30) healthbarAnim.Play("Hp40");
            else if (percentage > 20) healthbarAnim.Play("Hp30");
            else if (percentage > 10) healthbarAnim.Play("Hp20");
            else healthbarAnim.Play("Hp10");
        }

        private void updateBuff()
        {
            if (human.KnowsWhatsUp())
            {
                buffRenderer.enabled = true;
                buffAnim.Play("Alert");
            }
            else if (human.IsVerySuspicious())
            {
                buffRenderer.enabled = true;
                buffAnim.Play("Suspicious");
            }
            else if (human.IsVeryDrunk())
            {
                buffRenderer.enabled = true;
                buffAnim.Play("Drunk");
            }
            else
            {
                buffRenderer.enabled = false;
            }
        }
    }
}
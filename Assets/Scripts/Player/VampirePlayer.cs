namespace VampireDrama
{
    using System.Collections.Generic;
    using UnityEngine;

    public class VampirePlayer : MovingAnimation
    {
        public string Name;
        private float lastInput;
        private float maxDefense = 100;
        private bool nextMoveIsJump;

        public PlayerStats Stats;

        protected override void Start()
        {
            base.Start();

            Stats = GameGlobals.GetInstance().PlayerStats;
            nextMoveIsJump = false;

            lastInput = Time.time;

            Stats.Bloodfill = 10;
        }

        protected void UpdateItemEffects()
        {
            var globals = GameGlobals.GetInstance();
            foreach (var item in globals.PlayerStats.Items)
            {
                var toRemove = PossibleEffectsUtils.ListAll();

                foreach (var effectEnum in item.Stats.Effects)
                {
                    toRemove.Remove(effectEnum);

                    var comp = PossibleEffectsUtils.GetComponentFor(effectEnum, gameObject);
                    if (comp == null)
                    {
                        PossibleEffectsUtils.AddComponentFor(effectEnum, gameObject, item.Stats.ItemLevel);
                    }
                }

                foreach (var effectEnum in toRemove)
                {
                    var comp = PossibleEffectsUtils.GetComponentFor(effectEnum, gameObject);
                    if (comp != null)
                    {
                        Destroy(comp);
                    }
                }
            }
        }

        public override void Update()
		{
            UpdateItemEffects();
            base.Update();
            if (isMoving) return;

            var timeNow = Time.time;
            if (timeNow - lastInput < 0.2f) return;

            if (Input.GetButtonDown("Fire1"))
            {
                nextMoveIsJump = true;
            }

            if (Input.GetButtonDown("Fire2"))
            {
                RaycastHit2D currentPoshit;
                if (!IsSomethingAtTheEnd(transform.position, out currentPoshit))
                {
                    var inv = GetInventory();
                    var item = inv.FirstItem();
                    if (item != null)
                    {
                        var level = GameManager.GetCurrentLevel();
                        if (level.AddItem(item.Stats, transform.position))
                        {
                            inv.RemoveItem(item);
                        }
                    }
                }
            }

            int hor = (int)Input.GetAxisRaw("Horizontal");
            int ver = (int)Input.GetAxisRaw("Vertical");

            RaycastHit2D hit = new RaycastHit2D();
            bool hitSomething = false;

            if (nextMoveIsJump && ((hor != 0) || (ver != 0)))
            {
                hor *= 2;
                ver *= 2;
                nextMoveIsJump = false;
            }

            if ((ver == 0) && (hor != 0))
            {
                hitSomething = !Move(hor, ver, out hit);
                lastInput = timeNow;
            }
            else if ((hor == 0) && (ver != 0))
            {
                hitSomething = !Move(hor, ver, out hit);
                lastInput = timeNow;
            }

            if (hitSomething)
            {
                Transform objectHit = hit.transform;
                GameObject gameObjHit = objectHit.gameObject;

                Human sheep = gameObjHit.GetComponent<Human>();
                if (sheep != null)
                {
                    if (sheep.isMoving) return;

                    Fight(sheep, gameObjHit, hor, ver);
                }

                Item item = gameObjHit.GetComponent<Item>();
                if (item != null)
                {
                    var inventory = GetInventory();
                    if (inventory.AddItem(item.CreateInventoryItem()))
                    {
                        GameManager.GetCurrentLevel().PickUpItem(item);
                    }

                    Move(hor, ver, out hit);
                }
            }
        }

        public Inventory GetInventory()
        {
            return Camera.allCameras[0].GetComponentInChildren<Inventory>() as Inventory;
        }

        public float GetTotalStrength()
        {
            float strength = 1f;

            foreach (var item in Stats.Items)
            {
                strength += item.Stats.Strength;
            }

            return strength + Stats.Bloodfill;
        }

        public float GetTotalDefense()
        {
            float defense = 0;

            foreach (var item in Stats.Items)
            {
                defense += item.Stats.Defense;
            }

            return defense;
        }

        public override float GetTotalMovementSpeed()
        {
            float moveTime = baseMoveSpeed;

            foreach (var item in Stats.Items)
            {
                moveTime += item.Stats.TravelSpeed;
            }

            return Mathf.Min(99, moveTime + Stats.Bloodfill);
        }

        public bool Fight(Human target, GameObject obj, int hor, int ver)
        {
            var level = GameManager.GetCurrentLevel();

            if (target.GetResistance() > 0.5)
            {
                var originalPosition = transform.position;

                AttackMove(hor, ver);
                onAttackHalfway = () =>
                {
                    target.LoseBlood(GetTotalStrength() * Random.value, originalPosition);
                };
            }
            else
            {
                FullAttackMove(hor, ver);
                onAttackHalfway = () =>
                {
                    Stats.Bloodfill += (int)System.Math.Floor(target.LitresOfBlood);
                    level.Kill(target, obj);
                    Stats.Experience += 1;
                };
            }

            return true;
        }

        public void Burn(int strength)
        {
            Stats.Bloodfill = System.Math.Max(0, Stats.Bloodfill - strength);
            if (Stats.Bloodfill == 0)
            {
                Debug.Log("You just died, queue the high-score screen and start over again");

                GameManager.instance.GameOver();
            }
        }

        public void ReceivePunch(int strength)
        {
            var defense = GetTotalDefense();
            if (Random.value < defense / maxDefense)
            {
                Debug.Log("Attack dodged");
            }
            else
            {
                Stats.Bloodfill = System.Math.Max(0, Stats.Bloodfill - strength);
                if (Stats.Bloodfill == 0)
                {
                    Debug.Log("You just died, queue the high-score screen and start over again");

                    GameManager.instance.GameOver();
                }
            }
        }

        public void OnTriggerEnter2D(Collider2D item)
        {
            //Debug.Log(this.name + " triggered by " + item.gameObject.name);
        }
    }
}

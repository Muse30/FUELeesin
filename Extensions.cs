using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;
using static FUELeesin.Menus;
using Mario_s_Lib;
using static FUELeesin.SpellsManager;
using System.Collections.Generic;
using EloBuddy.SDK.Enumerations;

namespace FUELeesin
{
    public static class Extensions
    {
        /// <summary>
        /// Get total damge using the custom values provided by you in the spellmanager
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// 
        public static Vector3 InsecClickPos;
        public static bool ClicksecEnabled;
        public static Vector2 InsecLinePos;
        public static Vector2 JumpPos;
        public static bool isNullInsecPos = true;
        public static Vector3 insecPos;
        public static float doubleClickReset;
        public static bool castQAgain;
        public static int clickCount;
        public static SpellSlot igniteSlot;
        public static bool lastClickBool;
        public static Vector3 lastClickPos;
        public static float lastPlaced;
        public static InsecComboStepSelect insecComboStep;
        public static Vector3 lastWardPos;
        public static Vector3 mouse = Game.CursorPos;
        public static float passiveTimer;
        public static bool q2Done;
        public static float q2Timer;
        public static bool reCheckWard = true;
        public static float resetTime;
        public static readonly bool castWardAgain = true;
        public static bool waitingForQ2;
        public static bool wardJumped;
        public static float wcasttime;
        public static int Wcasttime;
        public static float LastWard;
        public const int FlashRange = 425;
        public const int WardRange = 760;
        public static bool CheckQ = true;

        public static readonly string[] SpellNames =
    {
                "blindmonkqone", "blindmonkwone", "blindmonkeone",
                "blindmonkwtwo", "blindmonkqtwo", "blindmonketwo",
                "blindmonkrkick"
            };


        public static Dictionary<string, string> Spellss = new Dictionary<string, string>
        {
            {"Q1", "BlindMonkQOne"},
            {"W1", "BlindMonkWOne"},
            {"E1", "BlindMonkEOne"},
            {"W2", "blindmonkwtwo"},
            {"Q2", "blindmonkqtwo"},
            {"E2", "blindmonketwo"},
            {"R1", "BlindMonkRKick"}
        };


        internal enum Spells
        {
            Q,

            W,

            E,

            R
        }



        public enum WCastStage
        {
            First,

            Second,

            Cooldown
        }




        public static float WardFlashRange => WardRange + R.Range - 100;


        public static WCastStage WStage
        {
            get
            {
                if (!W.IsReady())
                {
                    return WCastStage.Cooldown;
                }

                return (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W)
                            .Name.Equals("blindmonkwtwo", StringComparison.InvariantCultureIgnoreCase)
                            ? WCastStage.Second
                            : WCastStage.First);
            }
        }


        private static readonly int[] SmiteBlue = { 3706, 1403, 1402, 1401, 1400 };

        private static readonly int[] SmiteRed = { 3715, 1415, 1414, 1413, 1412 };


        public static bool WState
            => W.Name.Equals("BlindMonkWOne", StringComparison.InvariantCultureIgnoreCase);

        public static bool QState
        {
            get
            {
                return Q.Name == "BlindMonkQOne";
            }
        }

        public static bool EState
        {
            get
            {
                return E.Name == "BlindMonkEOne";
            }
        }

        public static bool Q2State
        {
            get
            {
                return E.Name == "BlindMonkETwo";
            }
        }

        public enum InsecComboStepSelect
        {
            None,

            Qgapclose,

            Wgapclose,

            Pressr
        };


        private static string SmiteSpellName()
        {
            if (SmiteBlue.Any(a => Item.HasItem(a)))
            {
                return "s5_summonersmitemyHeroganker";
            }

            if (SmiteRed.Any(a => Item.HasItem(a)))
            {
                return "s5_summonersmiteduel";
            }

            return "summonersmite";
        }

        public static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowMessages.LeftButtonDown)
            {
                return;
            }

            var asec =
                ObjectManager.Get<AIHeroClient>()
                    .Where(a => a.IsEnemy && a.Distance(Game.CursorPos) < 200 && a.IsValid && !a.IsDead);

            if (asec.Any())
            {
                return;
            }
            if (!lastClickBool || clickCount == 0)
            {
                clickCount++;
                lastClickPos = Game.CursorPos;
                lastClickBool = true;
                doubleClickReset = Environment.TickCount + 600;
                return;
            }
            if (lastClickBool && lastClickPos.Distance(Game.CursorPos) < 200)
            {
                clickCount++;
                lastClickBool = false;
            }
        }

        public static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter))
            {
                return;
            }
            if (sender.Name.Contains("blindmonk_q_resonatingstrike") && waitingForQ2)
            {
                waitingForQ2 = false;
                q2Done = true;
                q2Timer = Environment.TickCount + 800;
            }
        }



        public static void OnCreate(GameObject sender, EventArgs args)
        {
            if (Environment.TickCount < lastPlaced + 300)
            {
                var ward = (Obj_AI_Base)sender;
                if (W.IsReady() && ward.Name.ToLower().Contains("ward")
                    && ward.Distance(lastWardPos) < 500)
                {
                    W.Cast(ward);
                }
            }
        }

        public static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target.IsMe && PassiveStacks > 0)
            {
                PassiveStacks--;
            }
        }


        public static SpellDataInst GetItemSpell(InventorySlot invSlot)
        {
            return myHero.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }

        public static InventorySlot FindBestWardItem()
        {
            try
            {
                var slot = GetWardSlot();
                if (slot == default(InventorySlot))
                {
                    return null;
                }

                var sdi = GetItemSpell(slot);
                if (sdi != default(SpellDataInst) && sdi.State == SpellState.Ready)
                {
                    return slot;
                }
                return slot;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public static InventorySlot GetWardSlot()
        {
            var wardIds = new[] { 2045, 2049, 2050, 2301, 2302, 2303, 3340, 3361, 3362, 3711, 1408, 1409, 1410, 1411, 2043 };
            return (from wardId in wardIds where Item.CanUseItem(wardId) select ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)wardId)).FirstOrDefault();
        }

        public static void UseItems(Obj_AI_Base target)
        {
            if (target == null) { return; }
            if (Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
            }

            if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Tiamat_Melee_Only);
            }

            if (Item.CanUseItem(ItemId.Blade_of_the_Ruined_King) && 550 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Blade_of_the_Ruined_King);
            }

            if (Item.CanUseItem(ItemId.Youmuus_Ghostblade) && myHero.GetAutoAttackRange() > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Youmuus_Ghostblade);
            }
        }

        public static Obj_AI_Base ReturnQBuff()
        {
            return
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsValidTarget(1300))
                    .FirstOrDefault(unit => unit.HasQBuff());
        }

        public static Tuple<int, List<AIHeroClient>> GetEHits()
        {
            try
            {
                var hits =
                    EntityManager.Heroes.Enemies.Where(
                        e => e.IsValidTarget() && e.Distance(myHero) < 430f || e.Distance(myHero) < 430f).ToList();

                return new Tuple<int, List<AIHeroClient>>(hits.Count, hits);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Tuple<int, List<AIHeroClient>>(0, null);
        }

        public static int LastQ, LastQ2, LastW, LastW2, LastE, LastE2, LastR, LastSpell, PassiveStacks;


        public static bool HasQBuff(this Obj_AI_Base unit)
        {
            return (unit.HasAnyBuffs("BlindMonkQOne") || unit.HasAnyBuffs("blindmonkqonechaos")
                    || unit.HasAnyBuffs("BlindMonkSonicWave"));
        }

        private static bool HasAnyBuffs(this Obj_AI_Base unit, string s)
        {
            return
                unit.Buffs.Any(
                    a => a.Name.ToLower().Contains(s.ToLower()) || a.DisplayName.ToLower().Contains(s.ToLower()));
        }

        public static void CastW(Obj_AI_Base obj)
        {
            if (500 >= Environment.TickCount - Wcasttime || WStage != WCastStage.First)
            {
                return;
            }

            W.Cast(obj);
            Wcasttime = Environment.TickCount;
        }

        public static void CastQ(Obj_AI_Base target, bool smiteQ = false)
        {
            if (!Q.IsReady() || !target.IsValidTarget(Q.Range))
            {
                return;
            }

            var prediction = Q.GetPrediction(target);

            if (prediction.HitChance >= HitChance.High)
            {
                Q.Cast(target); //prediction.CastPosition
            }
            else if (MiscMenu.GetCheckBoxValue("smiteq") && Q.IsReady()
                     && target.IsValidTarget(Q.Range)
                     && prediction.CollisionObjects.Count(a => a.NetworkId != target.NetworkId && a.IsMinion) == 1
                     && Smite.IsReady())
            {
                Smite.Cast(prediction.CollisionObjects.Where(a => a.NetworkId != target.NetworkId && a.IsMinion).ToList()[0]);

                Q.Cast(prediction.CastPosition);
            }
        }

        public static float GetTotalDamage(this Obj_AI_Base target)
        {
            var slots = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            var dmg = Player.Spells.Where(s => slots.Contains(s.Slot)).Sum(s => target.GetDamage(s.Slot));
            dmg += Orbwalker.CanAutoAttack ? Player.Instance.GetAutoAttackDamage(target) : 0f;

            return dmg;
        }

        /// <summary>
        /// Gets the minion that can be lasthitable by the spell using the custom damage provided by you in spellmanager
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static Obj_AI_Minion GetLastHitMinion(this Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .FirstOrDefault(
                        m =>
                            m.IsValidTarget(spell.Range) && Prediction.Health.GetPrediction(m, spell.CastDelay) <= m.GetDamage(spell.Slot) &&
                            m.IsEnemy);
        }

        /// <summary>
        /// Gets the hero that can be killable by the spell using the custom damage provided by you in spellmanager
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static AIHeroClient GetKillableHero(this Spell.SpellBase spell)
        {
            return
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    e =>
                        e.IsValidTarget(spell.Range) && Prediction.Health.GetPrediction(e, spell.CastDelay) <= e.GetDamage(spell.Slot) &&
                        !e.HasUndyingBuff());
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance * Vector3.Normalize(direction - from).To2D();
        }


        public static void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        private static List<AIHeroClient> GetAllyHeroes(AIHeroClient position, int range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.Distance(position) < range)
                    .ToList();
        }

        private static List<AIHeroClient> GetAllyInsec(List<AIHeroClient> heroes)
        {
            byte alliesAround = 0;
            var tempObject = new AIHeroClient();
            foreach (var hero in heroes)
            {
                var localTemp =
                    GetAllyHeroes(hero, 500 + InsecMenu.GetSliderValue("bonusrangea")).Count;
                if (localTemp > alliesAround)
                {
                    tempObject = hero;
                    alliesAround = (byte)localTemp;
                }
            }
            return GetAllyHeroes(tempObject, 500 + InsecMenu.GetSliderValue("bonusrangea"));
        }

        private static Vector3 InterceptionPoint(List<AIHeroClient> heroes)
        {
            var result = new Vector3();
            foreach (var hero in heroes)
            {
                result += hero.Position;
            }
            result.X /= heroes.Count;
            result.Y /= heroes.Count;
            return result;
        }

          public static T MinOrDefault<T, TR>(this IEnumerable<T> container, Func<T, TR> valuingFoo)
            where TR : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var minElem = enumerator.Current;
            var minVal = valuingFoo(minElem);
            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);
                if (currVal.CompareTo(minVal) >= 0)
                {
                    continue;
                }

                minVal = currVal;
                minElem = enumerator.Current;
            }

            return minElem;
        }

        public static Vector3 GetInsecPos(AIHeroClient target)
        {
            try
            {
                if (ClicksecEnabled && InsecMenu.GetCheckBoxValue("clickInsec"))
                {
                    InsecLinePos = Drawing.WorldToScreen(InsecClickPos);
                    return V2E(InsecClickPos, target.Position, target.Distance(InsecClickPos) + 230).To3D();
                }
                if (isNullInsecPos)
                {
                    isNullInsecPos = false;
                    insecPos = myHero.Position;
                }

                if (GetAllyHeroes(target, 2000 + InsecMenu.GetSliderValue("bonusrangea")).Count > 0
                    && InsecMenu.GetCheckBoxValue("insecally"))
                {
                    var insecPosition =
                    InterceptionPoint(
                        GetAllyInsec(
                            GetAllyHeroes(target, 2000 + InsecMenu.GetSliderValue("bonusrangea"))));

                    InsecLinePos = Drawing.WorldToScreen(insecPosition);
                    return V2E(insecPosition, target.Position, target.Distance(insecPosition) + 230).To3D();
                }

                var tower =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(
                            turret =>
                            turret.Distance(target) - 725 <= 950 && turret.IsAlly && turret.IsVisible
                            && turret.Health > 0 && turret.Distance(target) <= 1300 && turret.Distance(target) > 400)
                        .MinOrDefault(i => target.Distance(myHero));

                if (tower != null)
                {
                    InsecLinePos = Drawing.WorldToScreen(tower.Position);
                    return V2E(tower.Position, target.Position, target.Distance(tower.Position) + 230).To3D();
                }

                if (InsecMenu.GetCheckBoxValue("insecpos"))
                {
                    InsecLinePos = Drawing.WorldToScreen(insecPos);
                    return V2E(insecPos, target.Position, target.Distance(insecPos) + 230).To3D();
                }
                return new Vector3();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Vector3();
        }
    }
}
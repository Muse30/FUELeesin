using EloBuddy;
using EloBuddy.SDK;
using Mario_s_Lib;
using System;
using System.Linq;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;
using EloBuddy.SDK.Events;

namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class Combo
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }

            if (!target.IsZombie && R.IsReady() && ComboMenu.GetCheckBoxValue("rUse") && ComboMenu.GetCheckBoxValue("qUse") && target.IsValidTarget(R.Range) && (myHero.GetSpellDamage(target, SpellSlot.R) >= target.Health || target.HasQBuff() && target.Health < myHero.GetSpellDamage(target, SpellSlot.R) + Q2Damage(target, myHero.GetSpellDamage(target, SpellSlot.R))))

            {
                R.Cast(target);
            }

            if (ComboMenu.GetCheckBoxValue("q2Use") && Q.IsReady()
                && Q.Name.Equals("blindmonkqtwo", StringComparison.InvariantCultureIgnoreCase)
                && target.HasQBuff() && (castQAgain))
            {
                CastQ(target, MiscMenu.GetCheckBoxValue("smiteq"));
                return;
            }

            if (target.HasQBuff() && ComboMenu.GetCheckBoxValue("q2Use"))
            {
                if (!target.IsZombie && target.HasQBuff() && target.Health < myHero.GetSpellDamage(target, SpellSlot.R) + Q2Damage(target, myHero.GetSpellDamage(target, SpellSlot.R)))
                {
                    if (ComboMenu.GetCheckBoxValue("rUseks"))
                    {
                        R.Cast(target);
                    }
                }
            }

            if (target.IsValidTarget(385f))
            {
                UseItems(target);
            }

            if (ComboMenu.GetCheckBoxValue("q2Use") && Q.IsReady()
                && Q.Name.Equals("blindmonkqtwo", StringComparison.InvariantCultureIgnoreCase)
                && target.HasQBuff()
                && (castQAgain || myHero.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.SecondCast) > target.Health)
                || ReturnQBuff()?.Distance(target) < myHero.Distance(target)
                && !target.IsValidTarget(myHero.GetAutoAttackRange()))
            {
                Q.Cast(target);
            }

            if (!target.IsZombie && myHero.GetSpellDamage(target, SpellSlot.R) >= target.Health
                && ComboMenu.GetCheckBoxValue("rUseks") && target.IsValidTarget(R.Range))
            {
                R.Cast(target);
            }

            if (Q.IsReady() && QState && ComboMenu.GetCheckBoxValue("qUse"))
            {
                CastQ(target, MiscMenu.GetCheckBoxValue("smiteq"));
            }

            if (ComboMenu.GetCheckBoxValue("Cpassive")
                && PassiveStacks > ComboMenu.GetSliderValue("passivestacks")
                && myHero.GetAutoAttackRange() > myHero.Distance(target))
            {
                return;
            }

            if (ComboMenu.GetCheckBoxValue("q2Use") && Q.IsReady()
                && Q.Name.Equals("blindmonkqtwo", StringComparison.InvariantCultureIgnoreCase)
                && target.HasQBuff()
                && (castQAgain || myHero.Distance(target) > myHero.GetAutoAttackRange()
                    || myHero.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.SecondCast) > target.Health + target.AttackShield))
            {
                CastQ(target, MiscMenu.GetCheckBoxValue("smiteq"));
                return;
            }

            if (ComboMenu.GetCheckBoxValue("wardjumpC"))
            {
                if (ComboMenu.GetCheckBoxValue("wardjumpCAA")
                    && target.Distance(myHero) > myHero.GetAutoAttackRange())
                {
                    WardJumper.WardJump(target.Position, false, true);
                }

                if (!ComboMenu.GetCheckBoxValue("wardjumpCAA") && target.Distance(myHero) > Q.Range)
                {
                    WardJumper.WardJump(target.Position, false, true);
                }
            }

            if (E.IsReady() && ComboMenu.GetCheckBoxValue("eUse") && target.Distance(myHero) < 400
                && !myHero.IsDashing())
            {
                if (Environment.TickCount - LastW <= 150)
                {
                    return;
                }

                if (EState)
                {
                    if (GetEHits().Item1 > 0 || (PassiveStacks == 0 && myHero.Mana >= 70)
                        || target.Distance(myHero) > myHero.GetAutoAttackRange() + 70)
                    {
                        E.Cast();
                    }
                }
                else
                {
                    if (LastE + 1800 < Environment.TickCount)
                    {
                        if (GetEHits().Item1 > 0
                            || target.Distance(myHero) > myHero.GetAutoAttackRange() + 50)
                        {
                            E.Cast();
                        }
                    }
                }
            }

            if (W.IsReady() && ComboMenu.GetCheckBoxValue("wUse"))
            {
                if (Environment.TickCount - LastE <= 250)
                {
                    return;
                }

                if (WState && target.IsValidTarget(myHero.GetAutoAttackRange()))
                {
                    W.Cast(myHero);
                    LastW = Environment.TickCount;
                }

                if (!WState && LastW + 1800 < Environment.TickCount)
                {
                    W.Cast(myHero);
                }
            }
        }

        internal static void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (SpellNames.Contains(args.SData.Name.ToLower()))
            {
                PassiveStacks = 2;
                passiveTimer = Environment.TickCount + 3000;
                LastSpellTime = Environment.TickCount;
            }

            if (args.SData.Name.Equals("BlindMonkQOne", StringComparison.InvariantCultureIgnoreCase))
            {
                castQAgain = false;
                Core.DelayAction(delegate { castQAgain = true; }, 2900);
            }

            if (R.IsReady() && Flash.IsReady())
            {
                var target = InsecMenu.GetCheckBoxValue("insecMode")
                                 ? TargetSelector.SelectedTarget
                                 : TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (target == null)
                {
                    return;
                }
                if (args.SData.Name.Equals("BlindMonkRKick", StringComparison.InvariantCultureIgnoreCase)
                    && InsecMenu.GetKeyBindValue("insecflash"))
                {
                    Core.DelayAction(delegate { Flash.Cast(GetInsecPos(target)); }, 80);
                }
            }

            if (args.SData.Name.Equals("summonerflash", StringComparison.InvariantCultureIgnoreCase)
                && insecComboStep != InsecComboStepSelect.None)
            {
                var target = InsecMenu.GetCheckBoxValue("insecMode")
                                 ? TargetSelector.SelectedTarget
                                 : TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                insecComboStep = InsecComboStepSelect.Pressr;

                Core.DelayAction(delegate { R.Cast(target); }, 80);
            }
            if (args.SData.Name.Equals("BlindMonkQTwo", StringComparison.InvariantCultureIgnoreCase))
            {
                waitingForQ2 = true;
                Core.DelayAction(delegate { waitingForQ2 = false; }, 3000);
            }
            if (args.SData.Name.Equals("BlindMonkRKick", StringComparison.InvariantCultureIgnoreCase))
            {
                insecComboStep = InsecComboStepSelect.None;
            }

            switch (args.SData.Name.ToLower())
            {
                case "blindmonkqone":
                    LastQ = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "blindmonkwone":
                    LastW = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "blindmonkeone":
                    LastE = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "blindmonkqtwo":
                    LastQ2 = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    CheckQ = false;
                    break;
                case "blindmonkwtwo":
                    LastW2 = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "blindmonketwo":
                    LastQ = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "blindmonkrkick":
                    LastR = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
            }
        }
    }
}

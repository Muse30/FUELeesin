using EloBuddy;
using EloBuddy.SDK;
using Mario_s_Lib;
using System;
using System.Linq;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;


namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class Harass
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                if (!QState && LastQ + 200 < Environment.TickCount && HarassMenu.GetCheckBoxValue("q1Use") && !QState
                    && Q.IsReady() && target.HasQBuff()
                    && (LastQ + 2700 < Environment.TickCount || myHero.GetSpellDamage(target, SpellSlot.Q) >= target.Health
                        || target.Distance(myHero) > myHero.GetAutoAttackRange() + 50))
                {
                    Q.Cast();
                }

                if (ComboMenu.GetCheckBoxValue("Cpassive")
                    && PassiveStacks > ComboMenu.GetSliderValue("passivestacks")
                    && myHero.GetAutoAttackRange() > myHero.Distance(target))
                {
                    return;
                }

                if (Q.IsReady() && HarassMenu.GetCheckBoxValue("q1Use") && LastQ + 200 < Environment.TickCount)
                {
                    if (QState && target.Distance(myHero) < Q.Range)
                    {
                        CastQ(target, MiscMenu.GetCheckBoxValue("smiteq"));
                    }
                }

                if (E.IsReady() && HarassMenu.GetCheckBoxValue("eeUse") && LastE + 200 < Environment.TickCount)
                {
                    if (EState && target.Distance(myHero) < E.Range)
                    {
                        E.Cast();
                        return;
                    }

                    if (!EState && target.Distance(myHero) > myHero.GetAutoAttackRange() + 50)
                    {
                        E.Cast();
                    }
                }

                if (HarassMenu.GetCheckBoxValue("wUse") && myHero.Distance(target) < 50 && !(target.HasQBuff())
                    && (EState || !E.IsReady() && HarassMenu.GetCheckBoxValue("eeUse"))
                    && (QState || !Q.IsReady() && HarassMenu.GetCheckBoxValue("q1Use")))
                {
                    var min =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(a => a.IsAlly && a.Distance(myHero) <= W.Range)
                            .OrderByDescending(a => a.Distance(target))
                            .FirstOrDefault();

                    W.Cast(min);
                }
            }
        }

    }
}
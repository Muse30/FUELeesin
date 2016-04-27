using EloBuddy;
using EloBuddy.SDK;
using Mario_s_Lib;
using System.Linq;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;
using System;

namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class LaneClear
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, null, Q.Range).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

           UseItems(minions);

            if (LaneClearMenu.GetCheckBoxValue("qUse") && !QState && Q.IsReady() && minions.HasQBuff()
                && (LastQ + 2700 < Environment.TickCount || myHero.GetSpellDamage(minions, SpellSlot.Q, DamageLibrary.SpellStages.SecondCast) > minions.Health
                    || minions.Distance(myHero) > myHero.GetAutoAttackRange() + 50))
            {
                Q.Cast(minions);
            }

            if (Q.IsReady() && LaneClearMenu.GetCheckBoxValue("qUse") && LastQ + 200 < Environment.TickCount)
            {
                if (QState && minions.Distance(myHero) < Q.Range)
                {
                    Q.Cast(minions);
                }
            }

            
            if (ComboMenu.GetCheckBoxValue("Cpassive")
                && PassiveStacks > ComboMenu.GetSliderValue("passivestacks")
                && myHero.GetAutoAttackRange() > myHero.Distance(minions))
            {
                return;
            }

            if (E.IsReady() && LaneClearMenu.GetCheckBoxValue("eUse"))
            {
                if (EState && E.IsInRange(minions))
                {
                    LastE = Environment.TickCount;
                    E.Cast();
                    return;
                }

                if (!EState && E.IsInRange(minions) && LastE + 400 < Environment.TickCount)
                {
                    E.Cast();
                }
            }
        }
    }
}
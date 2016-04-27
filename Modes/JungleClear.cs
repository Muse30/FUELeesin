using EloBuddy.SDK;
using Mario_s_Lib;
using System;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;
using System.Linq;

namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class JungleClear
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(null, Q.Range).FirstOrDefault();

            if (minion == null)
            {
                return;
            }

            if (PassiveStacks > 0 || LastSpell + 400 > Environment.TickCount)
            {
                return;
            }

            if (W.IsReady() && JungleClearMenu.GetCheckBoxValue("wUse"))
            {
                if (Environment.TickCount - LastE <= 50)
                {
                    return;
                }

                if (WState && minion.IsValidTarget(myHero.GetAutoAttackRange()))
                {
                    W.Cast(myHero);
                    LastW = Environment.TickCount;
                }

                if (!WState && LastW + 1000 < Environment.TickCount)
                {
                    W.Cast(myHero);
                }
            }

            if (E.IsReady() && JungleClearMenu.GetCheckBoxValue("eUse"))
            {
                if (EState && E.IsInRange(minion))
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                    return;
                }

                if (!EState && E.IsInRange(minion) && LastE + 400 < Environment.TickCount)
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                }
            }

            if (Q.IsReady() && JungleClearMenu.GetCheckBoxValue("qUse"))
            {
                if (QState && minion.Distance(myHero) < Q.Range && LastQ + 200 < Environment.TickCount)
                {
                    Q.Cast(minion);
                    LastSpell = Environment.TickCount;
                    return;
                }

                Q2.Cast();

            }
        }
    }
}
using EloBuddy.SDK;
using Mario_s_Lib;
using System;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;
using System.Linq;
using EloBuddy;

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

            if (JungleClearMenu.GetCheckBoxValue("Jpassive"))
            {
                if (PassiveStacks > 0 || LastSpell + 400 > Environment.TickCount)
                {
                    return;
                }
            }

            if (E.IsReady() && JungleClearMenu.GetCheckBoxValue("eUse"))
            {
                if (EState && E.IsInRange(minion))
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                }

                if (!EState && E.IsInRange(minion) && LastE + 400 < Environment.TickCount)
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                }
            }

            if (Q.IsReady() && JungleClearMenu.GetCheckBoxValue("qUse"))
            {
                Q.Cast(minion);
                LastSpell = Environment.TickCount;

                foreach (var jungleMobs in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Q.Range) && o.Team == GameObjectTeam.Neutral && o.IsVisible && !o.IsDead))
                {

                    if (EntityManager.MinionsAndMonsters.GetJungleMonsters(null, Q.Range).Any())
                    {
                        if (jungleMobs.HasQBuff())
                        {
                            Q2.Cast();
                            LastSpell = Environment.TickCount;
                        }
                    }
                }
            }

            if (W.IsReady()  && JungleClearMenu.GetCheckBoxValue("wUse"))
            {
                if (WState)
                {
                    myHero.GetAutoAttackRange();
                    W.Cast(myHero);
                    LastSpell = Environment.TickCount;
                }

                if (WState)
                {
                    return;
                }

                W2.Cast();
                LastSpell = Environment.TickCount;
                return;
            }
        }
    }
}

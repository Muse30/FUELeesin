using EloBuddy;
using EloBuddy.SDK;
using Mario_s_Lib;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;


namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class WardCombo
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(1500, DamageType.Physical);

            Orbwalker.OrbwalkTo(Game.CursorPos);

            if (target == null)
            {
                return;
            }

            UseItems(target);

            if (target.HasQBuff())
            {
                if (castQAgain
                    || target.HasBuffOfType(BuffType.Knockback) && !myHero.IsValidTarget(300)
                    && !R.IsReady()
                    || !target.IsValidTarget(myHero.GetAutoAttackRange()) && !R.IsReady())
                {
                    Q.Cast(target);
                }
            }
            if (target.Distance(myHero) > R.Range
                && target.Distance(myHero) < R.Range + 580 && target.HasQBuff())
            {
                WardJumper.WardJump(target.Position, false);
            }

            if (E.IsReady() && EState && target.IsValidTarget(E.Range))
            {
                E.Cast();
            }

            if (R.IsReady() && Q.IsReady() && target.HasQBuff())
            {
                R.Cast(target);
            }

            if (Q.IsReady() && QState)
            {
                Core.DelayAction(delegate { CastQ(target); }, 200);
            }
        }
    }
 }

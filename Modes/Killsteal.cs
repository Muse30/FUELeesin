using EloBuddy.SDK;
using Mario_s_Lib;
using System.Linq;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;



namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class Killsteal
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {

            foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
            {
                if (ComboMenu.GetCheckBoxValue("eUseks") && E.IsReady() && enemy.Health < EDamage(enemy) && E.Name == Spellss["E1"] &&
                    enemy.Distance(myHero) < 430)
                {
                    E.Cast();
                    return;
                }
                if (ComboMenu.GetCheckBoxValue("q2Useks") && Q.IsReady() && enemy.HasQBuff() && enemy.Health < Q2Damage(enemy) && Q.Name == Spellss["Q2"] && enemy.Distance(myHero) < 1400)
                {
                    Q2.Cast();
                    return;
                }
                if (ComboMenu.GetCheckBoxValue("q1Useks") && Q.IsReady() && enemy.Health < QDamage(enemy) && Q.Name == Spellss["Q1"] && enemy.Distance(myHero) < 1100)
                {
                    Q.Cast(enemy);
                    return;
                }

            }

            }
        }
    }

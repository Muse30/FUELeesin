using System;
using EloBuddy;
using EloBuddy.SDK;
using Mario_s_Lib;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using static FUELeesin.Menus;

namespace FUELeesin.Modes
{
    internal class ModeManager
    {
        /// <summary>
        /// Create the event on tick
        /// </summary>
        public static void InitializeModes()
        {
            Game.OnTick += Game_OnTick;
        }

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        /// <summary>
        /// This event is triggered every tick of the game
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnTick(EventArgs args)
        {
            if (Extensions.doubleClickReset <= Environment.TickCount && Extensions.clickCount != 0)
            {
                Extensions.doubleClickReset = float.MaxValue;
                Extensions.clickCount = 0;
            }

            if (Extensions.clickCount >= 2)
            {
                Extensions.resetTime = Environment.TickCount + 3000;
                Extensions.ClicksecEnabled = true;
                Extensions.InsecClickPos = Game.CursorPos;
                Extensions.clickCount = 0;
            }


            if (Extensions.passiveTimer <= Environment.TickCount)
            {
                Extensions.PassiveStacks = 0;
            }

            if (Extensions.resetTime <= Environment.TickCount && !InsecMenu.GetKeyBindValue("insec")
                && Extensions.ClicksecEnabled)
            {
                Extensions.ClicksecEnabled = false;
            }

            if (Extensions.q2Timer <= Environment.TickCount)
            {
                Extensions.q2Done = false;
            }

            if (myHero.IsDead || MenuGUI.IsChatOpen || myHero.IsRecalling())
            {
                return;
            }


            if ((InsecMenu.GetCheckBoxValue("insecMode") ? TargetSelector.SelectedTarget
                     : TargetSelector.GetTarget(SpellsManager.Q.Range, DamageType.Physical)) == null)
            {
                Extensions.insecComboStep = Extensions.InsecComboStepSelect.None;
            }

            if (ComboMenu.GetKeyBindValue("starCombo"))
            {
                WardCombo.Execute();
            }

            if (MiscMenu.GetCheckBoxValue("igniteks"))
            {
                var newTarget = TargetSelector.GetTarget(600, DamageType.True);

                if (newTarget != null && SpellsManager.igniteSlot != SpellSlot.Unknown
                    && myHero.Spellbook.CanUseSpell(SpellsManager.igniteSlot) == SpellState.Ready
                    && ObjectManager.Player.GetSummonerSpellDamage(newTarget, DamageLibrary.SummonerSpells.Ignite) > newTarget.Health)
                {
                    myHero.Spellbook.CastSpell(SpellsManager.igniteSlot, newTarget);
                }
            }

            if (InsecMenu.GetKeyBindValue("insec") && SpellsManager.R.IsReady())
            {
                if (InsecMenu.GetCheckBoxValue("insecorbw"))
                {
                    Extensions.Orbwalk(Game.CursorPos);
                }

                var newTarget = InsecMenu.GetCheckBoxValue("insecMode") ? TargetSelector.SelectedTarget : TargetSelector.GetTarget(SpellsManager.Q.Range, DamageType.Physical);

                if (newTarget != null)
                {
                    Insec.InsecCombo(newTarget);
                }
            }
            else
            {
                Extensions.isNullInsecPos = true;
                Extensions.wardJumped = false;
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Extensions.insecComboStep = Extensions.InsecComboStepSelect.None;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear.Execute();
            }

            Killsteal.Execute();


            if (WardJumpMenu.GetKeyBindValue("wardjump"))
            {
                WardJumper.WardjumpToMouse();
            }
            
            if (InsecMenu.GetKeyBindValue("insecflash"))
            {
                Extensions.Orbwalk(Game.CursorPos);

                var target = TargetSelector.GetTarget(SpellsManager.R.Range, DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                if (SpellsManager.R.IsReady() && !target.IsZombie
                    && SpellsManager.Flash.IsReady()
                    && target.IsValidTarget(SpellsManager.R.Range))
                {
                    SpellsManager.R.Cast(target);
                }
            }

            if (ComboMenu.GetCheckBoxValue("rkick"))
            {
                float leeSinRKickDistance = 700;
                float leeSinRKickWidth = 100;
                var minREnemies = ComboMenu.GetSliderValue("rkickcount");
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    var startPos = enemy.ServerPosition;
                    var endPos = myHero.ServerPosition.Extend(startPos, myHero.Distance(enemy) + leeSinRKickDistance);
                    var rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, leeSinRKickWidth);

                    if (EntityManager.Heroes.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies)
                    {
                      SpellsManager.R.Cast(enemy);
                    }
                }
            }
        }
    }
}
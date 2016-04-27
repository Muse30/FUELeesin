using EloBuddy;
using EloBuddy.SDK;
using Mario_s_Lib;
using SharpDX;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;
using System.Linq;
using System;
using System.Collections.Generic;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

using Color = System.Drawing.Color;

namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class WardJumper
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void WardJump(Vector3 pos, bool m2M = true, bool maxRange = false, bool reqinMaxRange = false, bool minions = true, bool champions = true)
        {
            if (WStage != WCastStage.First)
            {
                return;
            }

            var basePos = myHero.Position.To2D();
            var newPos = (pos.To2D() - myHero.Position.To2D());

            if (JumpPos == new Vector2())
            {
                if (reqinMaxRange)
                {
                    JumpPos = pos.To2D();
                }
                else if (maxRange || myHero.Distance(pos) > 590)
                {
                    JumpPos = basePos + (newPos.Normalized() * (590));
                }
                else
                {
                    JumpPos = basePos + (newPos.Normalized() * (myHero.Distance(pos)));
                }
            }
            if (JumpPos != new Vector2() && reCheckWard)
            {
                reCheckWard = false;
                Core.DelayAction(delegate
                {
                    if (JumpPos != new Vector2())
                    {
                        JumpPos = new Vector2();
                        reCheckWard = true;
                    }
                }, 20);
            }
            if (m2M)
            {
                Orbwalk(pos);
            }
            if (!W.IsReady() || WStage != WCastStage.First
                || reqinMaxRange && myHero.Distance(pos) > W.Range)
            {
                return;
            }

            if (minions || champions)
            {
                if (champions)
                {
                    var wardJumpableChampion =
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                x =>
                                x.IsAlly && x.Distance(myHero) < W.Range && x.Distance(pos) < 200
                                && !x.IsMe)
                            .OrderByDescending(i => i.Distance(myHero))
                            .ToList()
                            .FirstOrDefault();

                    if (wardJumpableChampion != null && WStage == WCastStage.First)
                    {
                        if (500 >= Environment.TickCount - Wcasttime || WStage != WCastStage.First)
                        {
                            return;
                        }

                        CastW(wardJumpableChampion);
                        return;
                    }
                }
                if (minions)
                {
                    var wardJumpableMinion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                m =>
                                m.IsAlly && m.Distance(myHero) < W.Range && m.Distance(pos) < 200
                                && !m.Name.ToLower().Contains("ward"))
                            .OrderByDescending(i => i.Distance(myHero))
                            .ToList()
                            .FirstOrDefault();

                    if (wardJumpableMinion != null && WStage == WCastStage.First)
                    {
                        if (500 >= Environment.TickCount - Wcasttime || WStage != WCastStage.First)
                        {
                            return;
                        }

                        CastW(wardJumpableMinion);
                        return;
                    }
                }
            }

            var isWard = false;

            var wardObject =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(o => o.IsAlly && o.Name.ToLower().Contains("ward") && o.Distance(JumpPos) < 200)
                    .ToList()
                    .FirstOrDefault();

            if (wardObject != null)
            {
                isWard = true;
                if (500 >= Environment.TickCount - Wcasttime || WStage != WCastStage.First)
                {
                    return;
                }

                CastW(wardObject);
            }

            if (!isWard && castWardAgain)
            {
                var ward = FindBestWardItem();
                if (ward == null)
                {
                    return;
                }

                if (W.IsReady() && WState && LastWard + 400 < Environment.TickCount)
                {
                    myHero.Spellbook.CastSpell(ward.SpellSlot, JumpPos.To3D());
                    lastWardPos = JumpPos.To3D();
                    LastWard = Environment.TickCount;
                }
            }
        }

        public static void WardjumpToMouse()
        {
            WardJump(
                Game.CursorPos,
                WardJumpMenu.GetCheckBoxValue("mousej"),
                false,
                false,
                WardJumpMenu.GetCheckBoxValue("minionsj"),
                WardJumpMenu.GetCheckBoxValue("champsj"));
        }

    }
}

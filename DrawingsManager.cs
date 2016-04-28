using System;
using EloBuddy;
using EloBuddy.SDK.Rendering;
using Mario_s_Lib;
using static FUELeesin.SpellsManager;
using static FUELeesin.Menus;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK;

namespace FUELeesin
{
    internal class DrawingsManager
    {
        public static void InitializeDrawings()
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            DamageIndicator.Init();
        }



        /// <summary>
        /// Normal drawings
        /// </summary>
        /// <param name="args"></param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            var newTarget = InsecMenu.GetCheckBoxValue("insecMode") ? TargetSelector.SelectedTarget : TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            
            if (Extensions.ClicksecEnabled && InsecMenu.GetCheckBoxValue("clickInsec"))
            {
                Drawing.DrawCircle(Extensions.InsecClickPos, 100, Color.DeepSkyBlue);
            }

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (InsecMenu.GetKeyBindValue("insecflash"))
            {
                Drawing.DrawText(playerPos.X - 55, playerPos.Y + 40, Color.Yellow, "Flash Insec enabled");
            }

            if (DrawingsMenu.GetCheckBoxValue("drawinseclines") && R.IsReady())
            {
                if (newTarget != null && newTarget.IsVisible && newTarget.IsValidTarget() && !newTarget.IsDead && myHero.Distance(newTarget) < 3000)
                {
                    Vector2 targetPos = Drawing.WorldToScreen(newTarget.Position);
                    Drawing.DrawLine(
                        Extensions.InsecLinePos.X,
                        Extensions.InsecLinePos.Y,
                        targetPos.X,
                        targetPos.Y,
                        3,
                        Color.Gold);

                    Drawing.DrawText(
                        Drawing.WorldToScreen(newTarget.Position).X - 40,
                        Drawing.WorldToScreen(newTarget.Position).Y + 10,
                        Color.White,
                        "Selected Target");

                    Drawing.DrawCircle(Extensions.GetInsecPos(newTarget), 100, Color.DeepSkyBlue);

                }
            }

            if (!DrawingsMenu.GetCheckBoxValue("drawsenabled"))
            {
                return;
            }

            

             if (WardJumpMenu.GetKeyBindValue("wardjump") && DrawingsMenu.GetCheckBoxValue("drawwardjump"))
            {
                Drawing.DrawCircle(Extensions.JumpPos.To3D(), 20, Color.Red);
                Drawing.DrawCircle(myHero.Position, 600, Color.Red);
            }
            var readyDraw = DrawingsMenu.GetCheckBoxValue("readyDraw");

            if (DrawingsMenu.GetCheckBoxValue("qDraw") && readyDraw ? Q.IsReady() : DrawingsMenu.GetCheckBoxValue("qDraw"))
            {
                Circle.Draw(QColorSlide.GetSharpColor(), Q.Range, 1f, Player.Instance);
            }

            if (DrawingsMenu.GetCheckBoxValue("wDraw") && readyDraw ? W.IsReady() : DrawingsMenu.GetCheckBoxValue("wDraw"))
            {
                Circle.Draw(WColorSlide.GetSharpColor(), W.Range, 1f, Player.Instance);
            }

            if (DrawingsMenu.GetCheckBoxValue("eDraw") && readyDraw ? E.IsReady() : DrawingsMenu.GetCheckBoxValue("eDraw"))
            {
                Circle.Draw(EColorSlide.GetSharpColor(), E.Range, 1f, Player.Instance);
            }

            if (DrawingsMenu.GetCheckBoxValue("rDraw") && readyDraw ? R.IsReady() : DrawingsMenu.GetCheckBoxValue("rDraw"))
            {
                Circle.Draw(RColorSlide.GetSharpColor(), R.Range, 1f, Player.Instance);
            }
        }

        /// <summary>
        /// This drawing will override some of the lol`s, like healthbars menus and atc
        /// </summary>
        /// <param name="args"></param>
        private static void Drawing_OnEndScene(EventArgs args)
        {
        }
    }
}
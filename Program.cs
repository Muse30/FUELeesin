using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using FUELeesin.Modes;
using static FUELeesin.Extensions;
using EloBuddy.SDK;

namespace FUELeesin
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        /// <summary>
        /// The firs thing that runs on the template
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        /// <summary>
        /// This event is triggered when the game loads
        /// </summary>
        /// <param name="args"></param>
        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            //Put the name of the champion here
            if (Player.Instance.ChampionName != "LeeSin") return;

            SpellsManager.InitializeSpells();
            Menus.CreateMenu();
            ModeManager.InitializeModes();
            DrawingsManager.InitializeDrawings();
            Combo.OnLoad();
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Smiter.Init();
            Game.OnWndProc += Game_OnWndProc;
        }
    }
}
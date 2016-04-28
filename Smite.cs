using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace FUELeesin
{
    class Smiter
    {
        public static readonly string[] SmiteableUnits =
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron"
        };

        private static readonly int[] SmiteRed = { 3715, 1415, 1414, 1413, 1412 };
        private static readonly int[] SmiteBlue = { 3706, 1403, 1402, 1401, 1400 };

        public static Spell.Targeted Smite;
        public static Menu SmiteMenu;

        private static void SetSmiteSlot()
        {
            SpellSlot smiteSlot;
            if (SmiteBlue.Any(x => ObjectManager.Player.InventoryItems.FirstOrDefault(a => a.Id == (ItemId)x) != null))
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("s5_summonersmiteplayerganker");
            else if (SmiteRed.Any(x => ObjectManager.Player.InventoryItems.FirstOrDefault(a => a.Id == (ItemId)x) != null))
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("s5_summonersmiteduel");
            else
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("summonersmite");
            Smite = new Spell.Targeted(smiteSlot, 500);
        }

        public static int GetSmiteDamage()
        {
            if (Smite == null || !Smite.IsReady())
            {
                return 0;
            }
            int level = ObjectManager.Player.Level;
            int[] smitedamage =
            {
                20*level + 370,
                30*level + 330,
                40*level + 240,
                50*level + 100
            };
            return smitedamage.Max();
        }

        public static void Init()
        {
            SmiteMenu = Menus.FirstMenu.AddSubMenu("• Smite Settings");
            SmiteMenu.AddGroupLabel("Smite Settings");
            SmiteMenu.AddLabel("Combo Settings");
            SmiteMenu.Add("smiteQ", new CheckBox("Q -> Smite"));
            SmiteMenu.AddLabel("Settings");
            SmiteMenu.Add("smiteEnabled", new KeyBind("Smite Enabled", false, KeyBind.BindTypes.PressToggle, 'H'));
            SmiteMenu.AddSeparator();
            SmiteMenu.Add("regularSmite", new CheckBox("Regular Smite"));
            SmiteMenu.Add("QSmite", new CheckBox("Q -> Smite -> Q"));
            SmiteMenu.Add("drawSmite", new CheckBox("Draw Smite Range"));
            SmiteMenu.Add("drawSmite1", new CheckBox("Draw Smite Status"));
            SmiteMenu.AddLabel("Camps");
            SmiteMenu.Add("SRU_Red", new CheckBox("Red"));
            SmiteMenu.Add("SRU_Blue", new CheckBox("Blue"));
            SmiteMenu.Add("SRU_Dragon", new CheckBox("Dragon"));
            SmiteMenu.Add("SRU_Baron", new CheckBox("Baron"));

            SetSmiteSlot();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (SmiteMenu["drawSmite"].Cast<CheckBox>().CurrentValue && SmiteMenu["smiteEnabled"].Cast<KeyBind>().CurrentValue)
            {
                Circle.Draw(Color.White, Smite.Range, Player.Instance.Position);
            }

            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var green = System.Drawing.Color.LimeGreen;
            var red = System.Drawing.Color.IndianRed;

            if (SmiteMenu["drawSmite1"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawText(heropos.X - 40, heropos.Y + 20, System.Drawing.Color.FloralWhite, "Smite:");
                Drawing.DrawText(heropos.X + 10, heropos.Y + 20,
                    SmiteMenu["smiteEnabled"].Cast<KeyBind>().CurrentValue ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red,
                    SmiteMenu["smiteEnabled"].Cast<KeyBind>().CurrentValue ? "On" : "Off");

            }
        }



        public static bool ForceSmite;

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!SmiteMenu["smiteEnabled"].Cast<KeyBind>().CurrentValue || Smite == null) return;

            SetSmiteSlot();

            var minion = ObjectManager.Get<Obj_AI_Base>().Where(a => SmiteableUnits.Contains(a.BaseSkinName) && SmiteMenu[a.BaseSkinName].Cast<CheckBox>() != null && SmiteMenu[a.BaseSkinName].Cast<CheckBox>().CurrentValue).OrderByDescending(a => a.MaxHealth).FirstOrDefault(a => a.IsValidTarget(1400));
            if (minion == null) return;
            if (Smite.IsReady() && minion.IsValidTarget(Smite.Range) && minion.Health <= GetSmiteDamage() && SmiteMenu["regularSmite"].Cast<CheckBox>().CurrentValue || ForceSmite && Player.Instance.Distance(minion) < 100)
            {
                Smite.Cast(minion);
                ForceSmite = false;
                return;
            }
            if (SpellsManager.Q.IsReady() && minion.HasQBuff() && SmiteMenu["QSmite"].Cast<CheckBox>().CurrentValue && minion.Health <= SpellsManager.Q2Damage(minion, GetSmiteDamage(), true) + GetSmiteDamage())
            {
                SpellsManager.Q2.Cast();
                ForceSmite = true;
                return;
            }
            if (SpellsManager.Q.IsReady() && SmiteMenu["QSmite"].Cast<CheckBox>().CurrentValue && SpellsManager.Q.Name == Extensions.Spellss["Q1"] && minion.IsValidTarget(SpellsManager.Q.Range) &&
                minion.Health <=
                SpellsManager.QDamage(minion) + SpellsManager.Q2Damage(minion, SpellsManager.Q2Damage(minion) + GetSmiteDamage(), true) +
                GetSmiteDamage())
            {
                SpellsManager.Q.Cast(minion);
            }
        }
    }
}

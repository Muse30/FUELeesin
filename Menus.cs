using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Mario_s_Lib;
using Mario_s_Lib.DataBases;

namespace FUELeesin
{
    internal class Menus
    {
        public const string ComboMenuID = "combomenuid";
        public const string HarassMenuID = "harassmenuid";
        public const string LaneClearMenuID = "laneclearmenuid";
        public const string JungleClearMenuID = "jungleclearmenuid";
        public const string InsecMenuID = "insecmenuid";
        public const string WardJumpMenuID = "wardjumpmenuid";
        public const string DrawingsMenuID = "drawingsmenuid";
        public const string MiscMenuID = "miscmenuid";
        public static Menu FirstMenu;
        public static Menu ComboMenu;
        public static Menu HarassMenu;
        public static Menu InsecMenu;
        public static Menu LaneClearMenu;
        public static Menu JungleClearMenu;
        public static Menu WardJumpMenu;
        public static Menu DrawingsMenu;
        public static Menu MiscMenu;

        //These colorslider are from Mario`s Lib
        public static ColorSlide QColorSlide;
        public static ColorSlide WColorSlide;
        public static ColorSlide EColorSlide;
        public static ColorSlide RColorSlide;
        public static ColorSlide DamageIndicatorColorSlide;

        public static void CreateMenu()
        {
            FirstMenu = MainMenu.AddMenu("F U E " + Player.Instance.ChampionName, Player.Instance.ChampionName.ToLower() + "hue");
            ComboMenu = FirstMenu.AddSubMenu("• Combo", ComboMenuID);
            HarassMenu = FirstMenu.AddSubMenu("• Harass", HarassMenuID);
            LaneClearMenu = FirstMenu.AddSubMenu("• LaneClear", LaneClearMenuID);
            InsecMenu = FirstMenu.AddSubMenu("• Insec", InsecMenuID);
            JungleClearMenu = FirstMenu.AddSubMenu("• JungleClear", JungleClearMenuID);
            WardJumpMenu = FirstMenu.AddSubMenu("• WardJump", WardJumpMenuID);
            MiscMenu = FirstMenu.AddSubMenu("• Misc", MiscMenuID);
            DrawingsMenu = FirstMenu.AddSubMenu("• Drawings", DrawingsMenuID);

            ComboMenu.AddGroupLabel("Spells");
            ComboMenu.CreateCheckBox(" - Use Q", "qUse");
            ComboMenu.CreateCheckBox(" - Use Q2", "q2Use");
            ComboMenu.CreateCheckBox(" - Use W", "wUse");
            ComboMenu.CreateCheckBox(" - Use E", "eUse");
            ComboMenu.CreateCheckBox(" - Use R", "rUse");
            ComboMenu.CreateSlider("Min [{0}] stacks on Passive", "passivestacks", 1,1,2);
            ComboMenu.CreateCheckBox(" - Wardjump in combo", "wardjumpC");
            ComboMenu.CreateCheckBox(" - Out of AA range - Wardjump", "wardjumpCAA");
            ComboMenu.CreateCheckBox(" - Use R Ks", "rUseks");
            ComboMenu.CreateCheckBox(" - Use Q1 Ks", "q1Useks");
            ComboMenu.CreateCheckBox(" - Use Q2 Ks", "q2Useks");
            ComboMenu.CreateCheckBox(" - Use E Ks", "eUseks");
            ComboMenu.CreateCheckBox(" - Use Passive?", "Cpassive", false);
            ComboMenu.Add("starCombo", new KeyBind("Use Star Combo", false, KeyBind.BindTypes.HoldActive, 'Y'));
            ComboMenu.CreateCheckBox(" - Kick multiple targets:", "rkick");
            ComboMenu.CreateSlider("Min [{0}] targets to use R", "rkickcount", 2, 2, 4);
            ComboMenu.CreateCheckBox(" - Kick to kill enemy behind:", "rkickkill");



            HarassMenu.AddGroupLabel("Spells");
            HarassMenu.CreateCheckBox(" - Use Q", "q1Use");
            HarassMenu.CreateCheckBox(" - Use W", "wUse");
            HarassMenu.CreateCheckBox(" - Use E", "eeUse");
            HarassMenu.AddGroupLabel("Settings");
            HarassMenu.CreateSlider("Min [{0}] Passive Stacks", "passivestacksH", 30);


            LaneClearMenu.AddGroupLabel("Spells");
            LaneClearMenu.CreateCheckBox(" - Use Q", "qUse");
            LaneClearMenu.CreateCheckBox(" - Use E", "eUse");


            JungleClearMenu.AddGroupLabel("Spells");
            JungleClearMenu.CreateCheckBox(" - Use Q", "qUse");
            JungleClearMenu.CreateCheckBox(" - Use W", "wUse");
            JungleClearMenu.CreateCheckBox(" - Use E", "eUse");      
            JungleClearMenu.AddGroupLabel("Settings");
            JungleClearMenu.CreateCheckBox("Use Passive", "Jpassive", true);

            WardJumpMenu.AddGroupLabel("Spells");
            WardJumpMenu.Add("wardjump", new KeyBind("Wardjump :", false, KeyBind.BindTypes.HoldActive, 'G'));
            WardJumpMenu.CreateCheckBox(" - Jump to mouse", "mousej");
            WardJumpMenu.CreateCheckBox(" - Jump to minions", "minionsj");
            WardJumpMenu.CreateCheckBox(" - Jump to champions", "champsj");
            WardJumpMenu.CreateCheckBox(" - Ward jump on max range", "wjmaxrange", false);


            InsecMenu.AddGroupLabel("Spells");
            InsecMenu.Add("insec", new KeyBind("Insec :", false, KeyBind.BindTypes.HoldActive, 'T'));
            InsecMenu.Add("insecflash", new KeyBind("Flash + R :", false, KeyBind.BindTypes.HoldActive, 'A'));
            InsecMenu.CreateCheckBox(" - Orbwalk?", "insecorbw");
            InsecMenu.CreateCheckBox(" - Insec to allies?", "insecally");
            InsecMenu.CreateCheckBox(" - Insec to original pos?", "insecpos");
            InsecMenu.CreateCheckBox(" - Flash Insec when no ward?", "insecnoward", false);
            InsecMenu.CreateCheckBox(" - Use minions [Q]", "inseccheck");
            InsecMenu.CreateCheckBox(" - Left click target to Insec", "insecMode");
            InsecMenu.CreateCheckBox(" - Click Insec", "clickInsec");
            InsecMenu.AddLabel("Click anywhere and you will see a blue circle, then click the enemy and it will insec to that position");
            InsecMenu.CreateSlider("Ally Bonus Range", "bonusrangea", 0, 0, 1000);
            InsecMenu.CreateSlider("Tower Bonus Range", "bonusranget", 0, 0, 1000);



            MiscMenu.AddGroupLabel("Skin Changer");

            var skinList = Skins.SkinsDB.FirstOrDefault(list => list.Champ == Player.Instance.Hero);
            if (skinList != null)
            {
                MiscMenu.CreateComboBox("Choose the skin", "skinComboBox", skinList.Skins);
                MiscMenu.Get<ComboBox>("skinComboBox").OnValueChange +=
                    delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args) { Player.Instance.SetSkinId(sender.CurrentValue); };
                Player.Instance.SetSkinId(MiscMenu.Get<ComboBox>("skinComboBox").CurrentValue);
            }

            MiscMenu.AddGroupLabel("Auto Level UP");
            MiscMenu.CreateCheckBox("Activate Auto Leveler", "activateAutoLVL", false);
            MiscMenu.AddLabel("The auto leveler will always focus R than the rest of the spells");
            MiscMenu.CreateComboBox("1st Spell to focus", "firstFocus", new List<string> {"Q", "W", "E"});
            MiscMenu.CreateComboBox("2nd Spell to focus", "secondFocus", new List<string> {"Q", "W", "E"}, 1);
            MiscMenu.CreateComboBox("3rd Spell to focus", "thirdFocus", new List<string> {"Q", "W", "E"}, 2);
            MiscMenu.CreateSlider("Delay slider", "delaySlider", 200, 150, 500);
            MiscMenu.CreateCheckBox(" - Smite", "smiteks");
            MiscMenu.CreateCheckBox(" - Ignite", "igniteks");
            MiscMenu.CreateCheckBox(" - Smite Q", "smiteq", true);


            DrawingsMenu.AddGroupLabel("Setting");
            DrawingsMenu.CreateCheckBox(" - Draw Spell`s range only if they are ready.", "readyDraw");
            DrawingsMenu.CreateCheckBox(" - Draw damage indicator.", "damageDraw");
            DrawingsMenu.CreateCheckBox(" - Draw damage indicator percent.", "perDraw");
            DrawingsMenu.CreateCheckBox(" - Draw damage indicator statistics.", "statDraw", false);
            DrawingsMenu.AddGroupLabel("Spells");
            DrawingsMenu.CreateCheckBox(" - Draw Enabled.", "drawsenabled");
            DrawingsMenu.CreateCheckBox(" - Draw Insec lines.", "drawinseclines");
            DrawingsMenu.CreateCheckBox(" - Draw Insec text.", "drawinsectext");
            DrawingsMenu.CreateCheckBox(" - Draw Outline.", "drawoutline");
            DrawingsMenu.CreateCheckBox(" - Draw WardJump.", "drawwardjump");
            DrawingsMenu.CreateCheckBox(" - Draw Insec.", "drawinsec");
            DrawingsMenu.CreateCheckBox(" - Draw Q.", "qDraw");
            DrawingsMenu.CreateCheckBox(" - Draw W.", "wDraw");
            DrawingsMenu.CreateCheckBox(" - Draw E.", "eDraw");
            DrawingsMenu.CreateCheckBox(" - Draw R.", "rDraw");

       

            DrawingsMenu.AddGroupLabel("Drawings Color");
            QColorSlide = new ColorSlide(DrawingsMenu, "qColor", Color.Red, "Q Color:");
            WColorSlide = new ColorSlide(DrawingsMenu, "wColor", Color.Purple, "W Color:");
            EColorSlide = new ColorSlide(DrawingsMenu, "eColor", Color.Orange, "E Color:");
            RColorSlide = new ColorSlide(DrawingsMenu, "rColor", Color.DeepPink, "R Color:");
            DamageIndicatorColorSlide = new ColorSlide(DrawingsMenu, "healthColor", Color.YellowGreen, "DamageIndicator Color:");
        }
    }
}
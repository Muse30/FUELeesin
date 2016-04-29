using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Mario_s_Lib;
using static FUELeesin.Menus;

namespace FUELeesin
{
    public static class SpellsManager
    {
        /*
        Targeted spells are like Katarina`s Q
        Active spells are like Katarina`s W
        Skillshots are like Ezreal`s Q
        Circular Skillshots are like Lux`s E and Tristana`s W
        Cone Skillshots are like Annie`s W and ChoGath`s W
        */

        //Remenber of putting the correct type of the spell here
        public static Spell.Skillshot Q;
        public static Spell.Active Q2;
        public static Spell.Active W2;
        public static Spell.Targeted W;
        public static Spell.Active E;
        public static Spell.Targeted R;

        public static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        public static Spell.Skillshot Flash { get; set; }

        public static Spell.Targeted Smite { get; set; }

        public static Spell.Targeted Ignite { get; set; }

        public static SpellSlot igniteSlot;

        public static List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        /// <summary>
        /// It sets the values to the spells
        /// </summary>
        public static void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1020, SkillShotType.Linear, 250, 1800, 50);
            Q2 = new Spell.Active(SpellSlot.Q, 1300);
            W = new Spell.Targeted(SpellSlot.W, 700);
            W2 = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, 330);
            R = new Spell.Targeted(SpellSlot.R, 375);




            var smite = Player.Instance.GetSpellSlotFromName("summonersmite");

            if (smite != SpellSlot.Unknown)
            {
                Smite = new Spell.Targeted(smite, 500);
            }

            var slot = Player.Instance.GetSpellSlotFromName("summonerflash");

            if (slot != SpellSlot.Unknown)
            {
                Flash = new Spell.Skillshot(slot, 680, SkillShotType.Linear);
            }

            var ign = Player.Instance.GetSpellSlotFromName("summonerdot");

            if (ign != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(ign, 600);
            }

            SpellList.Add(Q);
            SpellList.Add(Q2);
            SpellList.Add(W);
            SpellList.Add(W2);
            SpellList.Add(E);
            SpellList.Add(R);

            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
        }

        #region Damages

        /// <summary>
        /// It will return the damage but you need to set them before getting the damage
        /// </summary>
        /// <param name="target"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// 

        public static float Q2Damage(Obj_AI_Base target, float subHP = 0, bool monster = false)
        {
            var damage = (50 + (Q.Level * 30)) + (0.09 * myHero.FlatPhysicalDamageMod)
                         + ((target.MaxHealth - (target.Health - subHP)) * 0.08);

            if (monster && damage > 400)
            {
                return (float)myHero.CalculateDamageOnUnit(target, DamageType.Physical, 400);
            }

            return (float)myHero.CalculateDamageOnUnit(target, DamageType.Physical, (float)damage);
        }

        public static double QDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.Q).IsLearned) return 0;
            return myHero.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new double[] { 50, 80, 110, 140, 170 }[Q.Level - 1] + 0.9 * myHero.FlatPhysicalDamageMod));
        }

        public static double EDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.E).IsLearned) return 0;
            return myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)new double[] { 60, 95, 130, 165, 200 }[E.Level - 1] + 1 * myHero.FlatPhysicalDamageMod);
        }

        public static float GetDamage(this Obj_AI_Base target, SpellSlot slot)
        {
            const DamageType damageType = DamageType.Magical;
            var AD = Player.Instance.FlatPhysicalDamageMod;
            var AP = Player.Instance.FlatMagicDamageMod;
            var sLevel = Player.GetSpell(slot).Level - 1;

            //You can get the damage information easily on wikia

            var dmg = 0f;

            switch (slot)
            {
                case SpellSlot.Q:
                    if (Q.IsReady())
                    {
                        //Information of Q damage
                        dmg += new float[] { 15, 40, 65, 90, 115 }[sLevel] + new[] { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f }[sLevel] * AD;
                    }
                    break;
                case SpellSlot.W:
                    if (W.IsReady())
                    {
                        //Information of W damage
                        dmg += new float[] { 0, 0, 0, 0, 0 }[sLevel] + 1f * AD;
                    }
                    break;
                case SpellSlot.E:
                    if (E.IsReady())
                    {
                        //Information of E damage
                        dmg += new float[] { 0, 0, 0, 0, 0 }[sLevel];
                    }
                    break;
                case SpellSlot.R:
                    if (R.IsReady())
                    {
                        //Information of R damage
                        dmg += new float[] { 20, 60, 95, 130, 165 }[sLevel] + 0.45f * AP;
                    }
                    break;
            }
            return Player.Instance.CalculateDamageOnUnit(target, damageType, dmg - 10);
        }

        #endregion Damages

        /// <summary>
        /// This event is triggered when a unit levels up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (MiscMenu.GetCheckBoxValue("activateAutoLVL") && sender.IsMe)
            {
                var delay = MiscMenu.GetSliderValue("delaySlider");
                Core.DelayAction(LevelUPSpells, delay);

            }
        }

        /// <summary>
        /// It will level up the spell using the values of the comboboxes on the menu as a priority
        /// </summary>
        private static void LevelUPSpells()
        {
            if (Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.R))
            {
                Player.Instance.Spellbook.LevelSpell(SpellSlot.R);
            }

            var firstFocusSlot = GetSlotFromComboBox(MiscMenu.GetComboBoxValue("firstFocus"));
            var secondFocusSlot = GetSlotFromComboBox(MiscMenu.GetComboBoxValue("secondFocus"));
            var thirdFocusSlot = GetSlotFromComboBox(MiscMenu.GetComboBoxValue("thirdFocus"));

            var secondSpell = Player.GetSpell(secondFocusSlot);
            var thirdSpell = Player.GetSpell(thirdFocusSlot);

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(firstFocusSlot))
            {
                if (!secondSpell.IsLearned)
                {
                    Player.Instance.Spellbook.LevelSpell(secondFocusSlot);
                }
                if (!thirdSpell.IsLearned)
                {
                    Player.Instance.Spellbook.LevelSpell(thirdFocusSlot);
                }
                Player.Instance.Spellbook.LevelSpell(firstFocusSlot);
            }

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(secondFocusSlot))
            {
                if (!thirdSpell.IsLearned)
                {
                    Player.Instance.Spellbook.LevelSpell(thirdFocusSlot);
                }
                Player.Instance.Spellbook.LevelSpell(firstFocusSlot);
                Player.Instance.Spellbook.LevelSpell(secondFocusSlot);
            }

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(thirdFocusSlot))
            {
                Player.Instance.Spellbook.LevelSpell(thirdFocusSlot);
            }
        }

        /// <summary>
        /// It will transform the value of the combobox into a SpellSlot
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static SpellSlot GetSlotFromComboBox(this int value)
        {
            switch (value)
            {
                case 0:
                    return SpellSlot.Q;
                case 1:
                    return SpellSlot.W;
                case 2:
                    return SpellSlot.E;
            }
            Chat.Print("Failed getting slot");
            return SpellSlot.Unknown;
        }

        public static bool DoDynamicKillSteal(List<Spell.SpellBase> spells)
        {
            var target =
                EntityManager.Heroes.Enemies.OrderBy(e => e.Health)
                    .ThenByDescending(TargetSelector.GetPriority)
                    .ThenBy(e => e.FlatArmorMod)
                    .ThenBy(e => e.FlatMagicReduction)
                    .FirstOrDefault(e => e.IsValidTarget(spells.GetSmallestRange()) && !e.HasUndyingBuff());

            if (target != null)
            {
                var dmg = spells.Where(spell => spell.IsReady()).Sum(spell => target.GetDamage(spell.Slot));
                var delay = spells.Sum(s => s.CastDelay);
                var targetPredictedHealth = Prediction.Health.GetPrediction(target, delay);

                if (targetPredictedHealth <= dmg)
                {
                    foreach (var spell in spells.Where(s => target.CanCastSpell(s)))
                    {
                        try
                        {
                            spell.Cast();
                        }
                        catch (Exception)
                        {
                            spell.Cast(target);
                        }
                    }
                }
            }
            return false;
        }
    }
}
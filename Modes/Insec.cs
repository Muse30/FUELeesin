using EloBuddy.SDK;
using Mario_s_Lib;
using System;
using static FUELeesin.Menus;
using static FUELeesin.SpellsManager;
using static FUELeesin.Extensions;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Enumerations;

namespace FUELeesin.Modes
{
    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class Insec
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void InsecCombo(AIHeroClient target)
        {

                if (target != null && target.IsVisible)
                {
                    if (myHero.Distance(GetInsecPos(target)) < 200)
                    {
                        insecComboStep = InsecComboStepSelect.Pressr;
                    }
                    else if (insecComboStep == InsecComboStepSelect.None
                             && GetInsecPos(target).Distance(myHero.Position) < 600)
                    {
                        insecComboStep = InsecComboStepSelect.Wgapclose;
                    }
                    else if (insecComboStep == InsecComboStepSelect.None
                             && target.Distance(myHero) < Q.Range)
                    {
                        insecComboStep = InsecComboStepSelect.Qgapclose;
                    }

                    switch (insecComboStep)
                    {
                    
                        case InsecComboStepSelect.Qgapclose:

                            var prediction = Q.GetPrediction(target);
                            if (prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) >= 1 && InsecMenu.GetCheckBoxValue("inseccheck") && Q.IsReady())
                            {
                                foreach (var unit in ObjectManager.Get<Obj_AI_Base>().Where(obj => (((obj is Obj_AI_Minion) && myHero.GetSpellDamage(target, SpellSlot.Q) < obj.Health + 10) || (obj is AIHeroClient)) && obj.IsValidTarget(Q.Range) && obj.Distance(GetInsecPos(target)) < 500))
                                {
                                    var pred = Q.GetPrediction(unit);
                                    if (pred.HitChance >= HitChance.High)
                                    {
                                        Q.Cast(pred.CastPosition);
                                    }
                                    break;
                                }
                            }

                            if (!(target.HasQBuff()) && QState)
                            {
                                CastQ(target, MiscMenu.GetCheckBoxValue("smiteq"));
                            }
                            else if (target.HasQBuff())
                            {
                                Q2.Cast();
                                insecComboStep = InsecComboStepSelect.Wgapclose;
                            }
                            else
                            {
                                if (Q.Name == "blindmonkqtwo" && ReturnQBuff().Distance(target) <= 600 && target.HasQBuff())
                                {
                                    Q2.Cast();
                                }
                            }
                            break;

                        case InsecComboStepSelect.Wgapclose:

                            if (myHero.Distance(target) < WardRange)
                            {
                            WardJumper.WardJump(GetInsecPos(target), false, true, true);


                                if (FindBestWardItem() == null && R.IsReady()
                                    && InsecMenu.GetCheckBoxValue("insecnoward")
                                    && Flash.IsReady())
                                {
                                    if ((GetInsecPos(target).Distance(myHero.Position) < FlashRange
                                         && LastWard + 1000 < Environment.TickCount) || !W.IsReady())
                                    {
                                        Flash.Cast(GetInsecPos(target));
                                    }
                                }
                            }
                            else if (myHero.Distance(target) < WardFlashRange)
                            {
                            WardJumper.WardJump(target.Position);

                                if (R.IsReady() && InsecMenu.GetCheckBoxValue("insecnoward")
                                    && Flash.IsReady())
                                {
                                    if (myHero.Distance(target) < FlashRange - 25)
                                    {
                                        if (FindBestWardItem() == null || LastWard + 1000 < Environment.TickCount)
                                        {
                                            Flash.Cast(GetInsecPos(target));
                                        }
                                    }
                                }
                            }
                            break;

                        case InsecComboStepSelect.Pressr:
                            R.Cast(target);
                            break;
                    }
                }
            }

        }
    }
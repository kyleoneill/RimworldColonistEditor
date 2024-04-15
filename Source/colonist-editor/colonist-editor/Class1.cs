using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace colonist_editor
{
    [StaticConstructorOnStartup]
    public class Building_SkillShrine : Building
    {
        public FloatMenuOption skillShrineMenuOption(
            Building_SkillShrine shrine,
            Pawn pawn)
        {
            string label = "Use skill shrine";
            return new FloatMenuOption(label, (Action)(() => shrine.UseAct(pawn)));
        }
        
        private void UseAct(Pawn myPawn)
        {
            if (myPawn.IsColonistPlayerControlled)
            {
                Trait sanguineTrait = new Trait(TraitDef.Named("NaturalMood"), 2);
                Trait quickSleeperTrait = new Trait(TraitDef.Named("QuickSleeper"));
                List<Trait> traitsToAdd = new List<Trait>() {sanguineTrait, quickSleeperTrait};

                Trait wimpTrait = new Trait(TraitDef.Named("Wimp"));
                List<Trait> traitsToRemove = new List<Trait>() { wimpTrait };
                
                Map map = myPawn.Map;
                var playerPawns = map.mapPawns.FreeColonists;
                foreach (Pawn pawn in playerPawns)
                {
                    if (pawn.IsColonistPlayerControlled)
                    {
                        List<SkillRecord> skillRecords = pawn.skills.skills;
                        foreach(var record in skillRecords)
                        {
                            record.passion = Passion.Major;
                            record.Learn(300000, true, true);
                        }

                        var pawnTraits = pawn.story.traits;

                        // Remove traits we don't want
                        foreach (var traitToRemove in traitsToRemove)
                        {
                            if (pawnTraits.HasTrait(traitToRemove.def))
                            {
                                pawnTraits.RemoveTrait(traitToRemove);
                            }
                        }

                        // Add traits we do want
                        foreach (var trait in traitsToAdd)
                        {
                            // If we already have a desired trait, but with the wrong degree, we want to remove it
                            if (pawnTraits.HasTrait(trait.def) && !pawnTraits.HasTrait(trait.def, trait.Degree))
                            {
                                pawnTraits.RemoveTrait(trait);
                            }

                            // Add the traits we want
                            if (!pawnTraits.HasTrait(trait.def))
                            {
                                pawnTraits.GainTrait(trait);
                            }
                        }

                        // If any of our pawns have a story that disables a work type, replace it with some story that has nothing disabled
                        if (pawn.story.Childhood != null && pawn.story.Childhood.DisabledWorkTypes.Count > 0)
                        {
                            pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("FarmKid60");
                        }
                        if (pawn.story.Adulthood != null && pawn.story.Adulthood.DisabledWorkTypes.Count > 0)
                        {
                            pawn.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("CivilEngineer2");
                        }
                    }
                }
            }
        }
        
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            Building_SkillShrine shrine = this;
            yield return skillShrineMenuOption(shrine, myPawn);
        }
    }
}

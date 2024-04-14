using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using System;
using System.Collections.Generic;

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
                TraitDef naturalMoodTraitDef = TraitDef.Named("NaturalMood");
                Trait sanguineTrait = new Trait(naturalMoodTraitDef, 2);
                TraitDef quickSleeperTraitDef = TraitDef.Named("QuickSleeper");
                Trait quickSleeperTrait = new Trait(quickSleeperTraitDef);
                List<Trait> traitsToAdd = new List<Trait>() {sanguineTrait, quickSleeperTrait};
                
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
                        foreach (var trait in traitsToAdd)
                        {
                            if (!pawnTraits.HasTrait(trait.def))
                            {
                                pawnTraits.GainTrait(trait);
                            }
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

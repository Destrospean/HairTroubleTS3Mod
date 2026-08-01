using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;

namespace Destrospean.HairTrouble
{
    public static class OutfitExtensions
    {
        public delegate SimOutfit OutfitFunc(SimBuilder simBuilder, OutfitCategories outfitCategory, int outfitIndex);

        public static void ApplyToAllOutfits(this SimDescription simDescription, OutfitFunc outfitFunc, bool spin = false)
        {
            using (SimBuilder simBuilder = new SimBuilder
                {
                    UseCompression = true
                })
            {
                ApplyToAllOutfits(simDescription, simBuilder, outfitFunc, spin);
            }
        }

        public static void ApplyToAllOutfits(this SimDescription simDescription, SimBuilder simBuilder, OutfitFunc outfitFunc, bool spin = false)
        {
            OutfitCategories lastOutfitCategory = 0,
            tempOutfitCategory = OutfitCategories.Everyday;
            int lastOutfitIndex = 0,
            tempOutfitIndex = simDescription.GetOutfitCount(tempOutfitCategory);
            if (simDescription.CreatedSim != null)
            {
                lastOutfitCategory = simDescription.CreatedSim.CurrentOutfitCategory;
                lastOutfitIndex = simDescription.CreatedSim.CurrentOutfitIndex;
                if (spin)
                {
                    simDescription.AddOutfit(new SimOutfit(simDescription.CreatedSim.CurrentOutfit.Key), tempOutfitCategory);
                    simDescription.CreatedSim.SwitchToOutfitWithoutSpin(tempOutfitCategory, tempOutfitIndex);
                    SimOutfit outfit = outfitFunc(simBuilder, lastOutfitCategory, lastOutfitIndex);
                    if (outfit.IsValid)
                    {
                        simDescription.ReplaceOutfit(lastOutfitCategory, lastOutfitIndex, outfit);
                        using (Sims3.Gameplay.Actors.Sim.SwitchOutfitHelper switchOutfitHelper = new Sims3.Gameplay.Actors.Sim.SwitchOutfitHelper(simDescription.CreatedSim, Sims3.Gameplay.Actors.Sim.ClothesChangeReason.Force, lastOutfitCategory, lastOutfitIndex, false))
                        {
                            simDescription.CreatedSim.SwitchToOutfitWithSpin(switchOutfitHelper);
                        }
                        simDescription.RemoveOutfit(tempOutfitCategory, tempOutfitIndex, true);
                    }
                }
            }
            foreach (OutfitCategories outfitCategory in simDescription.ListOfCategories)
            {
                System.Collections.ArrayList outfits = simDescription.Outfits[outfitCategory] as System.Collections.ArrayList;
                if (outfits == null)
                {
                    continue;
                }
                for (int i = outfits.Count - 1; i > -1; i--)
                {
                    if (simDescription.CreatedSim == null || outfitCategory != lastOutfitCategory || i != lastOutfitIndex || !spin)
                    {
                        simDescription.ReplaceOutfit(outfitCategory, i, outfitFunc(simBuilder, outfitCategory, i));
                    }
                }
            }
            if (simDescription.CreatedSim != null && !spin)
            {
                simDescription.CreatedSim.UpdateOutfitInfo();
                simDescription.CreatedSim.RefreshCurrentOutfit(false);
            }
        }

        public static void PrepareForOutfit(this SimBuilder simBuilder, SimOutfit outfit)
        {
            simBuilder.Clear();
            OutfitUtils.SetAutomaticModifiers(simBuilder);
            OutfitUtils.SetOutfit(simBuilder, outfit, null);
        }

        public static void ReplaceOutfit(this SimDescription simDescription, OutfitCategories outfitCategory, int outfitIndex, SimOutfit newOutfit)
        {
            if (newOutfit != null && newOutfit.IsValid)
            {
                simDescription.RemoveOutfitInternal(outfitCategory, outfitIndex, true, simDescription.IsUsingMaternityOutfits);
                simDescription.AddOutfitInternal(newOutfit, outfitCategory, outfitIndex, simDescription.IsUsingMaternityOutfits, false);
            }
        }
    }
}

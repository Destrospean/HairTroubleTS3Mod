using System.Collections.Generic;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;

namespace Destrospean.HairTrouble
{
    public static class Replacements
    {
        public static Color[] InheritHairColor(Sims3.SimIFace.CAS.SimBuilder target, SimDescription[] parentSims, System.Random random)
        {
            List<Genealogy> genealogies = new List<Genealogy>();
            foreach (SimDescription parentSim in parentSims)
            {
                if (parentSim.Genealogy != null)
                {
                    genealogies.Add(parentSim.Genealogy);
                }
            }
            if (genealogies.Count == 0)
            {
                return Genetics.GeneticColors[random.Next(Genetics.GeneticColors.Length)] as Color[];
            }
            if (genealogies.Count == 1)
            {
                genealogies.Add(genealogies[0]);
            }
            List<Genealogy> parentGenealogies = new List<Genealogy>();
            foreach (Genealogy genealogy in genealogies)
            {
                foreach (Genealogy parent in genealogy.Parents)
                {
                    if (parent.SimDescription != null)
                    {
                        parentGenealogies.Add(parent);
                    }
                }
            }
            if ((float)random.NextDouble() * 100 < Genetics.kMutateHairColorChance)
            {
                return Genetics.GeneticColors[random.Next(Genetics.GeneticColors.Length)] as Color[];
            }
            SimDescription simDescription;
            if (parentGenealogies.Count > 0 && (float)random.NextDouble() * 100 < Genetics.kHairColorChooseGrandparentChance)
            {
                simDescription = parentGenealogies[random.Next(parentGenealogies.Count)].SimDescription;
            }
            else
            {
                simDescription = genealogies[random.Next(genealogies.Count)].SimDescription;
            }
            Color[] colors = new Color[10];
            for (int i = 0; i < 4; i++)
            {
                colors[i] = simDescription.GetOriginalHairColors()[i].Genetic;
            }
            colors[4] = simDescription.GetOriginalEyebrowColor().Genetic;
            for (int i = 0; i < 4; i++)
            {
                colors[i + 5] = simDescription.GetOriginalFacialHairColors()[i].Genetic;
            }
            colors[9] = simDescription.GetOriginalBodyHairColor().Genetic;
            return colors;
        }
    }
}

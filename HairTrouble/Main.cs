using System;
using Destrospean.PreserveGeneticHair;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.SimIFace;

namespace Destrospean.HairTrouble
{
    public class Main
    {
        [Tunable]
        protected static bool kInstantiator = false;

        static Main()
        {
            Common.ReplaceMethod(typeof(Sims3.Gameplay.CAS.Genetics).GetMethod("InheritHairColor"), typeof(Replacements).GetMethod("InheritHairColor"));
            EventListener simDescriptionDisposedListener = null,
            simInstantiatedListener = null,
            simSelectedListener = null;
            World.sOnObjectPlacedInLotEventHandler += (sender, e) =>
                {
                    World.OnObjectPlacedInLotEventArgs onObjectPlacedInLotEventArgs = e as World.OnObjectPlacedInLotEventArgs;
                    if (onObjectPlacedInLotEventArgs != null)
                    {
                        AddInteractions(Sims3.Gameplay.Abstracts.GameObject.GetObject(onObjectPlacedInLotEventArgs.ObjectId) as Mirror);
                    }
                };
            World.sOnWorldLoadFinishedEventHandler += (sender, e) =>
                {
                    SimHairGrowth.InitHairGrowthStateMap();
                    foreach (Mirror mirror in Sims3.Gameplay.Queries.GetObjects<Mirror>())
                    {
                        AddInteractions(mirror);
                    }
                    foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
                    {
                        AddInteractions(sim);
                    }
                    if (SimHairData.OriginalHairColors.Count == 0 && Sims3.Gameplay.CAS.Household.ActiveHousehold != null)
                    {
                        foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
                        {
                            sim.SimDescription.InitOriginalOverallHairColors();
                        }
                    }
                    simDescriptionDisposedListener = EventTracker.AddListener(EventTypeId.kSimDescriptionDisposed, evt =>
                        {
                            try
                            {
                                Sim sim = evt.TargetObject as Sim;
                                if (sim != null)
                                {
                                    sim.SimDescription.ClearOriginalOverallHairColors();
                                }
                            }
                            catch (Exception ex)
                            {
                                ((IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(null, ex);
                            }
                            return ListenerAction.Keep;
                        });
                    simInstantiatedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, evt =>
                        {
                            try
                            {
                                Sim sim = evt.TargetObject as Sim;
                                if (sim != null)
                                {
                                    AddInteractions(sim);
                                    sim.SimDescription.InitOriginalOverallHairColors();
                                }
                            }
                            catch (Exception ex)
                            {
                                ((IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(null, ex);
                            }
                            return ListenerAction.Keep;
                        });
                    simSelectedListener = EventTracker.AddListener(EventTypeId.kEventSimSelected, evt =>
                        {
                            try
                            {
                                if (SimHairData.OriginalHairColors.Count == 0)
                                {
                                    foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
                                    {
                                        sim.SimDescription.InitOriginalOverallHairColors();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ((IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(null, ex);
                            }
                            return ListenerAction.Keep;
                        });
                };
            World.sOnWorldQuitEventHandler += (sender, e) =>
                {
                    EventTracker.RemoveListener(simDescriptionDisposedListener);
                    EventTracker.RemoveListener(simInstantiatedListener);
                    EventTracker.RemoveListener(simSelectedListener);
                    simDescriptionDisposedListener = null;
                    simInstantiatedListener = null;
                    simSelectedListener = null;
                };
        }

        static void AddInteractions(Mirror mirror)
        {
            if (mirror != null)
            {
                mirror.AddInteraction(Interactions.RemoveHairDye.Singleton, true);
            }
        }

        static void AddInteractions(Sim sim)
        {
            if (sim != null)
            {
                sim.AddInteraction(Interactions.DecrementHairGrowthState.Singleton, true);
                sim.AddInteraction(Interactions.IncrementHairGrowthState.Singleton, true);
                sim.AddInteraction(Interactions.ResetOriginalHair.Singleton, true);
            }
        }
    }
}

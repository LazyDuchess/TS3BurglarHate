using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazyDuchess.BurglarHate
{
    public class BurglarHateTask : Task
    {
        public static BurglarHateTask Singleton = null;
        [PersistableStatic]
        static Dictionary<ulong, List<ulong>> alreadyAppliedLTRBetweenBurglarAndVictims = new Dictionary<ulong, List<ulong>>();
        public static void Start()
        {
            Singleton = new BurglarHateTask();
            Simulator.AddObject(Singleton);
        }

        public static void Shutdown()
        {
            if (Singleton == null)
                return;
            Singleton.Stop();
            Singleton.Dispose();
            Simulator.DestroyObject(Singleton.ObjectId);
            Singleton = null;
        }

        void UpdateBurglars()
        {
            foreach (var burglar in alreadyAppliedLTRBetweenBurglarAndVictims)
            {
                var simDescriptionId = burglar.Key;
                var burglarDescription = SimDescription.Find(simDescriptionId);
                if (burglarDescription == null)
                {
                    alreadyAppliedLTRBetweenBurglarAndVictims.Remove(simDescriptionId);
                    continue;
                }
                var createdBurglar = burglarDescription.CreatedSim;
                if (createdBurglar == null)
                {
                    alreadyAppliedLTRBetweenBurglarAndVictims.Remove(simDescriptionId);
                    continue;
                }
                if (!(createdBurglar.Service is Burglar) && !(createdBurglar.Service is Repoman))
                {
                    alreadyAppliedLTRBetweenBurglarAndVictims.Remove(simDescriptionId);
                    continue;
                }
            }
        }

        void RunBurglarLogic(Sim burglar)
        {
            if (burglar == null)
                return;
            foreach (var sim in Household.ActiveHousehold.Sims)
            {
                var rel = Relationship.Get(sim, burglar, false);
                if (rel == null)
                    continue;
                var burglarInDictionary = alreadyAppliedLTRBetweenBurglarAndVictims.TryGetValue(burglar.SimDescription.SimDescriptionId, out List<ulong> simList);
                if (burglarInDictionary)
                {
                    if (simList.Contains(sim.SimDescription.SimDescriptionId))
                        continue;
                }
                if (!burglarInDictionary)
                    alreadyAppliedLTRBetweenBurglarAndVictims[burglar.SimDescription.SimDescriptionId] = new List<ulong>();
                alreadyAppliedLTRBetweenBurglarAndVictims[burglar.SimDescription.SimDescriptionId].Add(sim.SimDescription.SimDescriptionId);
                var vTraits = sim.TraitManager;
                if (vTraits.HasElement(TraitNames.Frugal) || vTraits.HasElement(TraitNames.HotHeaded) || vTraits.HasElement(TraitNames.CanApprehendBurglar))
                    rel.LTR.UpdateLiking(-80f);
                else
                    rel.LTR.UpdateLiking(-50f);
            }
        }

        public override void Simulate()
        {
            if (Household.ActiveHousehold == null)
                return;
            if (Household.ActiveHouseholdLot == null)
                return;
            UpdateBurglars();
            RunBurglarLogic(Burglar.sBurglar.GetSimActiveOnLot(Household.ActiveHouseholdLot));
            if (Main.kExtendsToRepomen)
                RunBurglarLogic(Repoman.sRepoman.GetSimActiveOnLot(Household.ActiveHouseholdLot));
        }
    }
}

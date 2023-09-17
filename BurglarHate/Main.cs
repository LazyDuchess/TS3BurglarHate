using System;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Controller;

namespace LazyDuchess.BurglarHate
{
	public class Main
	{
		[Tunable] static bool init;
		[Tunable] public static bool kExtendsToRepomen = true;
		
		static Main()
		{
			World.sOnWorldLoadFinishedEventHandler += OnWorldLoad;
			World.sOnWorldQuitEventHandler += OnWorldQuit;
		}

		static void OnWorldLoad(object sender, EventArgs e)
        {
			BurglarHateTask.Start();
		}

		static void OnWorldQuit(object sender, EventArgs e)
        {
			BurglarHateTask.Shutdown();
        }
	}
}
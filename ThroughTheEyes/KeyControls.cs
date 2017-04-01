using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace FirstPerson
{
    class KeyControls
    {

		public static void rescueAfterHatchCheck()
		{
			if (!FlightGlobals.ActiveVessel.isEVA)
			{
				List<ProtoCrewMember> crew = FlightGlobals.ActiveVessel.GetVesselCrew();
				for (int i = 0; i < crew.Count; i++)
				{
					ProtoCrewMember pcm = crew[i];
					if (pcm.KerbalRef.state == Kerbal.States.BAILED_OUT) { pcm.KerbalRef.state = Kerbal.States.ALIVE; CameraManager.Instance.SetCameraIVA(); }
				}
			}
		}


		public static void GoEVA()
		{
			List<ProtoCrewMember> crew = FlightGlobals.ActiveVessel.GetVesselCrew();
			for (int i = 0; i < crew.Count; i++)
			{
				ProtoCrewMember pcm = crew[i];
				if (pcm != null && HighLogic.CurrentGame.Parameters.Flight.CanEVA)
				{
					if (pcm.KerbalRef.eyeTransform == InternalCamera.Instance.transform.parent)
					{
						if (! (pcm.KerbalRef.state == Kerbal.States.ALIVE 
							&& !pcm.KerbalRef.InPart.packed 
							&& HighLogic.CurrentGame.Parameters.Flight.CanEVA 
							&& !pcm.KerbalRef.InPart.NoAutoEVA
							&& !pcm.inactive
							&& pcm.type != ProtoCrewMember.KerbalType.Tourist 
							&& (GameVariables.Instance.UnlockedEVA(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex)) 
								|| (FlightGlobals.ActiveVessel.mainBody == Planetarium.fetch.Home && FlightGlobals.ActiveVessel.LandedOrSplashed) )) ) {
							ScreenMessages.PostScreenMessage(GameVariables.Instance.GetEVALockedReason(FlightGlobals.ActiveVessel, pcm), 5);
							return;
						}
						FlightEVA.SpawnEVA(pcm.KerbalRef);
						CameraManager.Instance.SetCameraFlight();
						pcm.KerbalRef.state = Kerbal.States.BAILED_OUT;
						break;
					}
				}
			}

		}

        public static void MyReviewData()
        {
			for(int i = 0; i < FlightGlobals.ActiveVessel.Parts.Count; i++)
            {
				Part part = FlightGlobals.ActiveVessel.Parts[i];
				for (int m = 0; m < part.Modules.Count; m++)
                {
					PartModule module = part.Modules[m];
                    if (module.moduleName == "ModuleScienceExperiment")
                    {
                        module.SendMessage("ReviewData");
                    }

                }
            }

        }

        public static void CameraSwitch(CameraManager flightCam,Vessel pVessel)
        {
            if (flightCam.currentCameraMode == CameraManager.CameraMode.IVA && pVessel.GetCrewCount() > 1)
			{
                flightCam.NextCameraIVA();
            }
        }

		public static void recoverVessel(Vessel vessel) {
			if (vessel != null && vessel.IsRecoverable)
			{
				GameEvents.OnVesselRecoveryRequested.Fire(vessel);
			}
		}
    }
}
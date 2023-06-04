using System;
using System.Collections.Generic;

namespace SweetSugar.Scripts.Integrations.Network.EpsilonServer
{
	public interface IDataManagerEpsilon {
		
		void GetLevels(Action<ResultObject> Callback);
		
		void SetBoosterData (Dictionary<string, int> dic);

		void GetBoosterData (Action<ResultObject[]> Callback);
		void UpdateBoost(string name, int count);

		void Logout ();

		void SetLevels();
	}
}



#if EPSILON
using System;
using System.Collections.Generic;
using System.Linq;
using EpsilonServer.EpsilonClientAPI;
using SweetSugar.Scripts.Integrations.Network;
using UnityEngine;
using UnityEngine.Networking;

namespace EpsilonServer
{
    public class EpsilonCurrencyManager : ICurrencyManager
    {
        public void IncBalance(int amount)
        {
            BoostsUpdateRequest boostsUpdateRequest = new BoostsUpdateRequest().addBoost(new BoostsUpdateRequest.Boost("currency1", amount));
            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "boosts", boostsUpdateRequest.toJson(), (response) => {});
        }

        public void DecBalance(int amount)
        {
            BoostsUpdateRequest boostsUpdateRequest = new BoostsUpdateRequest().addBoost(new BoostsUpdateRequest.Boost("currency1", amount));
            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "boosts", boostsUpdateRequest.toJson(), (response) => {});
        }

        public void SetBalance(int newbalance)
        {
            BoostsUpdateRequest boostsUpdateRequest = new BoostsUpdateRequest().addBoost(new BoostsUpdateRequest.Boost("currency1", newbalance));
            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "boosts", boostsUpdateRequest.toJson(), (response) => { });
        }

        public void GetBalance(Action<int> Callback)
        {
            /*new EpsilonRequest().SetTable("boosts").SetAttribute("boostName", "currency1").Get(response =>
            {
                if (!response.isNetworkError && !response.downloadHandler.text.Contains("error") && !response.downloadHandler.text.Contains("Error"))
                {
                    var resultArray = JsonHelper.getJsonArray<ResultObject>(response.downloadHandler.text);
                    if (resultArray != null && resultArray.Length != 0)
                    {
                        Callback(resultArray.First().count);
                    }
                    else
                    {
                        Debug.Log("No currency data available");
                    }
                }
            });*/
            SelectApiRequest select = new SelectApiRequest("boosts").where("name", "currency1");

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "select", select.toJson(), (response) =>
            {
                if (response.result != UnityWebRequest.Result.ConnectionError && !response.downloadHandler.text.Contains("error") && !response.downloadHandler.text.Contains("Error"))
                {
                    var resultArray = JsonHelper.getJsonArray<List<List<Cell>>>(response.downloadHandler.text);
                    if (resultArray != null && resultArray.Length != 0)
                    {
                        Callback(resultArray.First().Count());
                    }
                    else
                    {
                        Debug.Log("No currency data available");
                    }
                }
            });
        }
    }
}
#endif
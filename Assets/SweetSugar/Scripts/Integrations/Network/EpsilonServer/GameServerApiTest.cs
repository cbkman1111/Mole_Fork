using System.Collections;
#if EPSILON
using EpsilonServer.EpsilonClientAPI;
using SweetSugar.Scripts.Integrations;
using SweetSugar.Scripts.Integrations.Network.EpsilonServer;
#endif
using UnityEngine;

namespace EpsilonServer
{
    public class GameServerApiTest : MonoBehaviour
    {
        #if EPSILON
        private EpsilonManager epsilonManager = new EpsilonManager();

        void Start()
        {
            Debug.Log("Login by facebook id:");
            FacebookManager.userID = "12345";
            epsilonManager.LoginWithFB("", "");

            StartCoroutine(TestAfter3Seconds());
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void testApiInsert() {
            Debug.Log("API Insert:");
            InsertApiRequest insertApiRequest = new InsertApiRequest("levels").addRow(1, 400, 3);
            string jsonInsert = insertApiRequest.toJson();
            Debug.Log(jsonInsert);
       
            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "insert", jsonInsert, (response) =>
            {
                Debug.Log("API Insert response: "+response.downloadHandler.text);
            });
        }

        private void testApiSelect() {
            Debug.Log("API Select:");

            SelectApiRequest selectApiRequest1 = new SelectApiRequest("levels");
            SelectApiRequest selectApiRequest2 = new SelectApiRequest("levels").where("level", 1).whereOr("level", 2);
            SelectApiRequest selectApiRequest3 = new SelectApiRequest("levels").where("score", 500).whereAnd("stars", 2);

            string jsonSelect1 = selectApiRequest1.toJson();
            string jsonSelect2 = selectApiRequest2.toJson();
            string jsonSelect3 = selectApiRequest3.toJson();

            Debug.Log(jsonSelect1);
            Debug.Log(jsonSelect2);
            Debug.Log(jsonSelect3);

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "select", jsonSelect1, (response) =>
            {
                Debug.Log("API Select1 response: "+response.downloadHandler.text);
            });

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "select", jsonSelect2, (response) =>
            {
                Debug.Log("API Select2 response: "+response.downloadHandler.text);
            });

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "select", jsonSelect3, (response) =>
            {
                Debug.Log("API Select3 response: "+response.downloadHandler.text);
            });
        }

        private void testApiUpdate() {
            Debug.Log("API Update:");

            UpdateApiRequest updateApiRequest = new UpdateApiRequest("levels").set("stars", 1).set("score", 1).where("level", 1);
            string jsonUpdate = updateApiRequest.toJson();
            Debug.Log(jsonUpdate);
        
            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("api", "update", jsonUpdate, (response) =>
            {
                Debug.Log("API Update response: "+response.downloadHandler.text);
            });
        }

        private void testGameLevelsUpdate() {
            Debug.Log("GAME Levels update:");

            LevelsUpdateRequest levelsUpdateRequest = new LevelsUpdateRequest();
            levelsUpdateRequest.addLevel(new LevelsUpdateRequest.Level(1, 100, 1));
            levelsUpdateRequest.addLevel(new LevelsUpdateRequest.Level(2, 200, 2));
            levelsUpdateRequest.addLevel(new LevelsUpdateRequest.Level(3, 300, 3));
            levelsUpdateRequest.addLevel(new LevelsUpdateRequest.Level(4, 400, 1));
            string jsonLevelsUpdate = levelsUpdateRequest.toJson();
            Debug.Log(jsonLevelsUpdate);

            EpsilonClientAPI.EpsilonClientAPI.THIS.SendApi("game", "levels", jsonLevelsUpdate, (response) =>
            {
                Debug.Log("GAME Levels update response: "+response.downloadHandler.text);
            });
        }

        IEnumerator TestAfter3Seconds()
        {
            yield return new WaitForSeconds(3);
            testApiInsert();
            testApiSelect();
            testApiUpdate();
            testGameLevelsUpdate();
        }
        #endif
    }
}

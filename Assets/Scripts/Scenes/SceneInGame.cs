
using System;
using Common.Global;
using Common.Scene;
using Common.Utils.Pool;
using System.Collections;
using UI.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scenes
{
    public class SceneInGame : SceneBase
    {
        public GameObject[] hazards;
        public Vector3 spawnValues;
        public int hazardCount;
        public float spawnWait;
        public float startWait;
        public float waveWait;

        public Camera cameraUI = null;
        public UIMenuInGame menu = null;
        private bool gameOver;
        private bool restart;


        public Transform[] pooled;
        public AudioClip[] soundList;

        public Joystick joystick = null;
        public Done_PlayerController playerController = null;

        public override bool Init(JSONObject param)
        {
            gameOver = false;
            restart = false;

            UpdateScore();
            StartCoroutine(SpawnWaves());

            SoundManager.Instance.InitList(transform, soundList);
            PoolManager.Instance.InitList(transform, pooled);
            
            SoundManager.Instance.PlayMusic("music_background");

            joystick.Init((Vector3 direct, float angle) => {
                //SetText("Text - Debug", $"Angle : {angle}");
                //move?.Invoke(direct);
                // ž�� �������� ��ȯ.
                direct.z = direct.y;
                playerController.Direction = direct;
            },
            () =>
            {
                playerController.Direction = Vector3.zero;
                //stop?.Invoke();
            });

            return true;
        }

        void Update()
        {
            if (restart)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }

        IEnumerator SpawnWaves()
        {
            yield return new WaitForSeconds(startWait);
            while (true)
            {
                for (int i = 0; i < hazardCount; i++)
                {
                    int rand = Random.Range(0, hazards.Length);
                    var hazard = PoolManager.Instance.GetObject(hazards[rand].name);
                    if(hazard != null)
                    {
                        Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                        Quaternion spawnRotation = Quaternion.identity;
                        hazard.position = spawnPosition;
                        hazard.rotation = spawnRotation;

                        var weapon = hazard.GetComponent<Done_WeaponController>();
                        if (weapon != null)
                        {
                            weapon.Init();
                        }

                        /*
                        var evasive = hazard.GetComponent<Done_EvasiveManeuver>();
                        if (evasive != null)
                        {
                            evasive.boundary.xMin = -10;
                            evasive.boundary.xMax = 10;
                            evasive.boundary.zMin = -35;
                            evasive.boundary.zMax = 20;
                            //evasive.boundary = new Done_Boundary();
                        }
                        */
                    }

                    yield return new WaitForSeconds(spawnWait);
                }

                yield return new WaitForSeconds(waveWait);
                if (gameOver)
                {
                    //restartText.text = "Press 'R' for Restart";
                    restart = true;
                    break;
                }
            }
        }

        public void AddScore(int newScoreValue)
        {
            //score += newScoreValue;
            UpdateScore();
        }

        void UpdateScore()
        {
            //scoreText.text = "Score: " + score;
        }

        public void GameOver()
        {

            gameOver = true;
        }


        public override void OnTouchBean(Vector3 position)
        {
            position.z = 100;
            position = cameraUI.ScreenToWorldPoint(position);
            joystick.TouchBegin(position);   
        }

        public override void OnTouchMove(Vector3 position)
        {
            position.z = 100;
            position = cameraUI.ScreenToWorldPoint(position);
            joystick.TouchMove(position);
        }

        public override void OnTouchEnd(Vector3 position)
        {           
            position.z = 100;
            position = cameraUI.ScreenToWorldPoint(position);
            joystick.TouchEnd(position);
        }

        public override void OnTouchStationary(Vector3 position)
        {
            position.z = 100;
            position = cameraUI.ScreenToWorldPoint(position);
            joystick.TouchMove(position);
        }
    }
}

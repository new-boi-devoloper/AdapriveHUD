using System;
using System.Collections;
using GamePush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePush_KT
{
    public class GamePushManager : MonoBehaviour
    {
        [Header("Обязательный минимум UI")]
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI playerIdText;
        public Button initButton;
        public Button authButton;
        public Button saveButton;
        public Button loadButton;

        [Header("Тестовые данные")]
        public int testScore = 0;
        public string testName = "Player";

        private bool _isInitialized = false;
        private bool _isAuthorized = false;
        private string _playerId = "";

        private void Start()
        {
            SetupUI();
            
            StartCoroutine(CheckInitializationStatus());
        }

        private IEnumerator CheckInitializationStatus()
        {
            yield return new WaitForSeconds(1f);
            
            if (GP_Init.isReady)
            {
                OnSDKReady();
            }
            else
            {
                SubscribeToEvents();
            }
        }

        private void SubscribeToEvents()
        {
            
            GP_Init.OnReady += OnSDKReady;
            GP_Init.OnError += OnSDKError;
            GP_Player.OnConnect += OnPlayerConnected;
            GP_Player.OnLoginComplete += OnLoginComplete;
            GP_Player.OnPlayerChange += OnPlayerChange;
            GP_Player.OnSyncComplete += OnSyncComplete;
            GP_Player.OnSyncError += OnSyncError;
        }

        private void UnsubscribeFromEvents()
        {
            GP_Init.OnReady -= OnSDKReady;
            GP_Init.OnError -= OnSDKError;
            GP_Player.OnConnect -= OnPlayerConnected;
            GP_Player.OnLoginComplete -= OnLoginComplete;
            GP_Player.OnPlayerChange -= OnPlayerChange;
            GP_Player.OnSyncComplete -= OnSyncComplete;
            GP_Player.OnSyncError -= OnSyncError;
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SetupUI()
        {
            
            if (initButton != null) 
            {
                initButton.onClick.RemoveAllListeners();
                initButton.onClick.AddListener(ManualInitializeSDK);
            }
            if (authButton != null) 
            {
                authButton.onClick.RemoveAllListeners();
                authButton.onClick.AddListener(AuthPlayer);
            }
            if (saveButton != null) 
            {
                saveButton.onClick.RemoveAllListeners();
                saveButton.onClick.AddListener(SaveData);
            }
            if (loadButton != null) 
            {
                loadButton.onClick.RemoveAllListeners();
                loadButton.onClick.AddListener(LoadData);
            }

            UpdateUI();
        }

        private void ManualInitializeSDK()
        {
            UpdateStatus("Manual initialization");
            
            if (GP_Init.isReady)
            {
                OnSDKReady();
            }
            else
            {
                UpdateStatus("SDK not ready");
                StartCoroutine(WaitForInitialization());
            }
        }

        private IEnumerator WaitForInitialization()
        {
            float timeout = 5f;
            float elapsed = 0f;
            
            while (elapsed < timeout && !GP_Init.isReady)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            if (GP_Init.isReady)
            {
                OnSDKReady();
            }
            else
            {
                UpdateStatus("Initialization timeout");
                Debug.LogError("SDK initialization timeout");
            }
        }

        private void OnSDKReady()
        {
            _isInitialized = true;
            UpdateStatus("SDK ready");
            
            CheckAuthStatus();
            UpdateUI();
            
            Debug.Log("Player ID: " + GP_Player.GetID());
            Debug.Log("Player Name: " + GP_Player.GetName());
            Debug.Log("Is Logged In: " + GP_Player.IsLoggedIn());
        }

        private void OnSDKError()
        {
            Debug.LogError("SDK initialization error");
            UpdateStatus("Initialization error");
        }

        private void CheckAuthStatus()
        {
            bool isLoggedIn = GP_Player.IsLoggedIn();
            
            if (isLoggedIn)
            {
                _isAuthorized = true;
                _playerId = GP_Player.GetID().ToString();
                UpdatePlayerIdDisplay();
                Debug.Log("Player authorized: " + _playerId);
                UpdateStatus("Authorized");
            }
            else
            {
                UpdateStatus("Not authorized");
            }
        }

        private void AuthPlayer()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("Initialize SDK first");
                UpdateStatus("Initialize SDK first");
                return;
            }

            UpdateStatus("Authorization");
            
            try
            {
                GP_Player.Login();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Login call error: " + e.Message);
                UpdateStatus("Authorization error");
            }
        }

        private void OnPlayerConnected()
        {
            _isAuthorized = true;
            _playerId = GP_Player.GetID().ToString();
            UpdatePlayerIdDisplay();
            Debug.Log("Player connected: " + _playerId);
            UpdateStatus("Successful authorization");
            UpdateUI();
        }

        private void OnLoginComplete()
        {
            Debug.Log("OnLoginComplete called");
            CheckAuthStatus();
        }

        private void OnPlayerChange()
        {
            Debug.Log("OnPlayerChange called");
        }

        private void OnSyncComplete()
        {
            UpdateStatus("Data synchronized");
        }

        private void OnSyncError()
        {
            Debug.LogError("OnSyncError called - sync error");
            UpdateStatus("Sync error");
        }

        private void UpdatePlayerIdDisplay()
        {
            if (playerIdText != null)
            {
                playerIdText.text = "Player ID: " + _playerId;
                playerIdText.color = Color.green;
            }
        }

        public void SaveData()
        {
            if (!_isInitialized)
            {
                UpdateStatus("SDK not initialized");
                return;
            }

            UpdateStatus("Saving");

            testScore += 10;

            SaveLocal();

            if (_isAuthorized)
            {
                SaveCloud();
            }
            else
            {
                UpdateStatus("Saved locally (no auth)");
            }
        }

        private void SaveLocal()
        {
            try
            {
                PlayerPrefs.SetInt("testScore", testScore);
                PlayerPrefs.SetString("testName", testName);
                PlayerPrefs.SetString("lastSave", DateTime.Now.ToString());
                PlayerPrefs.Save();

            }
            catch (Exception e)
            {
                Debug.LogError("Local save error: " + e.Message);
            }
        }

        private void SaveCloud()
        {
            try
            {
                
                // ПРАВИЛЬНЫЕ МЕТОДЫ из GP_Player
                GP_Player.Set("testScore", testScore);
                GP_Player.Set("testName", testName);
                GP_Player.Set("lastSave", DateTime.Now.ToString());

                // Синхронизируем
                GP_Player.Sync();

                UpdateStatus("Saved to cloud");
            }
            catch (Exception e)
            {
                Debug.LogError("Cloud save error: " + e.Message);
                UpdateStatus("Cloud save error");
            }
        }

        public void LoadData()
        {
            if (!_isInitialized)
            {
                UpdateStatus("SDK not initialized");
                return;
            }

            UpdateStatus("Loading");

            if (_isAuthorized)
            {
                LoadCloud();
            }
            else
            {
                LoadLocal();
            }
        }

        private void LoadCloud()
        {
            try
            {
                // ПРАВИЛЬНЫЕ МЕТОДЫ из GP_Player
                bool hasScore = GP_Player.Has("testScore");
                bool hasName = GP_Player.Has("testName");
                bool hasLastSave = GP_Player.Has("lastSave");

                Debug.Log("Key check - Score: " + hasScore + ", Name: " + hasName + ", LastSave: " + hasLastSave);

                bool hasCloudData = false;

                if (hasScore)
                {
                    int cloudScore = GP_Player.GetInt("testScore");
                    testScore = cloudScore;
                    hasCloudData = true;
                }

                if (hasName)
                {
                    string cloudName = GetPlayerStringSafe("testName");
                    if (!string.IsNullOrEmpty(cloudName))
                    {
                        testName = cloudName;
                        hasCloudData = true;
                    }
                }

                if (hasCloudData)
                {
                    UpdateStatus("Loaded from cloud");
                    
                    // Сохраняем локально для резервной копии
                    SaveLocal();
                }
                else
                {
                    LoadLocal();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Cloud load error: " + e.Message);
                LoadLocal();
            }
        }

        // Безопасный метод для получения строковых значений
        private string GetPlayerStringSafe(string key)
        {
            try
            {
                if (GP_Player.Has(key))
                {
                    string value = GP_Player.GetString(key);
                    return string.IsNullOrEmpty(value) ? null : value;
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError("Error getting key " + key + ": " + e.Message);
                return null;
            }
        }

        private void LoadLocal()
        {
            try
            {
                testScore = PlayerPrefs.GetInt("testScore", 0);
                testName = PlayerPrefs.GetString("testName", "Player");

                UpdateStatus("Loaded locally");

                if (_isAuthorized)
                {
                    StartCoroutine(DelayedCloudSave());
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Local load error: " + e.Message);
                UpdateStatus("Load error");
            }
        }

        private IEnumerator DelayedCloudSave()
        {
            yield return new WaitForSeconds(1f);
            SaveCloud();
        }

        private void UpdateUI()
        {
            if (initButton != null) 
            {
                initButton.interactable = !_isInitialized;
                initButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                    _isInitialized ? "SDK Ready" : "Initialize SDK";
            }
            
            if (authButton != null) 
            {
                authButton.interactable = _isInitialized && !_isAuthorized;
                authButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                    _isAuthorized ? "Authorized" : "Authorize";
            }
            
            if (saveButton != null) saveButton.interactable = _isInitialized;
            if (loadButton != null) loadButton.interactable = _isInitialized;

            string status = _isInitialized ? "SDK: Good" : "SDK: Bad";
            status += _isAuthorized ? " | Auth: Good" : " | Auth: Bad";
            status += "\nScore: " + testScore + " | Name: " + testName;
        
            if (statusText != null)
                statusText.text = status;
        }

        private void UpdateStatus(string message)
        {
            Debug.Log("Status: " + message);
            
            if (statusText != null)
            {
                string currentStatus = statusText.text;
                if (currentStatus.Contains("\n"))
                {
                    string[] parts = currentStatus.Split('\n');
                    if (parts.Length > 1)
                    {
                        statusText.text = message + "\n" + parts[1];
                    }
                    else
                    {
                        statusText.text = message + "\n" + currentStatus;
                    }
                }
                else
                {
                    statusText.text = message + "\n" + currentStatus;
                }
                
                StartCoroutine(ClearTemporaryStatus());
            }
        }

        private IEnumerator ClearTemporaryStatus()
        {
            yield return new WaitForSeconds(3f);
            UpdateUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5)) SaveData();
            if (Input.GetKeyDown(KeyCode.F9)) LoadData();
            if (Input.GetKeyDown(KeyCode.F1)) CheckAuthStatus();
        }
    }
}
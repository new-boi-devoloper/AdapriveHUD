// using TMPro;
// using UnityEngine;
//
// namespace GamePush_KT
// {
// using UnityEngine;
// using UnityEngine.UI;
//
// public class GamePushTester : MonoBehaviour
// {
//     [Header("UI References")]
//     public TextMeshProUGUI playerInfoText;
//     public TextMeshProUGUI gameDataText;
//     public Button incrementScoreButton;
//     public Button unlockAchievementButton;
//     public Button showFullscreenAdButton;
//     
//     private void Start()
//     {
//         SetupUI();
//         SetupEventListeners();
//     }
//     
//     private void SetupUI()
//     {
//         if (incrementScoreButton != null)
//             incrementScoreButton.onClick.AddListener(IncrementScore);
//             
//         if (unlockAchievementButton != null)
//             unlockAchievementButton.onClick.AddListener(UnlockAchievement);
//             
//         if (showFullscreenAdButton != null)
//             showFullscreenAdButton.onClick.AddListener(ShowFullscreenAd);
//     }
//     
//     private void SetupEventListeners()
//     {
//         if (GamePushManager.Instance != null)
//         {
//             GamePushManager.Instance.OnPlayerAuthorized += OnPlayerAuthorized;
//             GamePushManager.Instance.OnSDKInitialized += OnSDKInitialized;
//         }
//     }
//     
//     private void OnSDKInitialized()
//     {
//         Debug.Log("🎮 SDK Initialized - Tester ready");
//         UpdateUI();
//     }
//     
//     private void OnPlayerAuthorized(string playerId)
//     {
//         Debug.Log($"🎮 Player authorized in tester: {playerId}");
//         UpdateUI();
//     }
//     
//     private void IncrementScore()
//     {
//         if (GamePushManager.Instance != null)
//         {
//             GamePushManager.Instance.currentGameData.playerScore += 10;
//             UpdateGameDataUI();
//             
//         }
//     }
//     
//     private void UnlockAchievement()
//     {
//         if (GamePushManager.Instance != null)
//         {
//             string achievement = $"achievement_{GamePushManager.Instance.currentGameData.unlockedAchievements.Count + 1}";
//             GamePushManager.Instance.currentGameData.unlockedAchievements.Add(achievement);
//             UpdateGameDataUI();
//             
//             Debug.Log($"🏆 Achievement unlocked: {achievement}");
//             
//         }
//     }
//     
//     private void ShowFullscreenAd()
//     {
//         if (GamePushManager.Instance != null)
//         {
//             GamePushManager.Instance.ShowFullscreenAd();
//         }
//     }
//     
//     private void UpdateUI()
//     {
//         UpdatePlayerInfoUI();
//         UpdateGameDataUI();
//     }
//     
//     private void UpdatePlayerInfoUI()
//     {
//         if (playerInfoText != null && GamePushManager.Instance != null)
//         {
//             string playerInfo = $"Player: {GamePushManager.Instance.currentGameData.playerName}\n";
//             playerInfo += $"Level: {GamePushManager.Instance.currentGameData.playerLevel}\n";
//             playerInfo += $"Score: {GamePushManager.Instance.currentGameData.playerScore}\n";
//             playerInfo += $"ID: {GamePushManager.Instance.PlayerId}";
//             
//             playerInfoText.text = playerInfo;
//         }
//     }
//     
//     private void UpdateGameDataUI()
//     {
//         if (gameDataText != null && GamePushManager.Instance != null)
//         {
//             var data = GamePushManager.Instance.currentGameData;
//             string gameInfo = $"Achievements: {data.unlockedAchievements.Count}\n";
//             gameInfo += $"Last Save: {data.lastSaveTime:HH:mm:ss}";
//             
//             gameDataText.text = gameInfo;
//         }
//     }
//     
//     private void Update()
//     {
//         // Обновляем UI каждый кадр для актуальности
//         UpdateUI();
//         
//         // Быстрые тесты по клавишам
//         if (Input.GetKeyDown(KeyCode.F1))
//         {
//             SaveGameData();
//         }
//         if (Input.GetKeyDown(KeyCode.F2))
//         {
//             LoadGameData();
//         }
//         if (Input.GetKeyDown(KeyCode.F3))
//         {
//             ShowRewardedAd();
//         }
//     }
//     
//     private void SaveGameData()
//     {
//         if (GamePushManager.Instance != null)
//         {
//             GamePushManager.Instance.SaveGameData();
//         }
//     }
//     
//     private void LoadGameData()
//     {
//         if (GamePushManager.Instance != null)
//         {
//             GamePushManager.Instance.LoadGameData();
//         }
//     }
//     
//     private void ShowRewardedAd()
//     {
//         if (GamePushManager.Instance != null)
//         {
//             GamePushManager.Instance.ShowRewardedAd();
//         }
//     }
// }
// }
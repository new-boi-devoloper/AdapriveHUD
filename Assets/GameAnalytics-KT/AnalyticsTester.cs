using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;

namespace GameAnalytics_KT
{
    public class AnalyticsTester : MonoBehaviour
    {
        [Header("UI References")] public Button startLevelButton;
        public Button completeLevelButton;
        public Button failLevelButton;
        public Button openShopButton;
        public Button killEnemyButton;
        public Button gainGoldButton;
        public Button spendGoldButton;
        public Button triggerErrorButton;

        private int currentLevel = 1;
        private int goldAmount = 1000;

        void Start()
        {
            //  кнопоки
            startLevelButton.onClick.AddListener(OnStartLevel);
            completeLevelButton.onClick.AddListener(OnCompleteLevel);
            failLevelButton.onClick.AddListener(OnFailLevel);
            openShopButton.onClick.AddListener(OnOpenShop);
            killEnemyButton.onClick.AddListener(OnKillEnemy);
            gainGoldButton.onClick.AddListener(OnGainGold);
            spendGoldButton.onClick.AddListener(OnSpendGold);
            triggerErrorButton.onClick.AddListener(OnTriggerError);

            AnalyticsManager.Instance.SetCustomDimension01("casual_player");
            AnalyticsManager.Instance.SetCustomDimension02("mobile_device");
        }

        void OnStartLevel()
        {
            AnalyticsManager.Instance.StartLevel(currentLevel);
        }

        void OnCompleteLevel()
        {
            int score = Random.Range(100, 1000);
            AnalyticsManager.Instance.CompleteLevel(currentLevel, score);
            currentLevel++;
        }

        void OnFailLevel()
        {
            string[] reasons = { "time_out", "player_died", "objective_failed" };
            string reason = reasons[Random.Range(0, reasons.Length)];
            AnalyticsManager.Instance.FailLevel(currentLevel, reason);
        }

        void OnOpenShop()
        {
            AnalyticsManager.Instance.TrackShopEvent("open");
            AnalyticsManager.Instance.TrackButtonClick("shop_button");
        }

        void OnKillEnemy()
        {
            string[] enemyTypes = { "robot_small", "robot_large", "alien", "zombie" };
            string[] weapons = { "pistol", "rifle", "shotgun", "rocket_launcher" };

            string enemy = enemyTypes[Random.Range(0, enemyTypes.Length)];
            string weapon = weapons[Random.Range(0, weapons.Length)];

            AnalyticsManager.Instance.TrackEnemyKill(enemy, weapon);
        }

        void OnGainGold()
        {
            int amount = Random.Range(10, 100);
            goldAmount += amount;
            AnalyticsManager.Instance.TrackResourceGain("gold", amount, "enemy_kill");
        }

        void OnSpendGold()
        {
            int amount = Random.Range(5, 50);
            if (goldAmount >= amount)
            {
                goldAmount -= amount;
                string[] items = { "health_potion", "ammo", "upgrade" };
                string item = items[Random.Range(0, items.Length)];
                AnalyticsManager.Instance.TrackResourceSpend("gold", amount, item);
            }
        }

        void OnTriggerError()
        {
            string[] errors =
            {
                "Test warning: Low memory",
                "Test error: Network timeout",
                "Test critical: Save game corrupted"
            };

            GAErrorSeverity[] severities =
            {
                GAErrorSeverity.Warning,
                GAErrorSeverity.Error,
                GAErrorSeverity.Critical
            };

            int index = Random.Range(0, errors.Length);
            AnalyticsManager.Instance.TrackError(errors[index], severities[index]);
        }
    }
}
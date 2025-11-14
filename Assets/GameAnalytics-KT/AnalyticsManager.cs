using GameAnalyticsSDK;
using UnityEngine;

namespace GameAnalytics_KT
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance;
        
        [Header("Debug")]
        public bool enableDebug = true;
    
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAnalytics();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        private void InitializeAnalytics()
        {
            GameAnalytics.Initialize();
        
            Log("GameAnalytics initialized");
        
            // Туст запуска
            GameAnalytics.NewDesignEvent("game:first_launch");
        }
    
        // ========== PROGRESSION EVENTS ==========
    
        public void StartLevel(int levelNumber)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level_" + levelNumber);
            Log($"Level {levelNumber} started");
        }
    
        public void CompleteLevel(int levelNumber, int score = 0)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level_" + levelNumber, score);
            Log($"Level {levelNumber} completed with score: {score}");
        }
    
        public void FailLevel(int levelNumber, string reason = "unknown")
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level_" + levelNumber);
            GameAnalytics.NewDesignEvent($"level:fail:reason_{reason}");
            Log($"Level {levelNumber} failed: {reason}");
        }
    
        // ========== DESIGN EVENTS ==========
    
        public void TrackShopEvent(string action, string item = "")
        {
            string eventName = $"shop:{action}";
            if (!string.IsNullOrEmpty(item))
            {
                eventName += $":{item}";
            }
        
            GameAnalytics.NewDesignEvent(eventName);
            Log($"Shop event: {eventName}");
        }
    
        public void TrackEnemyKill(string enemyType, string weapon = "")
        {
            string eventName = $"enemy:kill:{enemyType}";
            if (!string.IsNullOrEmpty(weapon))
            {
                eventName += $":{weapon}";
            }
        
            GameAnalytics.NewDesignEvent(eventName);
            Log($"Enemy kill: {enemyType} with {weapon}");
        }
    
        public void TrackButtonClick(string buttonName)
        {
            GameAnalytics.NewDesignEvent($"ui:button_click:{buttonName}");
            Log($"Button clicked: {buttonName}");
        }
    
        // ========== RESOURCE EVENTS ==========
    
        public void TrackResourceGain(string currency, int amount, string source)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, currency, amount, "gameplay", source);
            Log($"Resource gained: {amount} {currency} from {source}");
        }
    
        public void TrackResourceSpend(string currency, int amount, string item)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, currency, amount, "shop", item);
            Log($"Resource spent: {amount} {currency} on {item}");
        }
    
        // ========== BUSINESS EVENTS ==========
    
        public void TrackPurchase(string productId, decimal price, string currency = "USD")
        {
            GameAnalytics.NewBusinessEvent(currency, (int)(price * 100), productId, "premium_currency", "store");
            Log($"Purchase: {productId} for {price} {currency}");
        }
    
        // ========== ERROR EVENTS ==========
    
        public void TrackError(string message, GAErrorSeverity severity = GAErrorSeverity.Warning)
        {
            GameAnalytics.NewErrorEvent(severity, message);
            Log($"Error tracked: {severity} - {message}");
        }
    
        // ========== CUSTOM DIMENSIONS ==========
    
        public void SetCustomDimension01(string dimension)
        {
            GameAnalytics.SetCustomDimension01(dimension);
            Log($"Custom Dimension 01 set to: {dimension}");
        }
    
        public void SetCustomDimension02(string dimension)
        {
            GameAnalytics.SetCustomDimension02(dimension);
            Log($"Custom Dimension 02 set to: {dimension}");
        }
    
        public void SetCustomDimension03(string dimension)
        {
            GameAnalytics.SetCustomDimension03(dimension);
            Log($"Custom Dimension 03 set to: {dimension}");
        }
    
        // ========== UTILITY METHODS ==========
    
        private void Log(string message)
        {
            if (enableDebug)
            {
                Debug.Log($"[Analytics] {message}");
            }
        }
    
        // Метод для принудительной отправки событий
        public void FlushEvents()
        {
            // GameAnalytics автоматически отправляет события, но можно принудительно
            Log("Events flushed");
        }
    }
}
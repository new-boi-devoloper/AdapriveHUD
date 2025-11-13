using UnityEngine;

namespace Remote_Config
{
    public class ConfigTester : MonoBehaviour
    {
        private RemoteConfigLoader _configLoader;
        private string fakeId = "abcdefg";
        
        void Start()
        {
            _configLoader = FindObjectOfType<RemoteConfigLoader>();
            
            // тест через 2 секунды после старта
            Invoke("TestConfig", 2f);
        }
    
        private void TestConfig()
        {
            var weapons = _configLoader.GetWeapons();
        
            Debug.Log($"weapons count: {weapons.Count}");
        
            foreach (var weapon in weapons)
            {
                // тестируем поиск по id
                var foundWeapon = _configLoader.GetWeaponById(weapon.id);
                if (foundWeapon != null)
                {
                    Debug.Log($"found weapon: {foundWeapon}");
                }
            }
        
            // тестируем неправльное оружие
            var missingWeapon = _configLoader.GetWeaponById(fakeId);
            if (missingWeapon == null)
            {
                Debug.Log($"nonexistent weapon: {fakeId}");
            }
        }
    
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Reload...");
                _configLoader.ReloadConfig();
            }
        }
    }
}
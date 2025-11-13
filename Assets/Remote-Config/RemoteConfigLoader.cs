using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Remote_Config
{
    public class RemoteConfigLoader : MonoBehaviour
    {
        [Header("Settings")] public string googleSheetsUrl =
            "https://docs.google.com/spreadsheets/d/e/2PACX-1vTjqY4siTmAaRE8SK5YeHBVeIOGUx5_jbgLHIwOCOYuQ_kASlA9Jns1pQ961AFsmIxF609Oz0m60Oiu/pub?output=csv";

        public float timeout = 10f;

        [Header("Default Weapos")] public List<Weapon> defaultWeapons = new()
        {
            new Weapon("pistol", 5, 1.0f),
            new Weapon("rifle", 15, 0.5f)
        };

        private readonly List<Weapon> _loadedWeapons = new();
        private string _localConfigPath;

        private void Start()
        {
            _localConfigPath = Path.Combine(Application.persistentDataPath, "weapons_config.csv");
            StartCoroutine(LoadConfig());
        }

        public IEnumerator LoadConfig()
        {
            Debug.Log("Started config load...");

            yield return StartCoroutine(LoadRemoteConfig());
            ApplyConfig();
        }
        
        public void ReloadConfig()
        {
            StartCoroutine(LoadConfig());
        }

        public List<Weapon> GetWeapons()
        {
            return new List<Weapon>(_loadedWeapons);
        }

        public Weapon GetWeaponById(string weaponId)
        {
            return _loadedWeapons.FirstOrDefault(w => w.id == weaponId);
        }
        
        private IEnumerator LoadRemoteConfig()
        {
            try
            {
                using var webRequest = UnityWebRequest.Get(googleSheetsUrl);

                webRequest.timeout = (int)timeout;

                Debug.Log($"Downloading config from: {googleSheetsUrl}");
                webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var csvData = webRequest.downloadHandler.text;
                    Debug.Log("Remote config downloaded successfully");
                    Debug.Log($"Raw CSV data: {csvData}");

                    if (ParseAndValidateCSV(csvData))
                    {
                        SaveLocalConfig(csvData);
                        Debug.Log("Remote config applied and saved locally");
                    }
                    else
                    {
                        Debug.LogWarning("Remote config validation failed, trying local backup");
                        LoadLocalConfig();
                    }
                }
                else
                {
                    Debug.LogWarning($"Remote config download failed: {webRequest.error}");
                    LoadLocalConfig();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e}");
                yield break;
            }
        }

        private bool ParseAndValidateCSV(string csvData)
        {
            try
            {
                _loadedWeapons.Clear();

                // проверка на BOM символ если есть
                csvData = csvData.Trim('\uFEFF');

                var lines = csvData.Split('\n');
                Debug.Log($"Found {lines.Length} lines in CSV");

                // пропуск заголовока с id, damage, cooldown
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    var fields = ParseCSVLine(line);
                    Debug.Log($"Line {i}: {string.Join("|", fields)}");

                    if (fields.Length >= 3)
                    {
                        var id = fields[0].Trim();

                        // jбрабатываем числа с разными разделителями
                        var damageStr = fields[1].Trim().Replace(",", ".");
                        var cooldownStr = fields[2].Trim().Replace(",", ".");

                        if (int.TryParse(damageStr, NumberStyles.Integer,
                                CultureInfo.InvariantCulture, out var damage) &&
                            float.TryParse(cooldownStr, NumberStyles.Float,
                                CultureInfo.InvariantCulture, out var cooldown))
                        {
                            if (damage >= 0 && cooldown > 0)
                            {
                                _loadedWeapons.Add(new Weapon(id, damage, cooldown));
                                Debug.Log($"Validated: {id} - Damage: {damage}, Cooldown: {cooldown}");
                            }
                            else
                            {
                                Debug.LogWarning($"Invalid values for {id}: Damage={damage}, Cooldown={cooldown}");
                                return false;
                            }
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Failed to parse values for {id}: damage='{fields[1]}', cooldown='{fields[2]}'");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Not enough fields in line: {line}");
                        return false;
                    }
                }

                Debug.Log($"Successfully parsed {_loadedWeapons.Count} weapons");
                return _loadedWeapons.Count > 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV parsing error: {e.Message}");
                return false;
            }
        }

        private string[] ParseCSVLine(string line)
        {
            var result = new List<string>();
            var inQuotes = false;
            var currentField = "";

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }

            result.Add(currentField);

            return result.ToArray();
        }

        private void SaveLocalConfig(string csvData)
        {
            try
            {
                File.WriteAllText(_localConfigPath, csvData);
                Debug.Log($"Local config saved to: {_localConfigPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save local config: {e.Message}");
            }
        }

        private void LoadLocalConfig()
        {
            try
            {
                if (File.Exists(_localConfigPath))
                {
                    var localData = File.ReadAllText(_localConfigPath);
                    Debug.Log("Trying to load");

                    if (!ParseAndValidateCSV(localData)) throw new Exception("Local config validation failed");

                    Debug.Log(" Local backup config loaded successfully");
                }
                else
                {
                    throw new Exception("Local backup not found");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Local config loading failed: {e.Message}");
                Debug.Log("Fallback to default weapon values");
                UseDefaultWeapons();
            }
        }

        private void UseDefaultWeapons()
        {
            _loadedWeapons.Clear();
            _loadedWeapons.AddRange(defaultWeapons);
        }

        private void ApplyConfig()
        {
            foreach (var weapon in _loadedWeapons) Debug.Log(weapon.ToString());
        }
    }
}
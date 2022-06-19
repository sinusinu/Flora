using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Flora.Util;

public class Preference {
    internal string path;
    internal Dictionary<string, string> pref;

    public Preference(string path) {
        this.path = path;

        if (File.Exists(path)) {
            Load();
        } else {
            Directory.CreateDirectory(Directory.GetParent(path).FullName);
            using (File.Create(path)) {}
            pref = new Dictionary<string, string>();
        }
    }

    ~Preference() {
        Save();
    }

    private void Load() {
        byte[] rb = File.ReadAllBytes(path);
        if (rb.Length == 0) {
            pref = new Dictionary<string, string>();
            return;
        }
        var ros = new ReadOnlySpan<byte>(rb);
        try {
            pref = JsonSerializer.Deserialize<Dictionary<string, string>>(ros);
        } catch (Exception) {
            // TODO: do better error handling
            pref = new Dictionary<string, string>();
        }
    }

    public void Save() {
        byte[] data = JsonSerializer.SerializeToUtf8Bytes<Dictionary<string, string>>(pref);
        using (var fs = new FileStream(path, FileMode.Create)) {
            fs.Write(data);
            fs.Flush();
        }
    }

    public bool ContainsKey(string key) {
        return pref.ContainsKey(key);
    }

#region Getters
    public string GetValueString(string key, string fallback = null) {
        string ret;
        if (!pref.TryGetValue(key, out ret)) return fallback;
        return ret;
    }

    public int GetValueInt(string key, int fallback = 0) {
        string rawStr; int ret;
        if (!pref.TryGetValue(key, out rawStr)) return fallback;
        if (!int.TryParse(rawStr, out ret)) throw new InvalidDataException("Key " + key + " have non-integer value");
        return ret;
    }

    public float GetValueFloat(string key, float fallback = 0f) {
        string rawStr; float ret;
        if (!pref.TryGetValue(key, out rawStr)) return fallback;
        if (!float.TryParse(rawStr, out ret)) throw new InvalidDataException("Key " + key + " have non-float value");
        return ret;
    }

    public bool GetValueBool(string key, bool fallback = false) {
        string rawStr;
        if (!pref.TryGetValue(key, out rawStr)) return fallback;
        if (rawStr.Equals("true")) return true;
        else if (rawStr.Equals("false")) return false;
        throw new InvalidDataException("Key " + key + " have non-bool value");
    }
#endregion

#region Setters
    public void SetValue(string key, string value) { pref[key] = value; }
    public void SetValue(string key, int value) { pref[key] = value.ToString(); }
    public void SetValue(string key, float value) { pref[key] = value.ToString(); }
    public void SetValue(string key, bool value) { pref[key] = value ? "true" : "false"; }
#endregion
}

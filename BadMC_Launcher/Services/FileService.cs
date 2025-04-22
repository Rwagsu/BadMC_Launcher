using System.ComponentModel;
using System.Reflection;
using System.Security;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BadMC_Launcher.Models.Datas;
using BadMC_Launcher.Models.Datas.Mappings;
using Microsoft.UI.Xaml.Controls;
using MinecraftLaunch.Extensions;

namespace BadMC_Launcher.Services;

public class FileService {
    public bool CheckFolderAndFile(string path, bool isCheckFile) {
        if (isCheckFile ? File.Exists(path) : Path.Exists(path)) {
            return true;
        }
        else {
            try {
                if (isCheckFile) {
                    var directory = Path.GetDirectoryName(path);
                    if (directory != null) {
                        Directory.CreateDirectory(directory);
                    }
                    using (File.Create(path)) {
                        return true;
                    }
                }
                else {
                    Directory.CreateDirectory(path);
                    return true;
                }
            }
            catch (Exception ex) {
                if (!ShowErrorToast(ex)) {
                    throw;
                }
            }
        }
        return false;
    }

    public bool ReadConfig<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, out T? returnValue, Dictionary<string, string>? updateMapping = null) {
        if (CheckFolderAndFile(filePath, true)) {
            try {
                var fileValue = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileValue)) {
                    returnValue = fileValue.Deserialize(jsonTypeInfo);
                    return true;
                }
            }
            catch (Exception ex) {
                switch (ex) {
                    case JsonException:
                        if (updateMapping != null && TryReadConfigWithMappings(filePath, jsonTypeInfo, updateMapping)) {
                            ReadConfig(filePath, jsonTypeInfo, out _, updateMapping);
                        }
                        else {
                            //TODO: Dialog
                        }
                        break;
                    default:
                        if (!ShowErrorToast(ex)) {
                            throw;
                        }
                        break;
                }
            }
        }
        returnValue = default;
        return false;
    }

    public bool ReadConfigToJsonElement(string filePath, out JsonElement? returnValue) {
        if (CheckFolderAndFile(filePath, true)) {
            try {
                var fileValue = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileValue)) {
                    var jsonValue = JsonDocument.Parse(fileValue);
                    if (jsonValue != null) {
                        returnValue = jsonValue.RootElement;
                        return true;
                    }
                }

            }
            catch (Exception ex) {
                if(!ShowErrorToast(ex)) {
                    throw;
                }
            }
        }
        returnValue = default;
        return false;
    }

    public bool WriteConfig<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, T value) {
        if (CheckFolderAndFile(filePath, true)) {
            try {
                var jsonValue = value.Serialize(jsonTypeInfo);
                File.WriteAllText(filePath, jsonValue);
                return true;
            }
            catch (Exception ex) {
                if (!ShowErrorToast(ex)) {
                    throw;
                }
            }
        }
        return false;
    }

    public bool TryReadConfigWithMappings<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, Dictionary<string, string> updateMapping) {
        try {
            if (CheckFolderAndFile(filePath, true)) {
                var content = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(content)) {
                    return false;
                }

                using JsonDocument doc = JsonDocument.Parse(content);
                JsonObject? root = JsonObject.Create(doc.RootElement);
                if (root == null) return false;

                bool modified = ProcessJsonNode(root, updateMapping);

                if (modified) {
                    File.WriteAllText(filePath, root.ToJsonString(new() { WriteIndented = true }));
                    return true;
                }
            }
        }
        catch(Exception ex) {
            if (!ShowErrorToast(ex)) {
                throw;
            }
        }
        return false;
    }

    public bool TryOpenFolderFromPath(string path) {
        try {
            using (Process.Start(new ProcessStartInfo(path) {
                UseShellExecute = true,
                Verb = "open"
            })) {
                return true;
            }
        }
        catch (Exception ex) {
            switch (ex) {
                case Win32Exception:
                    //TODO: Dialog
                    break;
                case FileNotFoundException:
                    //TODO: Dialog
                    break;
                default:
                    throw;
            }
        }
        return false;
    }

    private bool ProcessJsonNode(JsonNode node, Dictionary<string, string> updateMapping) {
        var DeleteItems = new Dictionary<string, string>();
        bool isModified = false;

        switch (node) {
            case JsonObject obj:
                // Loop to find nested objects
                foreach (var prop in obj.ToList()) {
                    if (prop.Value != null && ProcessJsonNode(prop.Value, updateMapping)) {
                        isModified = true;
                    }

                    // Modify the key name to correspond to the mapping
                    if (prop.Value != null && updateMapping.TryGetValue(prop.Key, out var newKey)) {
                        if (newKey == "_Delete") {
                            DeleteItems.Add(prop.Key, prop.Value.ToJsonString(new() {
                                WriteIndented = true,
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                            }));
                        }
                        else {
                            obj.Add(newKey, prop.Value.DeepClone());
                            obj.Remove(prop.Key);
                            isModified = true;
                        }
                    }
                }
                break;
            case JsonArray arr:
                // 处理数组元素
                for (int i = 0; i < arr.Count; i++) {
                    if (arr[i] is JsonNode element && ProcessJsonNode(element, updateMapping)) {
                        // Update Modified
                        arr[i] = element.DeepClone();
                        isModified = true;
                    }
                }
                break;
        }

        // Toast Tip
        if (DeleteItems.Any()) {
            // TODO: Toast
        }
        return isModified;
    }
    private bool ShowErrorToast(Exception ex) {
        switch (ex) {
            case SecurityException:
                //TODO
                break;
            case UnauthorizedAccessException:
                //TODO
                break;
            case PathTooLongException:
                //TODO
                break;
            case IOException:
                //TODO
                break;
            default:
                return false;
        }
        return true;
    }
}

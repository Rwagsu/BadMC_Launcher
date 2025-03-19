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

namespace BadMC_Launcher.Servicess;
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
                        throw;
                }
            }
        }
        return false;
    }

    public bool ReadConfig<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, out T? returnValue, Dictionary<string, string>? updateMapping = null) {
        if (CheckFolderAndFile(filePath, true)) {
            try {
                if (updateMapping != null) {
                    TryReadConfigWithMappings(filePath, jsonTypeInfo, updateMapping);
                }
                var fileValue = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileValue)) {
                    returnValue = fileValue.Deserialize(jsonTypeInfo);
                    return true;
                }
            }
            catch (Exception ex) {
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
                    case JsonException:
                        //TODO
                        break;
                    default:
                        throw;
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
                    case JsonException:
                        //TODO
                        break;
                    default:
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
                    case JsonException:
                        //TODO
                        break;
                    default:
                        throw;
                }
            }
        }
        return false;
    }

    public bool TryReadConfigWithMappings<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, Dictionary<string, string> updateMapping) {
        try {
            if (CheckFolderAndFile(filePath, true)) {
                var fileValue = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileValue)) {
            
                    using JsonDocument doc = JsonDocument.Parse(fileValue);

                    JsonObject? jsonObject = JsonObject.Create(doc.RootElement);
                    if (jsonObject != null) {
                        foreach (var item in jsonObject.ToList()) {
                            var updateKey = updateMapping.FirstOrDefault(mappingItem => mappingItem.Key == item.Key).Value;
                            if (updateKey != null) {
                                JsonNode? valueCopy = item.Value?.DeepClone();

                                jsonObject.Add(updateKey, valueCopy);
                                jsonObject.Remove(item.Key);

                                File.WriteAllText(filePath, jsonObject.ToJsonString(new() { WriteIndented = true }));
                                return true;
                            }
                        }
                    }
                    
                    
                }
            }
        }
        catch(Exception ex) {
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
                case JsonException:
                    //TODO
                    break;
                default:
                    throw;
            }
        }
        return false;
    }
}

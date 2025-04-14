using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.RegularExpressions;
using Reveal_Last_Alive.Config;
using System.Drawing;
using CounterStrikeSharp.API.Core.Translations;
using System.Security.Cryptography;


namespace Reveal_Last_Alive;

public class Helper
{
    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToChat(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeNone = true, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true) 
    {
        return Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => 
                p != null && 
                p.IsValid &&
                p.Connected == PlayerConnectedState.PlayerConnected &&
                (IncludeBots || !p.IsBot) &&
                (IncludeHLTV || !p.IsHLTV) &&
                ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
                (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
                (IncludeNone && p.TeamNum == (byte)CsTeam.None) || 
                (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p => 
            p != null && 
            p.IsValid && 
            p.Connected == PlayerConnectedState.PlayerConnected && 
            (IncludeBots || !p.IsBot) &&
            (IncludeHLTV || !p.IsHLTV) &&
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }
    public static void ClearVariables(bool All = true)
    {
        var g_Main = MainPlugin.Instance.g_Main;

        if(All)
        {
            g_Main.B_Ready = false;
        }
        
        g_Main.Chicken_Spawned = false;
        g_Main.Glow_Spawned = false;

        if (g_Main.Timer != null)
        {
            g_Main.Timer.Kill();
            g_Main.Timer = null!;
        }
        if (g_Main.chickenGLOW != null && g_Main.chickenGLOW.IsValid)
        {
            g_Main.chickenGLOW.Remove();
        }
        if (g_Main.chicken != null && g_Main.chicken.IsValid)
        {
            g_Main.chicken.Remove();
        }
        if (g_Main.modelRelay != null && g_Main.modelRelay.IsValid)
        {
            g_Main.modelRelay.Remove();
        }
        if (g_Main.modelGlow != null && g_Main.modelGlow.IsValid)
        {
            g_Main.modelGlow.Remove();
        }
    }

    public static void DebugMessage(string message, bool prefix = true)
    {
        if (!Configs.GetConfigData().EnableDebug) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string output = prefix ? $"[Last Alive]: {message}" : message;
        Console.WriteLine(output);
        
        Console.ResetColor();
    }

    public static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }

    public static void CreateResource(string jsonFilePath)
    {
        string headerLine = "////// vvvvvv Add Paths For Precache Resources Down vvvvvvvvvv //////";
        string headerLine2 = "// soundevents/goldkingz_sounds.vsndevts";
        string headerLine3 = "// soundevents/addons_goldkingz_sounds.vsndevts";
        if (!File.Exists(jsonFilePath))
        {
            using (StreamWriter sw = File.CreateText(jsonFilePath))
            {
                sw.WriteLine(headerLine);
                sw.WriteLine(headerLine2);
                sw.WriteLine(headerLine3);
            }
        }
        else
        {
            string[] lines = File.ReadAllLines(jsonFilePath);
            if (lines.Length == 0 || lines[0] != headerLine)
            {
                using (StreamWriter sw = new StreamWriter(jsonFilePath))
                {
                    sw.WriteLine(headerLine);
                    foreach (string line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }
    }

    public static Color GetColorFromConfig(string ColorString)
    {
        string colorString = ColorString;
        if (colorString.StartsWith("#"))
        {
            colorString = colorString.Substring(1);
        }
        
        int rgb = int.Parse(colorString, System.Globalization.NumberStyles.HexNumber);
        int red = (rgb >> 16) & 0xFF;
        int green = (rgb >> 8) & 0xFF;
        int blue = rgb & 0xFF;
        return Color.FromArgb(red, green, blue);
    }


    public static async Task DownloadMissingFiles()
    {
        try
        {
            string baseFolderPath = MainPlugin.Instance.ModuleDirectory;

            string gamedataFileName = "gamedata/gamedata.json";
            string gamedataGithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Private-Plugins/main/Resources/gamedata.json";
            string gamedataFilePath = Path.Combine(baseFolderPath, gamedataFileName);
            string gamedataDirectoryPath = Path.GetDirectoryName(gamedataFilePath)!;
            await CheckAndDownloadFile(gamedataFilePath, gamedataGithubUrl, gamedataDirectoryPath);
        }
        catch (Exception ex)
        {
            DebugMessage($"Error in DownloadMissingFiles: {ex.Message}");
        }
    }

    public static async Task<bool> CheckAndDownloadFile(string filePath, string githubUrl, string directoryPath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                await DownloadFileFromGithub(githubUrl, filePath);
                return true;
            }
            else
            {
                if (Configs.GetConfigData().AutoUpdateSignatures)
                {
                    bool isFileDifferent = await IsFileDifferent(filePath, githubUrl);
                    if (isFileDifferent)
                    {
                        File.Delete(filePath);
                        await DownloadFileFromGithub(githubUrl, filePath);
                        return true;
                    }
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            DebugMessage($"Error in CheckAndDownloadFile: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> IsFileDifferent(string localFilePath, string githubUrl)
    {
        try
        {
            byte[] localFileBytes = await File.ReadAllBytesAsync(localFilePath);
            string localFileHash = GetFileHash(localFileBytes);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    byte[] githubFileBytes = await client.GetByteArrayAsync(githubUrl, cts.Token);
                    string githubFileHash = GetFileHash(githubFileBytes);
                    return localFileHash != githubFileHash;
                }
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error comparing files: {ex.Message}");
            return false;
        }
    }

    public static string GetFileHash(byte[] fileBytes)
    {
        try
        {
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(fileBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error generating file hash: {ex.Message}");
            return string.Empty;
        }
    }

    public static async Task DownloadFileFromGithub(string url, string destinationPath)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            
            try
            {
                byte[] fileBytes = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(destinationPath, fileBytes);
            }
            catch (Exception ex)
            {
                DebugMessage($"Error downloading file: {ex.Message}");
            }
        }
    }

    
    public static void Start_Reveal(CCSPlayerController? lastPlayer)
    {
        if (!MainPlugin.Instance.g_Main.B_Ready)
        {
            ClearVariables();
            return;
        }

        if(!lastPlayer.IsValid(true))return;

        if(Configs.GetConfigData().Chicken_Enable)
        {
            SetGlowChicken(lastPlayer);
        }
        
        if(Configs.GetConfigData().Player_Enable)
        {
            SetGlowPlayer(lastPlayer);
        }

        if(!string.IsNullOrEmpty(Configs.GetConfigData().Play_Sound))
        {
            float effectiveVolume = Configs.GetConfigData().Sound_Volume.ToPercentageFloat();

            foreach(var players in GetPlayersController(IncludeNone : false))
            {
                if(!players.IsValid())continue;

                if(Configs.GetConfigData().Play_Sound.StartsWith("sounds/"))
                {
                    players.ExecuteClientCommand($"play {Configs.GetConfigData().Play_Sound}");
                }else
                {
                    RecipientFilter filter = [players];
                    players.EmitSound(Configs.GetConfigData().Play_Sound, filter, (float)effectiveVolume);
                }
            }
        }
    }

    public static Vector CalculateBehindPosition(CCSPlayerPawn pawn)
    {
        var basePos = pawn.AbsOrigin;
        float x = basePos!.X;
        float y = basePos.Y;
        float z = basePos.Z + 30.0f;

        float eyeAngleY = pawn.EyeAngles.Y;

        float angleInRadians = (eyeAngleY + 180) * (MathF.PI / 180);
        const float spawnOffset = 60.0f;

        float spawnX = x + spawnOffset * MathF.Cos(angleInRadians);
        float spawnY = y + spawnOffset * MathF.Sin(angleInRadians);
        
        return new Vector(
            spawnX,
            spawnY,
            z + 30.0f
        );
    }

    public static void SetGlowChicken(CCSPlayerController player)
    {
        if (!player.IsValid(true) || player.PlayerPawn.Value == null)return;

        Vector spawnPosition = CalculateBehindPosition(player.PlayerPawn.Value);

        if(MainPlugin.Instance.g_Main.Chicken_Spawned)
        {
            if (MainPlugin.Instance.g_Main.chicken == null || !MainPlugin.Instance.g_Main.chicken.IsValid || MainPlugin.Instance.g_Main.chickenGLOW == null || !MainPlugin.Instance.g_Main.chickenGLOW.IsValid)return;

            MainPlugin.Instance.g_Main.chicken.Teleport(spawnPosition);
            MainPlugin.Instance.g_Main.chickenGLOW.Teleport(spawnPosition);

            return;
        }
        
        MainPlugin.Instance.g_Main.chicken = Utilities.CreateEntityByName<CChicken>("chicken")!;
        if(MainPlugin.Instance.g_Main.chicken == null)return;

        MainPlugin.Instance.g_Main.chicken.Spawnflags = 256U;
        MainPlugin.Instance.g_Main.chicken.RenderMode = RenderMode_t.kRenderNone;
        MainPlugin.Instance.g_Main.chicken.DispatchSpawn();
        MainPlugin.Instance.g_Main.chicken.Teleport(spawnPosition);
        MainPlugin.Instance.g_Main.chicken.AcceptInput("DisableCollision");

        MainPlugin.Instance.g_Main.chickenGLOW = Utilities.CreateEntityByName<CChicken>("chicken")!;
        if(MainPlugin.Instance.g_Main.chickenGLOW == null)return;

        MainPlugin.Instance.g_Main.chickenGLOW.Spawnflags = 256u;
        MainPlugin.Instance.g_Main.chickenGLOW.DispatchSpawn();
        MainPlugin.Instance.g_Main.chickenGLOW.Glow.GlowColorOverride = Helper.GetColorFromConfig(Configs.GetConfigData().Chicken_GlowColor);
        MainPlugin.Instance.g_Main.chickenGLOW.Glow.GlowRange = Configs.GetConfigData().Chicken_GlowRange;
        MainPlugin.Instance.g_Main.chickenGLOW.Glow.GlowTeam = -1;
        MainPlugin.Instance.g_Main.chickenGLOW.Glow.GlowType = Configs.GetConfigData().Chicken_GlowType?2:3;
        MainPlugin.Instance.g_Main.chickenGLOW.Glow.GlowRangeMin = 100;

        MainPlugin.Instance.g_Main.chickenGLOW.Teleport(spawnPosition);
        MainPlugin.Instance.g_Main.chickenGLOW.AcceptInput("DisableCollision");
        MainPlugin.Instance.g_Main.chickenGLOW.AcceptInput("SetScale", null, null, Configs.GetConfigData().Chicken_Size.ToString());
        MainPlugin.Instance.g_Main.chickenGLOW.AcceptInput("SetParent", caller: MainPlugin.Instance.g_Main.chickenGLOW, activator: MainPlugin.Instance.g_Main.chicken, value: "!activator");
        MainPlugin.Instance.g_Main.chicken.AcceptInput("SetParent", caller: MainPlugin.Instance.g_Main.chicken, activator: player.PlayerPawn.Value, value: "!activator");

        MainPlugin.Instance.g_Main.Chicken_Spawned = true;
    }

    
    public static void SetGlowPlayer(CCSPlayerController player)
    {
        if (!player.IsValid(true) || player.PlayerPawn.Value == null)return;

        if(MainPlugin.Instance.g_Main.Glow_Spawned)
        {
            if (MainPlugin.Instance.g_Main.modelGlow == null || !MainPlugin.Instance.g_Main.modelGlow.IsValid)return;
            
            MainPlugin.Instance.g_Main.modelGlow.Spawnflags = 256u;
            MainPlugin.Instance.g_Main.modelGlow.Glow.GlowColorOverride = Helper.GetColorFromConfig(Configs.GetConfigData().Player_GlowColor);
            MainPlugin.Instance.g_Main.modelGlow.Glow.GlowRange = Configs.GetConfigData().Player_GlowRange;
            MainPlugin.Instance.g_Main.modelGlow.Glow.GlowTeam = -1;
            MainPlugin.Instance.g_Main.modelGlow.Glow.GlowType = Configs.GetConfigData().Player_GlowType?2:3;
            MainPlugin.Instance.g_Main.modelGlow.Glow.GlowRangeMin = 100;

            return;
        }

        MainPlugin.Instance.g_Main.modelRelay = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic")!;
        if (MainPlugin.Instance.g_Main.modelRelay == null)return;

        string modelName = player.PlayerPawn.Value!.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;
        MainPlugin.Instance.g_Main.modelRelay.DispatchSpawn();
        MainPlugin.Instance.g_Main.modelRelay.SetModel(modelName);
        MainPlugin.Instance.g_Main.modelRelay.Spawnflags = 256u;
        MainPlugin.Instance.g_Main.modelRelay.RenderMode = RenderMode_t.kRenderNone;

        MainPlugin.Instance.g_Main.modelGlow = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic")!;
        if (MainPlugin.Instance.g_Main.modelGlow == null)return;

        MainPlugin.Instance.g_Main.modelGlow.DispatchSpawn();
        MainPlugin.Instance.g_Main.modelGlow.SetModel(modelName);
        MainPlugin.Instance.g_Main.modelGlow.Spawnflags = 256u;
        MainPlugin.Instance.g_Main.modelGlow.Glow.GlowColorOverride = Helper.GetColorFromConfig(Configs.GetConfigData().Player_GlowColor);
        MainPlugin.Instance.g_Main.modelGlow.Glow.GlowRange = Configs.GetConfigData().Player_GlowRange;
        MainPlugin.Instance.g_Main.modelGlow.Glow.GlowTeam = -1;
        MainPlugin.Instance.g_Main.modelGlow.Glow.GlowType = Configs.GetConfigData().Player_GlowType?2:3;
        MainPlugin.Instance.g_Main.modelGlow.Glow.GlowRangeMin = 100;

        MainPlugin.Instance.g_Main.modelRelay.AcceptInput("FollowEntity", player.PlayerPawn.Value, MainPlugin.Instance.g_Main.modelRelay, "!activator");
        MainPlugin.Instance.g_Main.modelGlow.AcceptInput("FollowEntity", MainPlugin.Instance.g_Main.modelRelay, MainPlugin.Instance.g_Main.modelGlow, "!activator");

        MainPlugin.Instance.g_Main.Glow_Spawned = true;
    }
}
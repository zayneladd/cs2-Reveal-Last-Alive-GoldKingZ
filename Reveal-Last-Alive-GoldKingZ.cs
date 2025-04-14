using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Utils;
using Reveal_Last_Alive.Config;
using System.Drawing;

namespace Reveal_Last_Alive;

public class MainPlugin : BasePlugin
{
    public override string ModuleName => "Reveal Last Player Alive By Glow/Chicken";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    public static MainPlugin Instance { get; set; } = new();
    public Globals g_Main = new();
    
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);

        _ = Task.Run(async () => 
        {
            try
            {
                await Helper.DownloadMissingFiles();
                await Server.NextFrameAsync(() => CustomHooks.StartHook());
            }
            catch (Exception ex)
            {
                Helper.DebugMessage($"DownloadMissingFiles failed: {ex.Message}");
            }
        });


        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundEnd>(OnEventRoundEnd);
        RegisterEventHandler<EventPlayerDeath>(OnEventPlayerDeath);

        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);

    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        try
        {
            string filePath = Path.Combine(ModuleDirectory, "config/ServerPrecacheResources.txt");
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("//"))continue;
                manifest.AddResource(line);
                Helper.DebugMessage("ResourceManifest : " + line);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage(ex.Message);
        }
    }

    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (@event == null || Helper.IsWarmup())return HookResult.Continue;

        g_Main.B_Ready = true;

        Helper.ClearVariables(false);
        
        return HookResult.Continue;
    }
    public HookResult OnEventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

        g_Main.B_Ready = false;
        
        Helper.ClearVariables(false);
        
        return HookResult.Continue;
    }

    private HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event == null || !g_Main.B_Ready) return HookResult.Continue;
        
        var victim = @event.Userid;
        if (!victim.IsValid(true)) return HookResult.Continue;

        int aliveCT = Helper.GetPlayersController(IncludeBots: true, IncludeCT: true, IncludeT: false, IncludeSPEC: false).Count(p => p.PlayerPawn?.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE);
        int aliveT = Helper.GetPlayersController(IncludeBots: true, IncludeT: true, IncludeCT: false, IncludeSPEC: false).Count(p => p.PlayerPawn?.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE);

        bool shouldTrigger = (Configs.GetConfigData().RevealLastPlayerOnTeam == 1 && aliveCT == 1) || (Configs.GetConfigData().RevealLastPlayerOnTeam == 2 && aliveT == 1);

        if (shouldTrigger)
        {
            if (g_Main.Timer != null)
            {
                g_Main.Timer.Kill();
                g_Main.Timer = null!;
            }

            CCSPlayerController? lastPlayer = null;
        
            if (Configs.GetConfigData().RevealLastPlayerOnTeam == 1)
            {
                lastPlayer = Helper.GetPlayersController(IncludeBots: true, IncludeCT: true, IncludeT: false, IncludeSPEC: false).FirstOrDefault(p => p.PlayerPawn?.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE);
            }
            else if (Configs.GetConfigData().RevealLastPlayerOnTeam == 2)
            {
                lastPlayer = Helper.GetPlayersController(IncludeBots: true, IncludeT: true, IncludeCT: false, IncludeSPEC: false).FirstOrDefault(p => p.PlayerPawn?.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE);
            }

            if(lastPlayer.IsValid(true))
            {
                g_Main.Timer = AddTimer(1.0f, () => Helper.Start_Reveal(lastPlayer), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
                Helper.AdvancedServerPrintToChatAll(Localizer["PrintChatToAll.LastPlayer.Alive"], lastPlayer.PlayerName);
            }
        }
        else
        {
            if (g_Main.Timer != null)
            {
                g_Main.Timer.Kill();
                g_Main.Timer = null!;
            }
        }

        return HookResult.Continue;
    }
    
    public void OnMapEnd()
    {
        Helper.ClearVariables();
    }

    public override void Unload(bool hotReload)
    {
        CustomHooks.CleanUp();
        Helper.ClearVariables();
    }


    /* [ConsoleCommand("css_test", "test")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(!player.IsValid())return;
    } */
    
}
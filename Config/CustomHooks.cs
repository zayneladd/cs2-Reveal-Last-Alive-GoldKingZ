using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace Reveal_Last_Alive;

public static class CustomHooks
{
    public static CustomGameData? CustomFunctions { get; set; }
    internal static void StartHook()
    {
        CustomFunctions = new();
        CustomFunctions.CSoundOpGameSystem_SetSoundEventParamFunc_2.Hook( CSoundOpGameSystem_StartSoundEventFunc_2_PostHook, HookMode.Pre );
    }

    internal static void CleanUp()
    {
        CustomFunctions = new();
        CustomFunctions.CSoundOpGameSystem_SetSoundEventParamFunc_2.Unhook( CSoundOpGameSystem_StartSoundEventFunc_2_PostHook, HookMode.Pre );
    }

    public static HookResult CSoundOpGameSystem_StartSoundEventFunc_2_PostHook(DynamicHook hook)
    {
        var hash = hook.GetParam<uint>(3);

        if (hash == 0x2D8464AF)
        {
            hook.SetParam(3, 0xBD6054E9);
        }
        
        return HookResult.Continue;
    }
}
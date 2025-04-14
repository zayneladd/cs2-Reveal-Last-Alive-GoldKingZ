using CounterStrikeSharp.API.Core;

namespace Reveal_Last_Alive;
public class Globals
{
    public bool B_Ready = false;
    public bool Chicken_Spawned = false;
    public bool Glow_Spawned = false;
    public CChicken chicken = null!;
    public CChicken chickenGLOW = null!;

    public CDynamicProp modelRelay = null!;
    public CDynamicProp modelGlow = null!;

    public CounterStrikeSharp.API.Modules.Timers.Timer Timer = null!;

}
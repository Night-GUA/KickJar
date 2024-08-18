using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using static CloudGenerator;
using UnityEngine.Playables;
using Il2CppSystem.IO;

[assembly: AssemblyFileVersion(KickJar.Main.PluginVersion)]
[assembly: AssemblyInformationalVersion(KickJar.Main.PluginVersion)]
[assembly: AssemblyVersion(KickJar.Main.PluginVersion)]
namespace KickJar;

[BepInPlugin(PluginGuid, "KickJar", PluginVersion)]
[BepInProcess("Among Us.exe")]
public class Main : BasePlugin
{

    public static readonly string ModName = "KickJar"; // 咱们的模组名字
    public static readonly string ModColor = "#fffcbe"; // 咱们的模组颜色
    public static readonly string MainMenuText = ""; // 咱们模组的首页标语
    public const string PluginGuid = "com.Yu.KickJar"; //咱们模组的Guid
    public const string PluginVersion = "1.0.0"; //咱们模组的版本号
    public const string CanUseInAmongUsVer = "2024.8.13"; //智齿的AU版本
    public const int PluginCreation = 1;
    
    public static NormalGameOptionsV08 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
    public static HideNSeekGameOptionsV08 HideNSeekOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;
    
    public static System.Version version = System.Version.Parse(PluginVersion);
        
    public static string HostRealName = "";
    
    public static int ModMode { get; private set; } =
#if DEBUG
0;
#elif CANARY
        1;
    #else
    2;
#endif
    

    public static List<PlayerControl> JoinedPlayer = new();
    
    public Harmony Harmony { get; } = new Harmony(PluginGuid);

    public static BepInEx.Logging.ManualLogSource Logger;

    public static string GameRules =
        "罐子游戏规则：\n狼刀人冷却10秒,不得离开大厅只可在大厅内部刀人狼人能破坏反应堆、灭灯、和关闭食堂大门,狼要阻止船员拍桌直到所有好人被杀死,一开始先等好人出去才可以关门\n注意：狼会自动变成红名\n输入 /h 查看指令菜单";

    public static string GameHelp =
        "/r → 规则\n" +
        "/h → 再次唤出此菜单\n" +
        "/qt → 拉黑此房间（此操作无法撤销！）\n";
    
    public static IEnumerable<PlayerControl> AllPlayerControls => PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);
    
    public static Main Instance; //设置Main实例
    
    public static bool VisibleTasksCount = false;
    
    public static ConfigEntry<string> BetaBuildURL { get; private set; }
    public override void Load()//加载 启动！
    {
        Instance = this; //Main实例
        Logger = BepInEx.Logging.Logger.CreateLogSource("KickJar"); //输出前缀 设置！
        
        BetaBuildURL = Config.Bind("Other", "BetaBuildURL", "");
        
        if (Application.version == CanUseInAmongUsVer)
            Logger.LogInfo($"AmongUs Version: {Application.version}"); //牢底居然有智齿的版本？！
        else
            Logger.LogInfo($"游戏本体版本过低或过高,AmongUs Version: {Application.version}"); //牢底你的版本也不行啊
        //各组件初始化
        Harmony.PatchAll();
        if (ModMode != 0) ConsoleManager.DetachConsole();
        else ConsoleManager.CreateConsole();
        //模组加载好了标语
        Logger.LogInfo("========= KJ loaded! =========");
    }
}
public enum RoleTeam
{
    Crewmate,
    Impostor,
    Error
}
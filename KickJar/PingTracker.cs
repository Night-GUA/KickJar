using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Text;
using Il2CppSystem;
using Rewired.UI.ControlMapper;
using TMPro;

namespace KickJar;

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
internal class PingTrackerUpdatePatch
{
    private static float deltaTime;
    private static TextMeshPro pingTrackerCredential = null;
    private static AspectPosition pingTrackerCredentialAspectPos = null;
    public static float fps;

    private static void Postfix(PingTracker __instance)
    {
        // __instance.text.text = "";
        if (pingTrackerCredential == null)
        {
            var uselessPingTracker = UnityEngine.Object.Instantiate(__instance, __instance.transform.parent);
            pingTrackerCredential = uselessPingTracker.GetComponent<TextMeshPro>();
            UnityEngine.Object.Destroy(uselessPingTracker);
            pingTrackerCredential.alignment = TextAlignmentOptions.TopRight;
            pingTrackerCredential.color = new(1f, 1f, 1f, 0.7f);
            pingTrackerCredential.rectTransform.pivot = new(1f, 1f); // 中心を右上角に設定
            pingTrackerCredentialAspectPos = pingTrackerCredential.GetComponent<AspectPosition>();
            pingTrackerCredentialAspectPos.Alignment = AspectPosition.EdgeAlignments.RightTop;
        }

        if (pingTrackerCredentialAspectPos)
        {
            pingTrackerCredentialAspectPos.DistanceFromEdge =
                DestroyableSingleton<HudManager>.InstanceExists &&
                DestroyableSingleton<HudManager>.Instance.Chat.chatButton.gameObject.active
                    ? new(2.5f, 0f, -800f)
                    : new(1.8f, 0f, -800f);
        }

        __instance.text.alignment = TextAlignmentOptions.Top;
        // __instance.transform.localPosition = new Vector3(__instance.transform.localPosition.x,
        //     __instance.transform.localPosition.y + 10, __instance.transform.localPosition.z);

        StringBuilder sb = new();
        if(Main.ModMode == 0) sb.Append($"<color=#FFC0CB>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}</color>");
        else if(Main.ModMode == 1) sb.Append($"<color=#6A5ACD>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}C{Main.PluginCanary}</color>");
        else sb.Append($"<color={Main.ModColor}>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}</color>");
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = Mathf.Ceil(1.0f / deltaTime);
        var ping = AmongUsClient.Instance.Ping;
        
        // if (Toggles.ShowPing) sb.Append($"\r\n").Append(Utils.Utils.getColoredPingText(AmongUsClient.Instance.Ping) + "<size=60%>Ping</size></color>"); // 书写Ping
        // if (Toggles.ShowFPS) sb.Append(!Toggles.ShowPing ? $"\r\n" : " ").Append(Utils.Utils.getColoredFPSText(fps) + "<size=60%>FPS</size></color>"); // 书写FPS
        // if(Toggles.ShowServer) sb.Append((!Toggles.ShowFPS && !Toggles.ShowPing) ? $"\r\n" : " ").Append("  " + (GetPlayer.IsOnlineGame ? ServerName : GetString("Local")));
        
        __instance.text.text = Utils.getColoredPingText(AmongUsClient.Instance.Ping) +
                                                     "<size=60%>Ping</size></color>";
            __instance.text.text += " " + Utils.getColoredFPSText(fps) + "<size=60%>FPS</size></color>";
            
        
        // sb.Append($"\r\n")
        //     .Append(
        //         $"{Utils.Utils.getColoredPingText(ping)} <size=60%>Ping</size></color>  {Utils.Utils.getColoredFPSText(fps)} <size=60%>FPS</size></color>{"  " + (GetPlayer.IsOnlineGame ? ServerName : GetString("Local"))}");
        sb.Append($"\r\n").Append($"<color=#FFFF00>By</color> <color=#36FF5D>Yu</color>");
        
        pingTrackerCredential.gameObject.SetActive(__instance.gameObject.active);
        pingTrackerCredential.text = sb.ToString();
        if (GameSettingMenu.Instance?.gameObject?.active ?? false)
            pingTrackerCredential.text = "";
    }
}
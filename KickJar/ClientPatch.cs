using HarmonyLib;
using UnityEngine;

namespace KickJar;

[HarmonyPatch(typeof(MMOnlineManager), nameof(MMOnlineManager.Start))]
internal class MMOnlineManagerStartPatch
{
    public static void Postfix(MMOnlineManager __instance)
    {
        var FindGameButton = GameObject.Find("FindGameButton");
        if (FindGameButton)
        {
            FindGameButton.SetActive(false);
            var textObj1 = Object.Instantiate(FindGameButton.transform.FindChild("Text_TMP").GetComponent<TMPro.TextMeshPro>());
            var parentObj = FindGameButton.transform.parent.gameObject;
            textObj1.transform.position = new Vector3(1f, -0.3f, 0);
            textObj1.name = "CanNotFindGame";
            var message = $"<size=2>{Utils.ColorString(Color.red, "请注意，装载此模组无法加入其他游戏\n可能导致兼容问题，故我们封锁了您的搜索按钮")}</size>";
            new LateTask(() => { textObj1.text = message; }, 0.01f, "CanNotFindGame");
        }
        
        var JoinGameButton = GameObject.Find("JoinGameButton");
        if (JoinGameButton)
        {
            GameObject.Destroy(JoinGameButton);
            JoinGameButton.SetActive(false);
            // JoinGameButton.active
        }
    }
}
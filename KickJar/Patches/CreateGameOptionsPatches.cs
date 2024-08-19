using System;
using AmongUs.GameOptions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using HarmonyLib;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using System.IO;
using System.Reflection;
using TMPro;
using System.IO;
using Sentry.Internal;
using UnityEngine;
using UnityEngine.UI;
using BackgroundWorker = System.ComponentModel.BackgroundWorker;


namespace KickJar;

internal static class CreateGameOptionsPatches
{
    // [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
    // public static class CreateOptionsPicker_Awake
    // {
    //     public static void Postfix(CreateOptionsPicker __instance)
    //     {
    //         var template = GameObject.Find("ChatTypeOptionsCompact");
    //         var ChatTypeOptionsCompact = GameObject.Find("ChatTypeOptionsCompact");
    //         var Language_Selection = GameObject.Find("Language Selection");
    //         var RoomProtocolOptionsCompact = Object.Instantiate(template, template.transform.parent);
    //         RoomProtocolOptionsCompact.transform.localPosition += new Vector3(4.2f, 0, 0);
    //         ChatTypeOptionsCompact.transform.localPosition -= new Vector3(0.9f, 0, 0);
    //         Language_Selection.transform.localPosition -= new Vector3(1.2f, 0, 0);
    //         var RoomProtocolOptionsCompact_textobj = Object.Instantiate(RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").GetComponent<TextMeshPro>(),RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").GetComponent<TextMeshPro>().transform.parent);
    //         // RoomProtocolOptionsCompact_textobj.transform.localPosition =
    //         //     RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").localPosition;
    //         // RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").gameObject.SetActive(false);
    //         GameObject.Destroy(RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").gameObject);
    //         RoomProtocolOptionsCompact_textobj.name = "RoomModeTips";
    //         RoomProtocolOptionsCompact_textobj.gameObject.SetActive(true);
    //         // RoomProtocolOptionsCompact_textobj.text = "房间模式";
    //         new LateTask(() => { RoomProtocolOptionsCompact_textobj.text = "房间模式"; }, 0.01f, "RoomModeTipsSet");
    //     }
    // }
    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Start))]
    public static class CreateOptionsPicker_Start
    {
        public static void Postfix()
        {
            var template = GameObject.Find("ChatTypeOptionsCompact");
            var ChatTypeOptionsCompact = GameObject.Find("ChatTypeOptionsCompact");
            var Language_Selection = GameObject.Find("Language Selection");
            var RoomProtocolOptionsCompact = Object.Instantiate(template, template.transform.parent);
            RoomProtocolOptionsCompact.transform.localPosition += new Vector3(4.2f, 0, 0);
            ChatTypeOptionsCompact.transform.localPosition -= new Vector3(0.9f, 0, 0);
            Language_Selection.transform.localPosition -= new Vector3(1.2f, 0, 0);
            // var RoomProtocolOptionsCompact_textobj = Object.Instantiate(RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").GetComponent<TextMeshPro>(),RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").GetComponent<TextMeshPro>().transform.parent);
            // RoomProtocolOptionsCompact_textobj.transform.localPosition =
            //     RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").localPosition;
            // RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").gameObject.SetActive(false);
            // RoomProtocolOptionsCompact_textobj.name = "RoomModeTips";
            // RoomProtocolOptionsCompact_textobj.gameObject.SetActive(true);
            // RoomProtocolOptionsCompact_textobj.text = "房间模式";
            RoomProtocolOptionsCompact.name = "RoomProtocolOptionsCompact";
            var OldBackground = RoomProtocolOptionsCompact.transform.FindChild("Background");
            // var RoomProtocolOptionsCompact_Background = Object.Instantiate(OldBackground.gameObject,OldBackground.parent);
            var OldText_TMP = OldBackground.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
            OldText_TMP.text = "标准模式";
            // var RoomProtocolOptionsCompact_Background_TextTMP = Object.Instantiate(OldText_TMP, OldText_TMP.transform.parent);
            // RoomProtocolOptionsCompact_Background_TextTMP.text = "普通模式";
            // GameObject.Destroy(RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").gameObject);
            PassiveButton passiveButton = RoomProtocolOptionsCompact.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            // passiveButton.OnClick.AddListener();
            passiveButton.OnClick.AddListener(new Action(() =>
            {
                // Logger.Info("NORMAL","Create");
                Main.isModProtocol = !Main.isModProtocol;
                OldText_TMP.text = Main.isModProtocol ? "模组模式" : "标准模式";
                // RoomProtocolOptionsCompact_Background_TextTMP.text = "模组模式";
                // passiveButton.gameObject.SetActive(false);

            }));
            new LateTask(() => { RoomProtocolOptionsCompact.transform.FindChild("TitleText_TMP").GetComponent<TextMeshPro>().text = "房间模式"; }, 0.01f, "RoomModeTipsSet");
        }
    }

}
using AmongUs.GameOptions;
using Hazel;
using InnerNet;
using System;
using System.Text;
using UnityEngine;

namespace KickJar;

static class ExtendedPlayerControl
{
    public const MurderResultFlags SucceededFlags = MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost;
    public const MurderResultFlags ResultFlags = MurderResultFlags.Succeeded; //No need for DecisonByHost
    public static bool OwnedByHost(this InnerNetObject innerObject)
        => innerObject.OwnerId == AmongUsClient.Instance.HostId;
}
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    public static bool GetReady(this Player player)
    {
        PhotonHashtable property = player.CustomProperties;

        if (property.ContainsKey("Ready"))
            return (bool)property["Ready"];
        else
            return false;
    }

    public static void SetReady(this Player player, bool ready)
    {
        PhotonHashtable property = player.CustomProperties;
        property["Ready"] = ready;
        player.SetCustomProperties(property);
    }
}

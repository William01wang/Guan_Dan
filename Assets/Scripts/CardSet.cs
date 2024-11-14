using UnityEngine;

using System.Collections.Generic;

public class CardSet : MonoBehaviour
{
    private AssetBundle asset_bundle;
    private List<string> player0_card;
    private List<string> player1_card;
    private List<string> player2_card;
    private List<string> player3_card;



    void Start()
    {
        asset_bundle = AssetBundle.LoadFromFile("Assets/AssetBundles/cards");
        if (asset_bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle.");
        }
        player0_card = new List<string>();
        player1_card = new List<string>();
        player2_card = new List<string>();
        player3_card = new List<string>();
    }

    void Update()
    {

    }

    void UpdateCards()
    {
       
    }
}


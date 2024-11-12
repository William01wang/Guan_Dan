using UnityEngine;

using System.Collections.Generic;

public class Cards : MonoBehaviour
{
    private AssetBundle asset_bundle;
    private List<string> player0_card;
    private List<string> player1_card;
    private List<string> player2_card;
    private List<string> player3_card;


    void Start()
    {
        asset_bundle = AssetBundle.LoadFromFile("Assets/AssetBundles/cards");
        if (asset_bundle == null){
            Debug.LogError("Failed to load AssetBundle.");
        }
        player0_card = new List<string>();
        player1_card = new List<string>();
        player2_card = new List<string>();
        player3_card = new List<string>();

        player0_card.Add("xLittleJoker");
        player0_card.Add("xLittleJoker");
        UpdateCards();
    }

    void Update()
    {
    }

    void UpdateCards()
    {
        float d_hand = 0.02f;
        
        Vector3 location = new Vector3(0f, 0.1f, 0.9f);
        Vector3 card_location = location;
        card_location.x -= (player0_card.Count - 1) * d_hand / 2;
        for (int i = 0; i < player0_card.Count; i++)
        {
            GameObject cardmodel = asset_bundle.LoadAsset<GameObject>($"Card_{player0_card[i]}");
            if (cardmodel != null)
            {
                Instantiate(cardmodel, card_location, Quaternion.identity);
                card_location.x += d_hand;
                card_location.z += 0.001f;
            }
            else
            {
                Debug.LogError("Asset not found in the bundle.");
            }
        }

    }
}

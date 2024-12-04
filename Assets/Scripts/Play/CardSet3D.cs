using UnityEngine;
using System;
using System.Collections.Generic;

public class CardSet3D : MonoBehaviour
{
    private AssetBundle asset_bundle;
    private List<string> full_cardname;
    public float card_ontable_height = 0;
    public int whos_turn = 0;
    public Dictionary<string, int> player_rank = new Dictionary<string, int>
        {
            { "Player", 0 },
            { "PlayerLeft", 0 },
            { "PlayerRight", 0 },
            { "PlayerBack", 0 },
        };
    public int n_finished_player = 0;

    void Start()
    {
        asset_bundle = AssetBundle.LoadFromFile("Assets/AssetBundles/cards");
        if (asset_bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle.");
        }
        full_cardname = new List<string>();
        full_cardname.Add("xLittleJoker");
        full_cardname.Add("xBigJoker");
        foreach (string suit in new string[] { "Club", "Diamond", "Heart", "Spade" })
        {
            full_cardname.Add($"{suit}Ace");
            full_cardname.Add($"{suit}Jack");
            full_cardname.Add($"{suit}King");
            full_cardname.Add($"{suit}Queen");
            for (int i = 2; i <= 10; i++)
            {
                full_cardname.Add($"{suit}{i}");
            }
        }
        transform.Find("Player").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, 0f, 0f));
        transform.Find("PlayerLeft").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, 90f, 0f));
        transform.Find("PlayerRight").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, -90f, 0f));
        transform.Find("PlayerBack").GetComponent<Player3D>().SetDirection(Quaternion.Euler(0f, 180f, 0f));

        DistributeCards();
    }

    void Update()
    {
        if (n_finished_player >= 4)
        {
            DistributeCards();
        }
    }

    void DistributeCards()
    {
        System.Random random = new System.Random();
        foreach (string playername in new string[] { "Player", "PlayerLeft", "PlayerRight", "PlayerBack" })
        {
            Transform child = transform.Find(playername);
            foreach (Transform card in child.transform)
            {
                Destroy(card.gameObject);
            }
            for (int i = 0; i < 27; i++)
            {
                string selected_cardname = full_cardname[random.Next(full_cardname.Count)];
                child.GetComponent<Player3D>().AddCard(asset_bundle.LoadAsset<GameObject>($"Card_{selected_cardname}"), selected_cardname);
            }
            child.GetComponent<Player3D>().OrgHands();

            player_rank[playername] = 0;
        }
        n_finished_player = 0;
        whos_turn = 0;
    }
}


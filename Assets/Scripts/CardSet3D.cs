using UnityEngine;
using System;
using System.Collections.Generic;

public class CardSet3D : MonoBehaviour
{
    private AssetBundle asset_bundle;
    private List<string> full_cardname;

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
        foreach (string suit in new string[] { "Club", "Diamond", "Heart", "Spade" }){
            full_cardname.Add($"{suit}Ace");
            full_cardname.Add($"{suit}Jack");
            full_cardname.Add($"{suit}King");
            full_cardname.Add($"{suit}Queen");
            for (int i = 2; i <= 10; i++){
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

    }

    void DistributeCards(){  
        System.Random random = new System.Random();
        foreach (string playername in new string[] {"Player", "PlayerLeft", "PlayerRight", "PlayerBack"}){
            Transform child = transform.Find(playername);
            for (int i = 0; i < 27; i++){
                string selected_cardname = full_cardname[random.Next(full_cardname.Count)];
                GameObject card = AddCard(selected_cardname, new Vector3(0f, 0f, 0f), Quaternion.identity, child.transform);
                if (playername == "Player"){
                    card.GetComponent<Card3D>().SetState("OnHand");
                }
                else{
                    card.GetComponent<Card3D>().SetState("OnOthers");
                }
            }
            child.GetComponent<Player3D>().OrgHands();
        }
    }

    GameObject AddCard(string cardname, Vector3 card_location, Quaternion card_rotation, Transform transform){
        GameObject card = Instantiate(asset_bundle.LoadAsset<GameObject>($"Card_{cardname}"), card_location, card_rotation, transform);
        BoxCollider boxCollider = card.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.13f, 0.19f, 0.01f);
        card.AddComponent<Card3D>();
        return card;
    }
}


using UnityEngine;
using System.Collections.Generic;

public class Player3D : MonoBehaviour
{
    public Quaternion direction;
    public bool operate_play = false;
    private int card_cnt = 0;

    void Start()
    {
    }

    public void SetDirection(Quaternion direction)
    {
        this.direction = direction;
    }

    public void AddCard(GameObject card_object, string cardname)
    {
        GameObject card = Instantiate(card_object, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform);
        BoxCollider boxCollider = card.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.13f, 0.19f, 0.01f);
        card.AddComponent<Card3D>();
        card.GetComponent<Card3D>().cardname = $"{cardname}";
        if (gameObject.name == "Player")
        {
            card.GetComponent<Card3D>().state = Card3D.CardState.OnHand;
        }
        else
        {
            card.GetComponent<Card3D>().state = Card3D.CardState.OnOthers;
        }
        card_cnt++;
    }

    public void OrgHands()
    {
        if (card_cnt <= 0)
        {
            return;
        }
        float dx = 0.03f;
        Vector3 card_location = new Vector3(0f, 0.1f, 1.01f);
        Quaternion card_rotation = Quaternion.Euler(-20f, -1f, 0f);
        card_location.x -= (card_cnt - 1) * dx / 2;
        foreach (Transform child in transform)
        {
            Card3D card_script = child.GetComponent<Card3D>();
            if (card_script == null)
            {
                continue;
            }
            if (card_script.state == Card3D.CardState.Selected)
            {
                card_script.SelectedToOnHand();
            }
            if (card_script.state == Card3D.CardState.OnHand || card_script.state == Card3D.CardState.OnOthers)
            {
                child.position = direction * card_location;
                child.rotation = direction * card_rotation;
                card_location.x += dx;
            }
        }
    }

    private bool PlayCardAction(Transform child)
    {
        Card3D card_script = child.GetComponent<Card3D>();
        if (card_script != null && (card_script.state == Card3D.CardState.Selected || card_script.state == Card3D.CardState.OnOthers))
        {
            card_script.state = Card3D.CardState.OnTable;
            float angle = Random.Range(-5f, 5f);
            card_script.ontable_v = direction * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, -Mathf.Cos(angle * Mathf.Deg2Rad));
            card_script.ontable_v = card_script.ontable_v.Value * Random.Range(1f, 1.2f);
            child.rotation = direction * Quaternion.Euler(-90f, -angle, 0f);
            child.position = new Vector3(Mathf.Clamp(child.position.x, -0.87f, 0.87f), GetComponentInParent<CardSet3D>().card_ontable_height, Mathf.Clamp(child.position.z, -0.87f, 0.87f));
            GetComponentInParent<CardSet3D>().card_ontable_height += 0.0001f;
            return true;
        }
        return false;
    }

    private void NextTrun()
    {
        GetComponentInParent<CardSet3D>().whos_turn++;
        if (GetComponentInParent<CardSet3D>().whos_turn >= 4)
        {
            GetComponentInParent<CardSet3D>().whos_turn = 0;
        }
    }

    private void PlayCards()
    {
        int card_cnt_before = card_cnt;
        if ((gameObject.name == "Player" && GetComponentInParent<CardSet3D>().whos_turn != 0) ||
            (gameObject.name == "PlayerRight" && GetComponentInParent<CardSet3D>().whos_turn != 1) ||
            (gameObject.name == "PlayerBack" && GetComponentInParent<CardSet3D>().whos_turn != 2) ||
            (gameObject.name == "PlayerLeft" && GetComponentInParent<CardSet3D>().whos_turn != 3))
        {
            return;
        }
        if (card_cnt <= 0)
        {
            NextTrun();
            return;
        }
        if (gameObject.name == "Player")
        {
            if (!operate_play)
            {
                return;
            }
            operate_play = false;
            foreach (Transform child in transform)
            {
                if (PlayCardAction(child))
                {
                    card_cnt--;
                }
            }
        }
        else
        {
            List<string> play_cardnames = new List<string>();
            while (play_cardnames.Count <= 0)
            {
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && Random.Range(0f, 1f) < 0.2f)
                    {
                        play_cardnames.Add(child.GetComponent<Card3D>().cardname);
                    }
                }

            }
            int iter = play_cardnames.Count;
            while (play_cardnames.Count > 0)
            {
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<Card3D>().state == Card3D.CardState.OnOthers && child.GetComponent<Card3D>().cardname == play_cardnames[0])
                    {
                        if (PlayCardAction(child))
                        {
                            card_cnt--;
                            play_cardnames.RemoveAt(0);
                            break;
                        }
                    }
                }
                iter--;
                if (iter < 0)
                {
                    Debug.LogError($"play_cardnames incomplete {play_cardnames}");
                    break;
                }
            }
        }
        if (card_cnt_before > card_cnt)
        {
            if (card_cnt <= 0)
            {
                GetComponentInParent<CardSet3D>().n_finished_player++;
                GetComponentInParent<CardSet3D>().player_rank[gameObject.name] = GetComponentInParent<CardSet3D>().n_finished_player;
            }
            OrgHands();
            NextTrun();
        }
    }

    void Update()
    {
        PlayCards();
    }


}





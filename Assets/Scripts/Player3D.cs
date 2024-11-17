using UnityEngine;

public class Player3D : MonoBehaviour
{
    public Quaternion direction;
    public bool let_me_play = false;
    void Start()
    {
    }

    public void SetDirection(Quaternion direction)
    {
        this.direction = direction;
    }

    void Update()
    {
        if (let_me_play)
        {
            let_me_play = false;
            foreach (Transform child in transform)
            {
                Card3D card_script = child.GetComponent<Card3D>();
                if (card_script != null && card_script.state == Card3D.CardState.Selected)
                {
                    card_script.state = Card3D.CardState.OnTable;
                    float angle = Random.Range(-5f, 5f);
                    card_script.ontable_v = direction * new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, -Mathf.Cos(angle * Mathf.Deg2Rad));
                    card_script.ontable_v = card_script.ontable_v.Value * Random.Range(1f, 1.2f);
                    child.rotation = direction * Quaternion.Euler(-90f, -angle, 0f);
                    child.position = new Vector3(Mathf.Clamp(child.position.x, -0.87f, 0.87f), GetComponentInParent<CardSet3D>().card_ontable_height, Mathf.Clamp(child.position.z, -0.87f, 0.87f));
                    GetComponentInParent<CardSet3D>().card_ontable_height += 0.0001f;
                }
            }
            OrgHands();
        }
    }

    public void OrgHands()
    {
        int card_cnt = CntCard();
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
    public int CntCard()
    {
        int card_cnt = 0;
        foreach (Transform child in transform)
        {
            Card3D card_script = child.GetComponent<Card3D>();
            if (card_script.state == Card3D.CardState.OnHand || card_script.state == Card3D.CardState.OnOthers)
            {
                card_cnt++;
            }
        }
        return card_cnt;
    }
}

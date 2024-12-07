using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;

public class Card3D : MonoBehaviour
{
    public enum CardState
    {
        OnHand,
        Selected,
        OnTable,
        OnOthers
    }
    public CardState state;
    private Renderer object_renderer;
    private Color original_color;
    private Vector3? mouse_down_position = null;
    private Vector3 onhand_position;
    public Vector3? ontable_v = null;
    public string cardname = "";
    void Start()
    {
        object_renderer = GetComponent<Renderer>();
        original_color = object_renderer.material.color;
    }

    void Update()
    {
        SelectUpdate();
        OnTableUpdate(Time.deltaTime);
    }

    void OnTableUpdate(float deltaTime)
    {
        if (ontable_v != null)
        {
            if (state == CardState.OnTable)
            {
                Vector3 half_dv = deltaTime * 0.5f * ontable_v.Value.normalized;
                if (ontable_v.Value.magnitude < half_dv.magnitude)
                {
                    ontable_v = null;
                    return;
                }
                ontable_v -= half_dv;
                Vector3 new_position = transform.position + deltaTime * ontable_v.Value;

                if (new_position.x > 0.88f || new_position.x < -0.88f || new_position.z > 0.88f || new_position.z < -0.88f)
                {
                    new_position.x = Mathf.Clamp(new_position.x, -0.88f, 0.88f);
                    new_position.z = Mathf.Clamp(new_position.z, -0.88f, 0.88f);
                    ontable_v = null;
                }
                else
                {
                    if (ontable_v.Value.magnitude < half_dv.magnitude)
                    {
                        ontable_v = null;
                    }
                    else
                    {
                        ontable_v -= half_dv;
                    }
                }
                transform.position = new_position;
            }
            else
            {
                Debug.LogError("not on table but have speed");
            }
        }
    }

    void SelectUpdate()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Camera.main == null");
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool mouse_leave = false;
        if ((state == CardState.OnHand || state == CardState.Selected) && Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            object_renderer.material.color = new Color(1, original_color.g * 0.5f, original_color.b * 0.5f);
            if (Input.GetMouseButtonDown(0))
            {
                if (mouse_down_position == null)
                {
                    mouse_down_position = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0) && mouse_down_position != null)
            {
                mouse_leave = true;
            }
        }
        else
        {
            object_renderer.material.color = original_color;
            if (mouse_down_position != null)
            {
                mouse_leave = true;
            }
        }
        if (mouse_leave)
        {
            if (state == CardState.OnHand)
            {
                OnHandToSelected();
            }
            else if (state == CardState.Selected)
            {
                Vector3 d_mouse_position = Input.mousePosition - mouse_down_position.Value;
                if (d_mouse_position.y > 0 && d_mouse_position.magnitude > 10 && Mathf.Abs(d_mouse_position.x) < d_mouse_position.y)//检测鼠标运动是否为打出或者取消选中牌
                {
                    Player3D player = GetComponentInParent<Player3D>();
                    if (!CheckHandType(player).Equals("")) {//牌型检测函数
                        player.operate_play = true;
                    }
                }
                else
                {
                    SelectedToOnHand();
                }
            }
            mouse_down_position = null;
        }
    }

    string CheckHandType(Player3D player) 
    {
        List<string> cardsOnPlaying = new List<string>();
        foreach (Transform child in player.transform)//获取所有被选中的牌，牌在cardsOnplaying从小到大排序
        {
            Card3D card_script = child.GetComponent<Card3D>();
            if (card_script != null && card_script.state == Card3D.CardState.Selected)
            {
                cardsOnPlaying.Add(card_script.cardname);
            }
        }
        //cardsOnPlaying.ForEach(x => Debug.Log(x));  //用于debug查看试图打出的牌
        string[] suits = new string[cardsOnPlaying.Count];
        int[] points = new int[cardsOnPlaying.Count];
        for (int i = 0; i < cardsOnPlaying.Count; i++) //对选中牌的花色和大小进行标准化
        {
            string[] cur = cardsOnPlaying[i].Split("-");
            suits[i] = cur[0];
            if (cur[1] == "Jack")
            {
                points[i] = 11;
            }
            else if (cur[1] == "Queen")
            {
                points[i] = 12;
            }
            else if (cur[1] == "King")
            {
                points[i] = 13;
            }
            else if (cur[1] == "Ace")
            {
                points[i] = 14;
            }
            else if (cur[1] == "LittleJoker")
            {
                points[i] = 16;
            }
            else if (cur[1] == "BigJoker")
            {
                points[i] = 17;
            }
            else 
            {
                points[i] = int.Parse(cur[1]);
            }
        }
        //开始判断牌型
        if (cardsOnPlaying.Count == 1) //单牌
        {
            return $"1-{points[0]}";
        }
        else if (cardsOnPlaying.Count == 2) //对子
        {
            if (points[0]==points[1])
            {
                return $"2-{points[0]}";
            }
        }
        else if (cardsOnPlaying.Count == 3) //三同张
        {
            if (points[0] == points[1] && points[0] == points[2])
            {
                return $"3-{points[0]}";
            }
        }
        else if (cardsOnPlaying.Count == 5) //顺子、同花顺和三带对
        {
            bool flush = true;
            bool Straight = true;
            for (int i = 0; i < cardsOnPlaying.Count-1; i++) //判断是否同花
            {
                if (!suits[i].Equals(suits[i + 1])) 
                {
                    flush = false; break;
                }
            }
            for (int i = 0; i < cardsOnPlaying.Count - 1; i++)//判断是否顺子
            {
                if (points[i]!= points[i+1]-1)
                {
                    Straight = false; break;
                }
            }
            if (flush && Straight)
            {
                return $"9-{points[0]}";
            }
            else if (Straight) 
            {
                return $"4-{points[0]}";
            }
            int point1 = points[0], point2=-1, count1=1, count2=0;
            for (int i = 1; i < cardsOnPlaying.Count; i++) //判断是否三带对
            {
                if (points[i] == point1)
                {
                    count1++;
                }
                else if (point2==-1)
                {
                    point2 = points[i];
                    count2++;
                }
                else if (points[i] == point2)
                {
                    count2++;
                }
                else 
                {
                    break;
                }
            }
            if (count1 + count2 == 5) 
            {
                if (count1 == 3)
                {
                    return $"5-{point1}";
                }
                else 
                {
                    return $"5-{point2}";
                }
            }
        }
        else if (cardsOnPlaying.Count == 6) //三连对和三同连张
        {
            int point1 = points[0], point2 = -1, point3=-1, count1 = 1, count2 = 0, count3 = 0;
            for (int i = 1; i < cardsOnPlaying.Count; i++)
            {
                if (points[i] == point1)
                {
                    count1++;
                }
                else if (point2 == -1)
                {
                    point2 = points[i];
                    count2++;
                }
                else if (point3 == -1)
                {
                    point3 = points[i];
                    count3++;
                }
                else if (points[i] == point2)
                {
                    count2++;
                }
                else if (points[i] == point3)
                {
                    count3++;
                }
                else
                {
                    break;
                }
            }
            if ((count1 == 3) && (count2 == 3)) //判断是否三同连张
            {
                return $"7-{point1}";
            }
            else if ((count1 == 2) && (count2 == 2) && (count3 == 2)) //判断是否三连对
            {
                return $"6-{point1}";
            }
        }
        bool Bomb = true;
        for (int i = 0; i < cardsOnPlaying.Count-1; i++) //判断炸弹
        {
            if (points[i] != points[i + 1]) 
            {
                Bomb = false; break;
            }
        }
        if (Bomb) 
        {
            return $"8-{cardsOnPlaying.Count}-{points[0]}";
        }
        
        if (cardsOnPlaying.Count == 4) //判断四大天王
        {
            Bomb = true;
            for (int i = 0; i < cardsOnPlaying.Count - 1; i++)
            {
                if (points[i] < 16)
                {
                    Bomb = false; break;
                }
            }
            if (Bomb)
                {
                    return $"8-11-16";
                }
        }
        return "";
    }
    //牌型大小的设计：
    //使用string
    //单牌："1-牌点"
    //对子："2-牌点"
    //三同张："3-牌点"
    //顺子："4-最小牌牌点"
    //三带对："5-三的牌点"
    //三连对："6-最小牌牌点"
    //三同连张："7-最小牌牌点"
    //炸弹："8-炸弹长度-牌点" 其中四大天王长度视为11，牌点设为16
    //同花顺："9-牌点"
    void OnHandToSelected()
    {
        state = CardState.Selected;
        onhand_position = transform.position;
        transform.position += GetComponentInParent<Player3D>().direction * new Vector3(0, 0.1f * Mathf.Cos(20 * Mathf.Deg2Rad), -0.1f * Mathf.Sin(20 * Mathf.Deg2Rad));

    }
    public void SelectedToOnHand()
    {
        state = CardState.OnHand;
        transform.position = onhand_position;
    }
}



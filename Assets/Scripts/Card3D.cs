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
        OnOthers,
        CloneOnShowing
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
        if (state == CardState.CloneOnShowing)//用于展示的克隆牌不参与UPDATE
        {
            return;
        }
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
                    string handType = player.CheckHandType();
                    if (!handType.Equals("")) {//牌型检测函数
                        if (CardSet3D.CompareCard(handType, GetComponentInParent<CardSet3D>().GetBestCardOnDeck()) == 1) //检测打出的牌能否大过上家
                        {
                            GetComponentInParent<CardSet3D>().UpdateCardOnDeck(handType, "Player");
                            player.operate_play = true;
                        }
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



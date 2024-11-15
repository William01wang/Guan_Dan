using UnityEngine;

public class Card3D : MonoBehaviour
{
    private enum CardState
    {
        OnHand,
        Selected,
        OnTable,
        OnOthers
    }
    private CardState state;
    private Renderer object_renderer;
    private Color original_color;
    void Start()
    {
        Debug.Log("Card created");
        object_renderer = GetComponent<Renderer>();
        original_color = object_renderer.material.color;
    }

    public void SetState(string state_string){
        // Debug.Log(state_string);
        if (state_string == "OnHand"){
            this.state = CardState.OnHand;
        }
        else if (state_string == "Selected"){
            this.state = CardState.Selected;
        }
        else if (state_string == "OnTable"){
            this.state = CardState.OnTable;
        }
        else if (state_string == "OnOthers"){
            this.state = CardState.OnOthers;
        }
        else{
            Debug.LogError($"Card3D state {state_string} undefined");
        }
    }

    void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Camera.main == null");
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if ((state == CardState.OnHand || state == CardState.Selected) && Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            object_renderer.material.color = new Color(1, original_color.g * 0.5f, original_color.b * 0.5f);
            if (Input.GetMouseButtonDown(0)){
                if(state == CardState.OnHand){
                    state = CardState.Selected;
                }
                else if(state == CardState.Selected){
                    state = CardState.OnHand;
                }
                Debug.Log(state);
            }
        }
        else
        {
            object_renderer.material.color = original_color;
        }
    }
}

using UnityEngine;

public class Card : MonoBehaviour
{
    private enum CardState
    {
        OnHand,
        Selected,
        OnTable,
        OnHeap
    }
    private CardState state;
    private Renderer object_renderer;
    public Color original_color;
    void Start()
    {
        // Get the Renderer component of the object this script is attached to
        object_renderer = GetComponent<Renderer>();

        // Store the original material of the object
        original_color = object_renderer.material.color;

        state = CardState.OnHand;
    }

    void Update()
    {
        // Create a ray from the camera through the mouse position
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

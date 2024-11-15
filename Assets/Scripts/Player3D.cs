using UnityEngine;

public class Player3D : MonoBehaviour
{
    private Quaternion direction;
    void Start()
    {
    }

    public void SetDirection(Quaternion direction)
    {
        this.direction = direction;
    }

    void Update()
    {
    }

    public void OrgHands()
    {
        if (transform.childCount <= 0)
        {
            return;
        }
        float dx = 0.03f;
        Vector3 card_location = new Vector3(0f, 0.1f, 1.01f);
        Quaternion card_rotation = Quaternion.Euler(-20f, -1f, 0f);
        card_location.x -= (transform.childCount - 1) * dx / 2;
        foreach (Transform child in transform)
        {
            child.position = direction * card_location;
            child.rotation = direction * card_rotation;
            card_location.x += dx;
        }
    }
}

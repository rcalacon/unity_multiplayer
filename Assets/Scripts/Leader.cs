using UnityEngine;

public class Leader : MonoBehaviour
{
    public float speed = 10.0f;
    public Vector3 newPosition;

    void Start()
    {
        InvokeRepeating("SetRandomPos", 0, 1);
    }

    void SetRandomPos()
    {
        newPosition = new Vector3(Random.Range(-10f, 0f), Random.Range(1f, 3f), Random.Range(-5f, 5f));
    }

    // Update is called once per frame
    private void Update()
    {
        float step = speed * Time.deltaTime;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, newPosition, step);
    }
}

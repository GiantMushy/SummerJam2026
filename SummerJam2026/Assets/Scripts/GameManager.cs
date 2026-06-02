using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    public float gameTime = 0f;
    public float score = 0f;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

    }
}

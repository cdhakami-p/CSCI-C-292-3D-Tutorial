using UnityEngine;

public class Ball : MonoBehaviour
{

    private bool isRed;
    private bool is8Ball = false;
    private bool isCueBall = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsBallRed()
    {
        return isRed;
    }

    public bool IsCueBall()
    {
        return isCueBall;
    }

    public bool IsEightBall()
    {
        return is8Ball;
    }

    public void BallSetup(bool red)
    {
        isRed = red;
        if (isRed)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    public void MakeCueBall()
    {
        isCueBall = true;
    }

    public void MakeEightBall()
    {
        is8Ball = true;
        GetComponent<Renderer>().material.color = Color.black;
    }
}

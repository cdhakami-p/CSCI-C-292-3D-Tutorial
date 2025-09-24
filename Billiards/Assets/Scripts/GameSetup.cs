using UnityEngine;
using UnityEngine.Rendering;

public class GameSetup : MonoBehaviour
{

    int redBallsRemaining = 7;
    int blueBallsRemaining = 7;

    float ballRadius;
    float ballDiameter;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform cueBallPosition;
    [SerializeField] Transform headBallPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballRadius = ballPrefab.GetComponent<SphereCollider>().radius * 100f;
        ballDiameter = ballRadius * 2f;

        PlaceAllBalls();
    }

    void PlaceAllBalls()
    {
        PlaceCueBall();
        PlaceRandomBalls();
    }

    void PlaceCueBall()
    {
        GameObject ball = Instantiate(ballPrefab, cueBallPosition.position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeCueBall();
    }

    void PlaceEightBall(Vector3 position)
    {
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeEightBall();
    }

    void PlaceRandomBalls()
    {
        int NumInThisRow = 1;
        int rand;

        Vector3 firstInRowPosition = headBallPosition.position;
        Vector3 currentPosition = firstInRowPosition;

        void PlaceRedBall(Vector3 position)
        {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(true);
            redBallsRemaining--;
        }

        void PlaceBlueBall(Vector3 position)
        {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(false);
            blueBallsRemaining--;
        }

        // Outer loop for 5 rows
        for (int row = 0; row < 5; row++)
        {
            // Inner loop for number of balls in each row
            for (int ballInRow = 0; ballInRow < NumInThisRow; ballInRow++)
            {
                // Place the 8 ball in the center of the triangle
                if (row == 2 && ballInRow == 1)
                {
                    PlaceEightBall(currentPosition);
                }
                // Randomly place red or blue balls in the other positions
                else if (redBallsRemaining > 0 && blueBallsRemaining > 0)
                {
                    rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        PlaceRedBall(currentPosition);
                    }
                    else
                    {
                        PlaceBlueBall(currentPosition);
                    }
                }
                // If one color is out of balls, place the remaining color
                else if (redBallsRemaining > 0)
                {
                    PlaceRedBall(currentPosition);
                }
                else if (blueBallsRemaining > 0)
                {
                    PlaceBlueBall(currentPosition);
                }

                // Move to the next position in the row
                currentPosition += new Vector3(0, 0, -1).normalized * ballDiameter;
            }

            // Move to the start of the next row
            firstInRowPosition += Vector3.left * (Mathf.Sqrt(3) * ballRadius) + Vector3.forward * ballRadius;
            currentPosition = firstInRowPosition;
            NumInThisRow++;
        }
    }
}

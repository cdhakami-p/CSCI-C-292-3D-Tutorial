using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    enum CurrentPlayer { Player1, Player2 };

    CurrentPlayer currentPlayer;
    bool isWinningShotForPlayer1 = false;
    bool isWinningShotForPlayer2 = false;
    int player1BallsRemaining = 7;
    int player2BallsRemaining = 7;
    bool isWaitingForBallMovementToStop = false;
    bool willSwapPlayers = false;
    bool isGameOver = false;
    bool resetCueBall = false;

    bool ballPocketed = false;

    [SerializeField] float shotTimer = 3f;
    private float currentTimer;
    [SerializeField] float movementThreshold;

    [SerializeField] TextMeshProUGUI player1BallsText;
    [SerializeField] TextMeshProUGUI player2BallsText;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI messageText;

    [SerializeField] GameObject restartButton;

    [SerializeField] Transform headPosition;

    [SerializeField] Camera cueStickCam;
    [SerializeField] Camera overheadCam;
    Camera currentCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
        currentCam = cueStickCam;
        currentTimer = shotTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaitingForBallMovementToStop && !isGameOver)
        { 
            currentTimer -= Time.deltaTime;
            if (currentTimer > 0)
            {
                return;
            }

            bool allStopped = true;
            
            foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
            {
                if (ball.GetComponent<Rigidbody>().linearVelocity.magnitude >= movementThreshold)
                {
                    allStopped = false;
                    break;
                }
            }

            if (allStopped)
            {
                isWaitingForBallMovementToStop = false;

                if (resetCueBall)
                {
                    ResetCueBall();
                    resetCueBall = false;
                }

                if (willSwapPlayers || !ballPocketed)
                {
                    NextPlayerTurn();
                }
                else
                {
                    SwitchCameras();
                }

                currentTimer = shotTimer;
                ballPocketed = false;
            }
        }
    }

    public void SwitchCameras()
    {
        if (currentCam == cueStickCam)
        {
            cueStickCam.enabled = false;
            overheadCam.enabled = true;
            currentCam = overheadCam;
            isWaitingForBallMovementToStop = true;
        }
        else
        {
            currentCam.enabled = false;
            cueStickCam.enabled = true;
            currentCam = cueStickCam;

            currentCam.gameObject.GetComponent<CameraController>().ResetCamera();
        }
    }

    public void restartGame()
    {
        SceneManager.LoadScene(0);
    }

    bool Scratch()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            if (isWinningShotForPlayer1)
            {
                ScratchOnWinningShot("Player 1");

                return true;
            }
            else
            {
                if (isWinningShotForPlayer2)
                {
                    ScratchOnWinningShot("Player 2");

                    return true;
                }
            }
        }
        willSwapPlayers = true;
        return false;
    }

    void EarlyEightBall()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            Lose("Player 1 hit the 8 ball in too early and lost!");
        }
        else
        {
            Lose("Player 2 hit the 8 ball in too early and lost!");
        }
    }

    void ScratchOnWinningShot(string player)
    {
        Lose(player + " scratched on the winning shot and lost!");
    }

    bool CheckBall(Ball ball)
    {
        if (ball.IsCueBall())
        {
            if (Scratch())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (ball.IsEightBall())
        {
            if (currentPlayer == CurrentPlayer.Player1)
            {
                if (isWinningShotForPlayer1)
                {
                    Win("Player 1 ");
                    return true;
                }
                else
                {
                    EarlyEightBall();
                }
            }
            else
            {
                if (isWinningShotForPlayer2)
                {
                    Win("Player 2 ");
                    return true;
                }
                else
                {
                    EarlyEightBall();
                }
            }
        } else
        {
            if (ball.IsBallRed())
            {
                player1BallsRemaining--;
                player1BallsText.text = "Player 1 Balls Remaining: " + player1BallsRemaining;
                if (player1BallsRemaining <= 0)
                {
                    isWinningShotForPlayer1 = true;
                    //NoMoreBalls(CurrentPlayer.Player1);
                }
                if (currentPlayer != CurrentPlayer.Player1)
                {
                    //NextPlayerTurn();
                    isWaitingForBallMovementToStop = true;
                    willSwapPlayers = true;
                    resetCueBall = true;
                }
            }
            else
            {
                player2BallsRemaining--;
                player2BallsText.text = "Player 2 Balls Remaining: " + player2BallsRemaining;
                if (player2BallsRemaining <= 0)
                {
                    isWinningShotForPlayer2 = true;
                    //NoMoreBalls(CurrentPlayer.Player2);
                }
                if (currentPlayer != CurrentPlayer.Player2)
                {
                    //NextPlayerTurn();
                    isWaitingForBallMovementToStop = true;
                    willSwapPlayers = true;
                    resetCueBall = true;
                }
            }
        }

        return true;
    }

    void Lose(string message)
    {
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        restartButton.SetActive(true);
    }

    void Win(string player)
    {
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = player + "has won!";
        restartButton.SetActive(true);
    }

    void NextPlayerTurn()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            currentPlayer = CurrentPlayer.Player2;
            currentTurnText.text = "Current Turn: Player 2";
        }
        else
        {
            currentPlayer = CurrentPlayer.Player1;
            currentTurnText.text = "Current Turn: Player 1";
        }

        willSwapPlayers = false;
        SwitchCameras();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            ballPocketed = true;
            if (CheckBall(other.gameObject.GetComponent<Ball>()))
            {
                Destroy(other.gameObject);
            }
            else
            {
                other.gameObject.transform.position = headPosition.position;
                other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }

    void ResetCueBall()
    {
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
        {
            if (ball.GetComponent<Ball>().IsCueBall())
            {
                ball.transform.position = headPosition.position;
                ball.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                break;
            }
        }
    }
}

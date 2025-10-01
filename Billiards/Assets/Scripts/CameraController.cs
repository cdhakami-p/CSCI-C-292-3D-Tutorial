using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{

    Transform cueBall;

    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] float downAngle;
    [SerializeField] float power;
    [SerializeField] GameObject cueStick;

    private float horizontalInput;

    private bool isTakingShot = false;
    [SerializeField] float maxDrawDistance;
    private float saveMousePosition;

    GameManager gameManager;

    [SerializeField] TextMeshProUGUI powerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
        {
            if (ball.GetComponent<Ball>().IsCueBall())
            {
                cueBall = ball.transform;
                break;
            }
        }

        ResetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (cueBall != null && !isTakingShot)
        {
            horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            transform.RotateAround(cueBall.position, Vector3.up, horizontalInput);
        }

        Shoot();
    }

    public void ResetCamera()
    {
        transform.position = cueBall.position + offset;
        transform.LookAt(cueBall.position);
        transform.localEulerAngles = new Vector3(downAngle, transform.localEulerAngles.y, 0f);

        cueStick.SetActive(true);
    }

    void Shoot()
    {
        if (gameObject.GetComponent<Camera>().enabled)
        {
            if (Input.GetButtonDown("Fire1") && !isTakingShot)
            {
                isTakingShot = true;
                saveMousePosition = 0f;
            }
            else if (isTakingShot)
            {
                if (saveMousePosition + Input.GetAxis("Mouse Y") <= 0)
                {
                    saveMousePosition += Input.GetAxis("Mouse Y");
                    if (saveMousePosition <= maxDrawDistance)
                    {
                        saveMousePosition = maxDrawDistance;
                    }
                    
                    float powerValue = ((saveMousePosition - 0) / (maxDrawDistance - 0)) * (100-0) + 0;
                    int powerValueInt = Mathf.RoundToInt(powerValue);
                    powerText.text = "Power: " + powerValueInt.ToString() + "%";
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    Vector3 hitDirection = transform.forward;
                    hitDirection = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;

                    cueBall.gameObject.GetComponent<Rigidbody>().AddForce(hitDirection * power * Mathf.Abs(saveMousePosition), ForceMode.Impulse);
                    cueStick.SetActive(false);
                    gameManager.SwitchCameras();
                    isTakingShot = false;
                }
            }
        }
    }
}

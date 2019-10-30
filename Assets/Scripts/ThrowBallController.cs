using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThrowBallController : MonoBehaviour
{
    //instance of singleton
    private static ThrowBallController throwPaperInstance;
    //ball properties
    [Header("Ball Properties")]
    List<Vector3> ballPos = new List<Vector3>();
    List<float> ballTime = new List<float>();
    private GameObject currentBall;
    private GameObject previousBall;
    public bool isCurveReady = false;
    private float factor = 230.0f;
    private float startTime;
    public Vector3 startPos;
    public GameObject ball;
    private Transform ballChildObj;
    public Vector3 minThrow;
    public Vector3 maxThrow;
    //Bool Logics
    private bool isCalculatingDir = false;
    private bool isGameStart = true;
    private bool isStartRoate = false;
    private bool ObjectMouseDown = false;
    //Vectors and direction variables
    private Vector3 lastPos;
    private Vector3 lastBallPosition = Vector3.zero;
    private Vector3 v3Offset;
    float lastAngel = 0f;
    float rotationSpeed = 2.5f;
    float totalX = 0f, totalY = 0f;
    public int angleDirection = 0;
    int DirectionL = 0, DirectionR = 0;
    //Game Objects
    private Plane plane;
    public GameObject linkedObject; // use for a 2d aim object
    public Transform target;
    public ParticleSystem curveParticle;

    //set instance to this object only happends once per game startup 
    void Awake()
    {
        throwPaperInstance = this;
    }


    public static ThrowBallController Instance
    {
        get
        {
            return throwPaperInstance;
        }
    }


    //Time to spawn the ball calling GetBallNow 
    void Start()
    {
        StartCoroutine(GetBallNow());
    }

    //spawn new ball
    IEnumerator GetBallNow()
    {
        Debug.Log("Spawning new Ball..");
        //wait for few seconds then spawn ball when game starts
        yield return new WaitForSeconds(3f);
        if (previousBall != null)
        {
            //destroy old ball
            Destroy(previousBall);
        }
        //spawn new ball
        currentBall = Instantiate(ball, ball.transform.position, Quaternion.identity) as GameObject;
        //disable collider just in case
        currentBall.GetComponent<Collider>().enabled = false;
    }


    void Update()
    {
        //if current ball exist, lets follow it with this collider
        if (currentBall)
        {
            transform.position = currentBall.transform.position;
        }
        //if current ball exist and we are calulating direction
        if (currentBall && isCalculatingDir)
        {
            transform.position = currentBall.transform.position;
            float angle = 0f;
            if (ballChildObj != null)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 playerPos = Camera.main.WorldToScreenPoint(ballChildObj.localPosition);
                mousePos.x = mousePos.x - playerPos.x;
                mousePos.y = mousePos.y - playerPos.y;
                angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            }
            if (Vector3.Distance(currentBall.transform.position, lastBallPosition) > 0f)
            {

                if (!isStartRoate)
                {
                    Invoke("ResetBall", 0.01f);
                }
                else
                {
                    Vector3 dir = (currentBall.transform.position - lastBallPosition);
                    if (dir.x > dir.y)
                    {
                        totalX += 0.12f;
                    }
                    else
                    {
                        if (dir.x != dir.y)
                        {
                            totalY += 0.12f;
                        }
                    }
                    if (totalX >= 2 && totalY >= 2)
                    {
                        isCurveReady = true;

                        //if you want to add a paricle effects when the ball is spining and ready for a curve throw
                        if (curveParticle != null)
                        {
                            curveParticle.gameObject.SetActive(true);
                            if (curveParticle.isStopped)
                            {
                                curveParticle.Play();
                            }
                            curveParticle.transform.position = currentBall.transform.position;
                        }

                    }
                }

                isStartRoate = true;
                if (angle > 0)
                {
                    if (lastAngel >= angle)
                    {
                        DirectionR++;
                    }
                    else
                    {
                        DirectionL++;
                    }
                }
                else
                {
                    if (lastAngel >= angle)
                    {
                        DirectionR++;
                    }
                    else
                    {
                        DirectionL++;
                    }
                }

                if (DirectionL < DirectionR)
                {
                    angleDirection = 1;
                    if (ballChildObj != null)
                        ballChildObj.transform.Rotate(new Vector3(0, 0, 40f));
                }
                else
                {
                    angleDirection = -1;
                    if (ballChildObj != null)
                        ballChildObj.transform.Rotate(new Vector3(0, 0, -40f));
                }

            }
            else
            {

                if (isStartRoate == true)
                {
                    StartCoroutine(StopRotation(angleDirection, 0.5f));
                }
            }

            lastAngel = angle;
            lastBallPosition = currentBall.transform.position;
        }
    }

    private void ResetBall()
    {
        if (!isStartRoate)
        {
            totalX = 0f;
            totalY = 0f;
            DirectionL = 0;
            DirectionR = 0;
            isCurveReady = false;
            startPos = currentBall.transform.position;
            ballPos.Clear();

            ballTime.Clear();
            ballTime.Add(Time.time);

            ballPos.Add(currentBall.transform.position);

            //if you have a particle attached then lets reset everything and stop it / disble it
            if (curveParticle != null)
            {
                curveParticle.Stop();
                curveParticle.gameObject.SetActive(false);
            }

            ThrowBallController.Instance.isCurveReady = false;
        }
    }

    public bool IsGameStart
    {
        get
        {
            return isGameStart;
        }
        set
        {
            isGameStart = value;
        }
    }


    IEnumerator StopRotation(int direction, float t)
    {
        isStartRoate = false;
        float rate = 1.0f / t;
        float i = 0f;
        while (i < 1.0f)
        {
            i += rate * Time.deltaTime;
            if (!isStartRoate)
            {
                if (direction == 1)
                {
                    if (ballChildObj != null)
                        ballChildObj.transform.Rotate(new Vector3(0, 0, 40f) * (Mathf.Lerp(rotationSpeed * totalX, 0, i)) * Time.deltaTime);
                }
                else
                {
                    if (ballChildObj != null)
                        ballChildObj.transform.Rotate(new Vector3(0, 0, -40f) * (Mathf.Lerp(rotationSpeed * totalX, 0, i)) * Time.deltaTime);
                }
            }

            yield return 0;
        }

        if (!isStartRoate)
        {

            totalX = 0f;
            totalY = 0f;
            DirectionL = 0;
            DirectionR = 0;
            isCurveReady = false;
            rotationSpeed = 100f;

            //if you have a particle attached then lets reset everything and stop it / disble it
            if (curveParticle != null)
            {
                curveParticle.Stop();
                curveParticle.gameObject.SetActive(false);
            }
        }
    }

    //when player click on ball / touch if mobile
    void OnMouseDown()
    {
        if (currentBall == null)
        {
            return;
        }
        plane.SetNormalAndPosition(Camera.main.transform.forward, currentBall.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        v3Offset = currentBall.transform.position - ray.GetPoint(dist);
        ObjectMouseDown = true;

        if (!currentBall || !isGameStart)
            return;
        ballPos.Clear();

        ballTime.Clear();
        ballTime.Add(Time.time);

        ballPos.Add(currentBall.transform.position);

        totalX = 0f;
        totalY = 0f;

        DirectionL = 0;
        DirectionR = 0;

        isCurveReady = false;

        isCalculatingDir = true;
        currentBall.SendMessage("isThrow", true, SendMessageOptions.RequireReceiver);

        ballChildObj = currentBall.transform.Find("Ball");
        StartCoroutine(GettingDirection());

    }

    IEnumerator GettingDirection()
    {
        while (isCalculatingDir)
        {
            startTime = Time.time;
            lastPos = startPos;
            startPos = Camera.main.WorldToScreenPoint(currentBall.transform.position);
            startPos.z = currentBall.transform.position.z - Camera.main.transform.position.z;
            startPos = Camera.main.ScreenToWorldPoint(startPos);

            yield return new WaitForSeconds(0.01f);
        }
    }

    //when player is dragging the ball with mouse / touch if mobile
    void OnMouseDrag()
    {
        if (ObjectMouseDown == true)
        {

            if (!currentBall)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            plane.Raycast(ray, out dist);
            Vector3 v3Pos = ray.GetPoint(dist);
            v3Pos.z = currentBall.transform.position.z;
            v3Offset.z = 0;
            currentBall.transform.position = v3Pos + v3Offset;


            if (ballPos.Count > 0)
            {


                if (ballPos.Count <= 4)
                {
                    if (Vector3.Distance(currentBall.transform.position, ballPos[ballPos.Count - 1]) >= 0.01f)
                    {
                        ballTime.Add(Time.time);
                        ballPos.Add(currentBall.transform.position);

                    }
                }
                else
                {
                    if (Vector3.Distance(currentBall.transform.position, ballPos[ballPos.Count - 1]) >= 0.01f)
                    {
                        ballTime.RemoveAt(0);
                        ballPos.RemoveAt(0);
                        ballTime.Add(Time.time);
                        ballPos.Add(currentBall.transform.position);
                    }
                }

            }
            else
            {
                ballPos.Add(currentBall.transform.position);
            }

            if (linkedObject != null)
            {
                linkedObject.transform.position = v3Pos + v3Offset;
            }
        }
    }

    //when player release the mouse to flick or / touch if mobile
    void OnMouseUp()
    {
        //if you have a particle attached then lets reset everything and stop it / disble it
        if (curveParticle != null)
        {
            curveParticle.Stop();
            curveParticle.gameObject.SetActive(false);
        }

        isCalculatingDir = false;

        if (!currentBall || !isGameStart)
            return;

        var endPos = Input.mousePosition;
        endPos.z = currentBall.transform.position.z - Camera.main.transform.position.z;
        endPos = Camera.main.ScreenToWorldPoint(endPos);

        int ballPositionIndex = ballPos.Count - 2;

        if (ballPositionIndex < 0)
            ballPositionIndex = 0;

        Vector3 force = currentBall.transform.position - ballPos[ballPositionIndex];

        if (Vector3.Distance(lastPos, startPos) <= 0.0f)
        {
            currentBall.SendMessage("ResetBall", SendMessageOptions.RequireReceiver);
            return;
        }

        //if downside
        if (currentBall.transform.position.y <= ballPos[ballPositionIndex].y)
        {
            currentBall.SendMessage("ResetBall", SendMessageOptions.RequireReceiver);

            return;
        }

        //if not swipe
        if (force.magnitude < 0.02f)
        {
            currentBall.SendMessage("ResetBall", SendMessageOptions.RequireReceiver);
            return;
        }
        force.z = force.magnitude * 10;
        //force /= (Time.time - ballTime[ballPositionIndex]);
        force.y *= 4f;
        //force.x /= 2f;

        force.x = Mathf.Clamp(force.x, minThrow.x, maxThrow.x);
        force.y = Mathf.Clamp(force.y, minThrow.y, maxThrow.y);
        force.z = Mathf.Clamp(force.z, minThrow.z, maxThrow.z);

        //send message ball was thrown
        currentBall.SendMessage("isThrow", true, SendMessageOptions.RequireReceiver);


        if (isCurveReady)
        {
            force.z -= 0.1f;
            if (angleDirection == 1)
            {
                if (force.z < 2.3f)
                    force.z = 2.3f;
                currentBall.SendMessage("SetCurve", -factor);
            }
            else
            {
                if (force.z < 2.3f)
                    force.z = 2.3f;

                currentBall.SendMessage("SetCurve", factor);
            }
        }

        //get rigidbody
        Rigidbody ballRigidbody = currentBall.GetComponent<Rigidbody>();
        //enable collider
        currentBall.GetComponent<Collider>().enabled = true;
        ballRigidbody.useGravity = true;
        Debug.Log(force * factor);
        ballRigidbody.AddForce(force * factor);

        StartCoroutine(GetBallNow());

        previousBall = currentBall;
        currentBall = null;
    }

    public bool IsGettingDirection
    {
        get
        {
            return isCalculatingDir;
        }
        set
        {
            isCalculatingDir = value;
        }
    }


}

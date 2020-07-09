using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static int selectedIndex = 0;

    static int globalIndex = 0;
    int index;

    public enum Direction { Up, Down, Right, Left, None };
    public enum Gestures { Yes, No }
    Direction currentDirection;

    public Dictionary<Collider2D, int> HandContacts = new Dictionary<Collider2D, int>();
    Rigidbody2D heldObject;

    public enum Hands { Right, Left };

    [SerializeField] private Rigidbody2D RightHand;
    [SerializeField] private Rigidbody2D LeftHand;
    [SerializeField] private LineRenderer RLine;
    [SerializeField] private LineRenderer LLine;
    [SerializeField] private Transform body;
    [SerializeField] private Transform Eyes;
    [SerializeField] private SpriteRenderer gestureIcon;
    [SerializeField] private Animator gestureAnimator;


    [SerializeField] private Sprite YesIcon;
    [SerializeField] private Sprite NoIcon;

    Vector2 RRestingHand;
    Vector2 LRestingHand;

    Vector3 mousePositionInWorld;
    Vector2 movementVector;


    Camera cam;

    public float handFollowSpeed = 0.2f;
    public float movementForce = 500f;
    public float handMouseInfluence = 0.2f;
    public float maxHandRange = 2f;
    public float disarmRange = 2f;
    public float headSensitivity = 0.2f;
    public float throwStrength = 5f;
    public float gestureCooldown = 1f;
    public float gestureInterval = 0.2f;

    [Header("Punch Setting")]
    public float punchLength = 1;
    public float punchStrength = 2;

    Hands lastPunch;
    float punchTime = 0;
    Vector3 punchPoint;

    Rigidbody2D rb;
    private void Awake()
    {
        index = globalIndex++;

        rb = GetComponent<Rigidbody2D>();
        RRestingHand = RightHand.transform.localPosition;
        LRestingHand = LeftHand.transform.localPosition;
    }
    private void Start()
    {
        RightHand.transform.parent = null;
        LeftHand.transform.parent = null;
        cam = CameraController.instance.cam;
        CameraController.instance.SetTarget(transform);
    }

    private void FixedUpdate()
    {
        if(index == selectedIndex)
        {
            rb.AddForce(movementForce * movementVector);
        }

        SetHandPosition(RightHand, RRestingHand, RLine.transform.position, Hands.Right);
        SetHandPosition(LeftHand, LRestingHand, LLine.transform.position, Hands.Left);

        heldObject?.MovePosition(Vector3.Lerp(RightHand.position, LeftHand.position, 0.5f));
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(heldObject)
            {
                heldObject.AddForce(GetDirectionFromCursor(heldObject.position) * throwStrength, ForceMode2D.Impulse);
                heldObject = null;
            }
            else if(HandContacts.Count > 0)
            {
                foreach (var item in HandContacts)
                {
                    heldObject = item.Key.attachedRigidbody;
                    break;
                }
            }
        }

        movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f);
        Eyes.localPosition = Vector3.ClampMagnitude(GetDirectionFromCursor(body.position) * headSensitivity, 0.4f);
        UpdateLines();


        if(Input.GetMouseButtonDown(0) && punchTime <= 0)
        {
            lastPunch = lastPunch == Hands.Left ? Hands.Right : Hands.Left;
            punchPoint = mousePositionInWorld;
            punchTime = punchLength;
        }

        SetMouseWorldPosition();

        Vector3 mouseVector = GetDirectionFromCursor(transform.position);

        CameraController.instance.SetOffset(-mouseVector);
        currentDirection = DetectDirection();

        if(Time.time >= cooldownTime)
        {
            if (currentDirection == Direction.Left || currentDirection == Direction.Right)
            {
                StartCoroutine(DetectGesture(true));
            }
            else if(currentDirection == Direction.Up || currentDirection == Direction.Down)
            {
                StartCoroutine(DetectGesture(false));
            }
        }
    }

    public void UpdateLines()
    {
        RLine.SetPosition(0, RLine.transform.position);
        LLine.SetPosition(0, LLine.transform.position);
        RLine.SetPosition(1, RightHand.transform.position);
        LLine.SetPosition(1, LeftHand.transform.position);
    }

    private void OnValidate()
    {
        UpdateLines();
    }

    public void SetHandPosition(Rigidbody2D hand, Vector3 restingPosition, Vector3 shoulderPosition, Hands side)
    {
        Vector3 target;

        float influence = Mathf.InverseLerp(disarmRange, 0, Vector3.Distance(mousePositionInWorld, body.position));
        target = Vector3.Lerp(shoulderPosition + Vector3.ClampMagnitude(GetDirectionFromCursor(shoulderPosition), maxHandRange), transform.position + restingPosition, influence);

        if(lastPunch == side) target -= (transform.position + restingPosition - punchPoint).normalized * punchStrength * punchTime;
        hand.MovePosition(Vector3.Lerp(hand.position, target, handFollowSpeed * Time.deltaTime));
        punchTime = Mathf.Max(0, punchTime - Time.deltaTime);
    }

    public Vector3 GetDirectionFromCursor(Vector3 startingPoint)
    {
        return  mousePositionInWorld - startingPoint;
    }

    public void SetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        mousePositionInWorld = cam.ScreenToWorldPoint(mousePos);
    }

    public void Emote(Gestures emotion)
    {
        switch (emotion)
        {
            case Gestures.Yes:
                gestureIcon.sprite = YesIcon;
                break;
            case Gestures.No:
                gestureIcon.sprite = NoIcon;
                break;
            default:
                break;
        }
        gestureAnimator.SetTrigger("Emote");
    }

    Direction DetectDirection()
    {
        if (Mathf.Max(Mathf.Abs(Eyes.localPosition.y), Mathf.Abs(Eyes.localPosition.x)) < 0.2f) return Direction.None;

        if(Mathf.Abs(Eyes.localPosition.y) > Mathf.Abs(Eyes.localPosition.x))
        {
            return Eyes.localPosition.y > 0 ? Direction.Up : Direction.Down;
        }
        else
        {
            return Eyes.localPosition.x > 0 ? Direction.Right : Direction.Left;
        }
    }

    float cooldownTime = 0;

    IEnumerator DetectGesture(bool horizontal)
    {
        ///Debug.Log("Detecting gesture...");
        cooldownTime = Time.time + 1000;
        Direction last = currentDirection;
        for (int i = 0; i < 3; i++)
        {
            float interval = gestureInterval;
            while((currentDirection == Direction.None || last == currentDirection) && interval > 0)
            {
                interval -= Time.deltaTime;
                yield return null;
            }
            if((horizontal && (currentDirection == Direction.Up || currentDirection == Direction.Down))
                || (!horizontal && (currentDirection == Direction.Right || currentDirection == Direction.Left)) 
                || interval <= 0)
            {
                ///Debug.Log("Detection cancelled");
                cooldownTime = Time.time;
                yield break;
            }
            last = currentDirection;
        }
        Emote(horizontal ? Gestures.No : Gestures.Yes);
        cooldownTime = Time.time + gestureCooldown;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(mousePositionInWorld, 0.1f);
        Gizmos.DrawWireSphere(body.position, disarmRange);
    }
}

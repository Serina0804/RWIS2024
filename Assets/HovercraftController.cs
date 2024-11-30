using UnityEngine;
using System.Collections;

public class HovercraftController : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float rotationSpeed = 200f;
    public float changeDirectionInterval = 2f;
    public Texture targetTexture;
    public int jumpCount = 3;
    public Transform[] targetPoints; // 変更: 目標地点の配列

    private Rigidbody rb;
    private float timer = 0f;
    private int targetIndex = 0; // 変更: 目標地点のインデックス
    private bool targetFound = false;
    private int jumpCounter = 0; // ジャンプ回数のカウンター

    Vector3 boxCenter;
    Vector3 hovercraftCenter;

    float xOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody を取得
        if (rb == null)
        {
            Debug.LogError("Rigidbody is not attached to the gameObject.");
        }

        // targetPoints 配列の初期化
        targetPoints = new Transform[2];

        // box_1とbox_2を検索してtargetPointsに割り当てる
        GameObject box1 = GameObject.Find("Box_1");
        GameObject box2 = GameObject.Find("Box_2");

        // GameObjectが見つかったかどうかを確認し、Transformを取得してtargetPointsに割り当てる
        if (box1 != null)
        {
            targetPoints[0] = box1.transform;
            Debug.Log("box_1 found and assigned.");
        }
        else
        {
            Debug.LogError("box_1 not found.");
        }

        if (box2 != null)
        {
            targetPoints[1] = box2.transform;
            Debug.Log("box_2 found and assigned.");
        }
        else
        {
            Debug.LogError("box_2 not found.");
        }
    }

    void Start()
    {
        if (targetPoints[0] == null || targetPoints[1] == null)
        {
            Debug.LogError("One or both target points are null in Start method.");
            return; // これ以上進まないようにする
        }

        boxCenter = targetPoints[0].position;
        Debug.Log("Target Points Length: " + targetPoints.Length);
        for (int i = 0; i < targetPoints.Length; i++)
        {
            Debug.Log("Target Point " + i + ": " + targetPoints[i]);
        }

        boxCenter = targetPoints[0].position;
        hovercraftCenter = transform.position;
        xOffset = boxCenter.x - hovercraftCenter.x;
        MoveToTarget();
    }

    void FixedUpdate()
    {
        if (!targetFound)
        {
            timer += Time.deltaTime;

            if (timer >= changeDirectionInterval)
            {
                ChangeDirection();
                timer = 0f;
            }

            MoveHovercraft();
        }
    }

    void MoveToTarget()
    {
        if (targetPoints == null || targetPoints.Length == 0 || targetPoints[targetIndex] == null)
        {
            Debug.LogError("Target points array is null, empty, or contains null elements.");
            return;
        }

        // 目標地点への移動
        Vector3 targetPosition = new Vector3(targetPoints[targetIndex].position.x + xOffset, transform.position.y + 4, targetPoints[targetIndex].position.z);
        Debug.Log("Moving to target position: " + targetPosition);

        Vector3 direction = (targetPosition - rb.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        if (jumpCounter < jumpCount)
        {
            StartCoroutine(JumpOnBox());
            Debug.Log("jumpcounter: " + jumpCounter);
            jumpCounter++;
        }
        else
        {
            // 指定回数以上の場合、次の目標地点に向かう
            targetFound = false;
            ChangeDirection();
        }
    }

    void MoveHovercraft()
    {
        // Rigidbody がアタッチされているか確認
        if (rb != null)
        {
            // targetPoints 配列が空でないことを確認
            if (targetPoints.Length > 0)
            {
                // 目標地点への移動
                if (targetPoints[targetIndex] == null)
                {
                    Debug.LogError("Target point at index " + targetIndex + " is null.");
                    return;
                }

                Vector3 targetPosition = targetPoints[targetIndex].position + Vector3.up * 4;
                Vector3 direction = (targetPosition - rb.position).normalized;
                Vector3 movement = direction * moveSpeed * Time.deltaTime;
                rb.MovePosition(rb.position + movement);
                if (jumpCounter < jumpCount)
                {
                    StartCoroutine(JumpOnBox());
                    Debug.Log("jumpcounter: " + jumpCounter);
                    jumpCounter++;
                }
                else
                {
                    // 指定回数以上の場合、次の目標地点に向かう
                    targetFound = false;
                    ChangeDirection();
                }
            }
            else
            {
                Debug.LogError("Target points array is empty.");
            }
        }
        else
        {
            Debug.LogError("Rigidbody is not attached to the gameObject.");
        }
    }

    void ChangeDirection()
    {
        // targetPoints 配列の長さが0でない場合にのみ方向を変更する
        if (targetPoints.Length > 0)
        {
            targetIndex = (targetIndex + 1) % targetPoints.Length;
            MoveToTarget();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            // ジャンプ回数が指定回数以下の場合
            if (jumpCounter < jumpCount)
            {
                StartCoroutine(JumpOnBox());
                Debug.Log("jumpcounter: " + jumpCounter);
                jumpCounter++;
            }
            else
            {
                // 指定回数以上の場合、次の目標地点に向かう
                targetFound = false;
                ChangeDirection();
            }
        }
    }

    IEnumerator JumpOnBox()
    {
        rb = GetComponent<Rigidbody>(); // rb を適切に初期化
        for (int i = 0; i < 3; i++) // ジャンプ回数を3回に変更
        {
            // ボックスの上にジャンプ
            rb.MovePosition(targetPoints[0].position + Vector3.up * (i + 1));

            // 一定の待機時間
            yield return new WaitForSeconds(0.5f);
        }

        // 指定回数のジャンプが終了後、次の目標地点に向かう
        jumpCounter = 0; // ジャンプ回数カウンターをリセット
        targetFound = false;
        ChangeDirection();
    }
}

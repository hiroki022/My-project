using UnityEngine;
using UnityEngine.UI;

public class GoalSpawner : MonoBehaviour
{
    public GameObject groundObject;
    public GameObject playerObject;
    public GameObject goalObject;
    public Text goalDirectionText;
    public Canvas canvas;
    public Button restartButton;

    private float groundHeight;
    private Vector3 playerPosition;
    private Vector3 goalPosition;
    private bool hasReachedGoal;
    private bool isGoalVisible;

    private Camera mainCamera;

    public float playerMoveSpeed = 2f; // プレイヤーの移動速度

    private Vector3 initialPlayerPosition;

private void SetRandomGoalPosition()
{
    // 1分間に移動できる距離を計算する
    float maxDistance = playerMoveSpeed * 60f;

    // プレイヤーの位置から最小2分、最大3分の範囲でランダムにゴールの位置を決定する
    float x = playerObject.transform.position.x + Random.Range(maxDistance * 2f, maxDistance * 3f);
    float z = playerObject.transform.position.z + Random.Range(maxDistance * 2f, maxDistance * 3f);

    // Raycastを使って地面の高さを取得する
    RaycastHit hit;
    if (Physics.Raycast(new Vector3(x, 100f, z), Vector3.down, out hit))
    {
        // 地面の高さを取得し、ゴールの位置を設定する
        float groundHeight = hit.point.y;
        goalPosition = new Vector3(x, groundHeight, z);
        goalObject.transform.position = goalPosition;
    }
    else
    {
        // 地面が見つからない場合は、デフォルトの高さを使う
        goalPosition = new Vector3(x, groundObject.transform.position.y, z);
        goalObject.transform.position = goalPosition;
    }
}

    /// <summary>
    /// プレイヤーの位置から地面の高さを取得する
    /// </summary>
    /// <returns>地面の高さ</returns>
    private float GetGroundHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerObject.transform.position, Vector3.down, out hit))
        {
            return hit.point.y;
        }
        else
        {
            return groundObject.transform.position.y;
        }
    }

void Start()
{
    canvas = GameObject.Find("arrows").GetComponent<Canvas>();
    goalDirectionText = canvas.transform.Find("GoalDirectionText").GetComponent<Text>();

    // リスタートボタンの設定
    restartButton = canvas.transform.GetComponentInChildren<Button>();
    restartButton.onClick.AddListener(Restart);

    // プレイヤーの初期位置を取得
    initialPlayerPosition = playerObject.transform.position;

    // メインカメラの取得
    mainCamera = Camera.main;

    NewGame();
}

private PhysicMaterial groundPhysicsMaterial;

void Update()
{
    // プレイヤーの位置から地面の高さを取得する
    RaycastHit hit;
    if (Physics.Raycast(playerObject.transform.position, Vector3.down, out hit))
    {
        groundHeight = hit.point.y;
        groundPhysicsMaterial = hit.collider.sharedMaterial;
    }
    else
    {
        groundHeight = groundObject.transform.position.y;
        groundPhysicsMaterial = groundObject.GetComponent<Collider>().sharedMaterial;
    }

    // プレイヤーの位置を更新する
    playerPosition = playerObject.transform.position;
    float newY = Mathf.Max(groundHeight, playerPosition.y);
    playerObject.transform.position = new Vector3(playerPosition.x, newY, playerPosition.z);

    // プレイヤーの移動方向を取得する
    Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    moveDirection = Quaternion.AngleAxis(mainCamera.transform.eulerAngles.y, Vector3.up) * moveDirection;
    moveDirection.Normalize();

    // プレイヤーを移動させる
    float frictionCoefficient = groundPhysicsMaterial != null ? groundPhysicsMaterial.dynamicFriction : 1f;
    playerObject.transform.Translate(moveDirection * playerMoveSpeed * frictionCoefficient * Time.deltaTime, Space.Self);

    float distance = Vector3.Distance(playerPosition, goalPosition);

    // ゴールに到達したかどうかを判定
    if (distance < 2f && !hasReachedGoal)
    {
        hasReachedGoal = true;
        goalDirectionText.text = "ゴールしました!";
        Debug.Log("ゴールしました!");
        UpdateGoalDirectionText();
    }

    // ゴールオブジェクトの表示/非表示を切り替える
    isGoalVisible = true;
    goalObject.SetActive(isGoalVisible);

    // 矢印の表示/非表示を常に表示する
    goalDirectionText.gameObject.SetActive(true);
    UpdateGoalDirectionText();
}

    /// <summary>
    /// ゴールの方向を示す矢印の表示を更新する
    /// </summary>
    private void UpdateGoalDirectionText()
    {
        Vector3 direction = (goalPosition - playerPosition).normalized;

        string directionText;
        float angle = GetAngleFromDirection(direction, mainCamera.transform.forward);

        if (angle >= -45f && angle < 45f)
            directionText = "→";
        else if (angle >= 45f && angle < 135f)
            directionText = "↑";
        else if (angle >= 135f || angle < -135f)
            directionText = "←";
        else
            directionText = "↓";

        goalDirectionText.fontSize = 36;
        goalDirectionText.text = directionText;
        goalDirectionText.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    /// <summary>
    /// 方向ベクトルからアングルを計算する
    /// </summary>
    /// <param name="direction">方向ベクトル</param>
    /// <param name="cameraForward">カメラの向き</param>
    /// <returns>アングル</returns>
    private float GetAngleFromDirection(Vector3 direction, Vector3 cameraForward)
    {
        // カメラの向きを考慮して、方向ベクトルのアングルを計算する
        Vector3 relativeDirection = Quaternion.Inverse(Quaternion.LookRotation(cameraForward)) * direction;
        float angle = Mathf.Atan2(relativeDirection.z, relativeDirection.x) * Mathf.Rad2Deg;
        return angle;
    }

/// <summary>
/// ゲームをリスタートする
/// </summary>
public void Restart()
{
    // ゲームの状態をリセットする
    hasReachedGoal = false;
    isGoalVisible = true;
    playerObject.transform.position = initialPlayerPosition; // プレイヤーの位置を初期化
    NewGame();
}

void NewGame()
{
    // ゴールの位置をランダムに決定する
    SetRandomGoalPosition();

    // ゲームの状態を初期化する
    hasReachedGoal = false;
    UpdateGoalDirectionText();
}
}
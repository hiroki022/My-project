// using System.Collections;
// using System.Collections.Generic;
// using Unity.MLAgents;
// using Unity.MLAgents.Sensors;
// using UnityEngine;
// using UnityStandardAssets.Characters.ThirdPerson;

// public class GhostAgent : Agent
// {
//     private ThirdPersonCharacter m_ThirdPersonCharacter;
//     private Animator animator;
//     // 追いかけるターゲット
//     [SerializeField]
//     private Transform target;
//     // 速さ
//     [SerializeField]
//     private float walkSpeed = 2f;
//     // 速度
//     private Vector3 velocity;

//     private Vector3 initialPosition = new Vector3(466.559998f, 25.6709995f, 750.969971f);

//     private void Start()
//     {
//         m_ThirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
//         animator = GetComponent<Animator>();
//     }

//     public override void OnEpisodeBegin()
//     {
//         Reset();
//     }

//     // 観察の収集
//     public override void CollectObservations(VectorSensor sensor)
//     {
//         // ゲームの舞台サイズに合わせて正規化し観察に追加する
//         sensor.AddObservation(target.localPosition / 15f);
//         sensor.AddObservation(transform.localPosition / 15f);
//         // 主人公の方向を正規化し観察に追加する
//         var direction = (target.localPosition - transform.localPosition).normalized;
//         sensor.AddObservation(direction);
//     }

//     // アクションの受け取りと報酬を与える
//     public override void OnActionReceived(ActionBuffers actions)
//     {
//         // MaxStepを分母にして1ステップ毎にマイナス報酬を与える
//         AddReward(-1f / MaxStep);

//         // 移動データの作成
//         var input = new Vector3(actions[0], 0f, actions[1]);

//         velocity = Vector3.zero;

//         // Debug.Log("input.magnitude");
//         // Debug.Log(input.magnitude);

//         if (input.magnitude > 0f)
//         {
//             // キャラクターの向きは徐々に変える
//             transform.rotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(input.normalized, Vector3.up), 0.1f * Time.deltaTime);
//             m_ThirdPersonCharacter.Move(new Vector3(walkSpeed, 0f, 0f), false, false);
//             // m_ThirdPersonCharacter.Move(input.normalized * walkSpeed, false, false);

//             // Debug.Log("walkSpeed");
//             // Debug.Log(walkSpeed);
//             animator.SetFloat("Forward", input.magnitude);
//         }
//         else
//         {
//             animator.SetFloat("Forward", 0f);
//         }

//         velocity.y += Physics.gravity.y * Time.deltaTime;
//         // velocity.y = 0f;
//         m_ThirdPersonCharacter.Move(velocity * Time.deltaTime, false, false);

//         // 主人公と自身（敵）の距離が1.0m以下になると、エピソードを終了する
//         var distance = Vector3.Distance(target.localPosition, transform.localPosition);
//         if (distance < 1.8f)
//         {
//             AddReward(1f - distance / 1.8f);
//             Debug.Log("reset");
//             Reset();
//         }

//         // なんらかの影響でFloorから転落し位置が-5より下になったら-0.1の報酬を与える
//         if (transform.localPosition.y < -5f)
//         {
//             AddReward(-0.1f);
//             Reset();
//         }
//     }

//     // データの初期化メソッド
//     public void Reset()
//     {
//         velocity = Vector3.zero;
//         m_ThirdPersonCharacter.enabled = false;
//         transform.localPosition = initialPosition;
//         m_ThirdPersonCharacter.enabled = true;

//         var targetThirdPersonCharacter = target.GetComponent<ThirdPersonCharacter>();
//         targetThirdPersonCharacter.enabled = false;
//         target.localPosition = new Vector3(initialPosition.x + Random.Range(0f, 30f), initialPosition.y, initialPosition.z + Random.Range(-30f, 30f));
//         targetThirdPersonCharacter.enabled = true;
//     }

//     // 自分で操作
//     public override void Heuristic(ActionBuffers actionBuffersOut)
//     {
//         actionBuffersOut[0] = Input.GetAxis("Horizontal");
//         actionBuffersOut[1] = Input.GetAxis("Vertical");
//     }
// }

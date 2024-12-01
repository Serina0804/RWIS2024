#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using UnityEngine;

public class GPTManager : MonoBehaviour
{
    [SerializeField] private PromptHandler promptHandler;

    // テスト用のボタンをエディタに追加
#if UNITY_EDITOR
    [CustomEditor(typeof(GPTManager))]
    public class GPTManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GPTManager gptManager = (GPTManager)target;
            if (GUILayout.Button("Run Test Prompt"))
            {
                gptManager.TestSendPrompt();
            }
        }
    }
#endif

    // テスト用プロンプト送信メソッド
    public void TestSendPrompt()
    {
        StartCoroutine(promptHandler.SendPrompt(response =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                Debug.Log("Test GPT Response: " + response);
            }
            else
            {
                Debug.LogError("Test GPT Response failed.");
            }
        }));
    }

    // **自動実行するためのStartメソッドを追加**
    private void Start()
    {
        Debug.Log("Running Test Prompt Automatically...");
        TestSendPrompt(); // テスト用プロンプト送信を自動実行
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json; // Newtonsoft.Jsonを使用

public class PromptHandler : MonoBehaviour
{
    [SerializeField] private APIRequester apiRequester;

    public IEnumerator SendPrompt(System.Action<string> onResponse)
    {
        // プロンプトの内容をJSON形式で作成
        // var requestBody = new
        // {
        //     model = "gpt-4",
        //     messages = new List<object>
        //     {
        //         new { role = "system", content = "あなたはピアノに関してのインタビューをしています．assistantは今までのインタビューの履歴です．assistantに対して共感してください．その後，assistantに続く質問をしてください．また，60字以内に収めてください．" },
        //         new { role = "assistant", content = "あなたがピアノを始めたきっかけは何ですか？\n元々姉がピアノを習っていたことです．" },
        //         new { role = "assistant", content = "あなたの姉さんの影響が大きかったんですね。そこから今のパッションが生まれたんですね。 姉さんは今でもピアノを弾いていますか？" }
        //     },
        //     temperature = 1.0,
        //     max_tokens = 2816,
        //     top_p = 1.0,
        //     frequency_penalty = 0.0,
        //     presence_penalty = 0.0
        // };
        var requestBody = new
        {
            model = "gpt-4o",
            messages = new List<object>
            {
                new { role = "system", content = "あなたはアシスタントです。" },
                new { role = "user", content = "こんにちは" }
            }
        };


        // Newtonsoft.Jsonを使用してJSON文字列を作成
        string jsonPayload = JsonConvert.SerializeObject(requestBody);

        // APIリクエストを送信
        yield return apiRequester.SendRequest(jsonPayload, response =>
        {
            if (response != null)
            {
                Debug.Log("GPT Response: " + response);
                onResponse(response);
            }
            else
            {
                Debug.LogError("Failed to get GPT response");
                onResponse(null);
            }
        });
    }
}

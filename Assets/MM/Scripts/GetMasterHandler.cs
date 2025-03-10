using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MM
{
    public enum MasterDataType
    {
        csv,
        bin
    }
    
    public class GetMasterHandler
    {
        // データを取得するメソッド
        public static async UniTask GetMaster(string environment, MasterDataType dataType,CancellationToken cancelToken)
        {
            // リクエストURLを作成
            string url = $"{Constant.BaseUrl}?environment={environment}&dataType={dataType}";
            UnityWebRequest request = UnityWebRequest.Get(url);

            // リクエストを送信し、完了を待機
            await request.SendWebRequest().WithCancellation(cancelToken);

            // エラーチェック
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                // データを取得
                byte[] data = request.downloadHandler.data;

                // データの処理（例：バイナリデータをファイルに保存）
                string filePath = $"{Application.persistentDataPath}/masterdata-{environment}.{dataType}";
                await System.IO.File.WriteAllBytesAsync(filePath, data, cancelToken);
                Debug.Log($"Data saved to: {filePath}");
            }
        }
    }
}
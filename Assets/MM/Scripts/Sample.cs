using System.IO;
using System.Threading;
using MasterMemory;
using UnityEngine;

[assembly: MasterMemoryGeneratorOptions(Namespace = "MM")]
namespace MM
{
    
    /// <summary>
    /// Sample.cs
    /// GAS のエンドポイントから CSV をダウンロードし、
    /// MasterMemoryLoader を使って CSV から MemoryDatabase を構築、
    /// その中の PlanetTable の全データをテキスト表示するサンプル実装です。
    /// </summary>
    public class Sample : MonoBehaviour
    {
        // GAS のエンドポイントに渡す環境名（PlanetMaster用）
        private const string EnvironmentName = "dev1";
        // データ形式は csv 固定
        private const MasterDataType DataType = MasterDataType.csv;
        private string FilePath => $"{Application.persistentDataPath}/masterdata-{EnvironmentName}.{DataType}";

        private CancellationTokenSource _cts;

        private async void Start()
        {
            _cts = new CancellationTokenSource();

            // GAS からマスターデータ（CSV）をダウンロード（既存の GetMasterHandler を利用）
            await GetMasterHandler.GetMaster(EnvironmentName, MasterDataType.csv,  _cts.Token);

            // ダウンロード済み CSV ファイルの存在確認
            if (!File.Exists(FilePath))
            {
                Debug.LogError($"CSV ファイルが存在しません: {FilePath}");
                return;
            }

            // CSV ファイルをテキストとして読み込み
            string csvText = await File.ReadAllTextAsync(FilePath, _cts.Token);

            // CSV から MasterMemory 用の MemoryDatabase を構築
            MemoryDatabase memoryDatabase = MasterMemoryLoader.BuildDatabaseFromCSV(csvText);

            // 生成されたテーブル（PlanetTable）の全データを取得して表示
            // ※PlanetTable は MasterMemory のソースジェネレータにより生成されるプロパティです
            var allPlanets = memoryDatabase.PlanetMasterTable;
            Debug.Log(allPlanets.Count);
        }

        private void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}

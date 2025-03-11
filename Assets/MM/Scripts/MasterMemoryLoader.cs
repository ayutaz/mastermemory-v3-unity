using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MM
{
    // MasterMemoryへCSVから構築するための例（DatabaseBuilderを利用）
    public static class MasterMemoryLoader
    {
        /// <summary>
        /// CSVテキストからMasterMemory用のMemoryDatabaseを構築する
        /// </summary>
        /// <param name="csvText">CSVテキスト</param>
        /// <returns>構築されたMemoryDatabase</returns>
        public static MemoryDatabase BuildDatabaseFromCSV(string csvText)
        {
            var records = ParsePlanetMasterData(csvText);
            var builder = new DatabaseBuilder();
            // ローカルに保存
            var stream = new System.IO.MemoryStream();
            builder.WriteToStream(stream);
            
            // データを追加
            builder.Append(records.ToArray());
            byte[] dbBinary = builder.Build();
            return new MemoryDatabase(dbBinary);
        }

        /// <summary>
        /// タブ区切りCSVテキストから、ヘッダー3行をスキップしてPlanetMasterのリストを生成する
        /// </summary>
        /// <param name="csvText">CSVテキスト</param>
        /// <returns>パースしたPlanetMasterのリスト</returns>
        private static List<PlanetMaster> ParsePlanetMasterData(string csvText)
        {
            var result = new List<PlanetMaster>();
            var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 5) // ヘッダー3行 + 1行以上のデータが必要
            {
                Debug.LogError("CSVデータの行数が足りません。");
                return result;
            }

            for (int i = 3; i < lines.Length; i++) // 4行目以降をデータとして扱う
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // CSVの行を適切に解析（ダブルクォーテーションを考慮）
                var tokens = ParseCSVLine(line);
                if (tokens.Length < 9) // 9列以上あるか確認
                {
                    Debug.LogError($"行 {i + 1} の列数が不正です。");
                    continue;
                }

                try
                {
                    var record = new PlanetMaster
                    {
                        Id = int.Parse(tokens[0], CultureInfo.InvariantCulture),
                        Name = tokens[1],
                        NameJP = tokens[2],
                        RotationCenterPlanetId = int.Parse(tokens[3], CultureInfo.InvariantCulture),
                        Radius = float.Parse(tokens[4], CultureInfo.InvariantCulture),
                        Gravity = float.Parse(tokens[5], CultureInfo.InvariantCulture),
                        OrbitalSpeed = float.Parse(tokens[6], CultureInfo.InvariantCulture),
                        LightIntensity = float.Parse(tokens[7], CultureInfo.InvariantCulture),
                        LightOuterRadius = float.Parse(tokens[8], CultureInfo.InvariantCulture)
                    };
                    result.Add(record);
                } catch (Exception ex)
                {
                    Debug.LogError($"行 {i + 1} のパースに失敗: {ex.Message}");
                    // デバッグのために各トークンを出力
                    for (int j = 0; j < tokens.Length; j++)
                    {
                        Debug.LogWarning($"  トークン[{j}] = '{tokens[j]}'");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// CSVの1行をダブルクォーテーションを考慮して適切に解析する
        /// </summary>
        private static string[] ParseCSVLine(string line)
        {
            List<string> tokens = new List<string>();
            Regex csvRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            // カンマで分割（ダブルクォーテーション内のカンマは無視）
            string[] parts = csvRegex.Split(line);

            // 各部分からダブルクォーテーションを削除
            foreach (var part in parts)
            {
                string token = part.Trim();
                if (token.StartsWith("\"") && token.EndsWith("\""))
                {
                    token = token.Substring(1, token.Length - 2);
                }

                tokens.Add(token);
            }

            return tokens.ToArray();
        }
    }
}
using MasterMemory;
using MessagePack;

namespace MM
{
    // マスターデータの型定義（惑星マスタ）
    [MemoryTable("planet"), MessagePackObject(true)]
    public record PlanetMaster
    {
        [PrimaryKey] public int Id { get; init; }
        public string Name { get; init; }
        public string NameJP { get; init; }
        public int RotationCenterPlanetId { get; init; }
        public float Radius { get; init; }
        public float Gravity { get; init; }
        public float OrbitalSpeed { get; init; }
        public float LightIntensity { get; init; }
        public float LightOuterRadius { get; init; }
    }
}
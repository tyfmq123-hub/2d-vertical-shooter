using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// stage_data.json 한 항목에 대응하는 데이터 구조
public class SpawnData
{
    public enum EnemyType
    {
        A,
        B,
        C
    }

    // 이전 스폰으로부터 대기할 시간(초)
    public float delay;

    // JSON의 "type" 키("A"/"B"/"C" 문자열)를 EnemyType 열거형으로 변환
    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public EnemyType enemyType;

    // 스폰 위치 배열(spawnPoints)의 인덱스
    public int point;
}
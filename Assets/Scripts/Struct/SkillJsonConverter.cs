using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

public class SkillJsonConverter : JsonConverter
{
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jo = JObject.Load(reader);
        var type = jo["subtype"]?.ToString();

        SkillData skillData;
        switch (type)
        {
            case "areaAttack":
                skillData = new AttackEffect();
                break;
            case "healHp":
                skillData = new HealHpEffect();
                break;
            case "increaseAttack":
                skillData = new BuffEffect();
                break;
            default:
                skillData = new SkillData();
                break;
        }
        serializer.Populate(jo.CreateReader(), skillData);
        return skillData;
    }

    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(SkillData))
            return true;
        return false;
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // 내보낼 일은 없어 사용하지 않는다.
    }
}
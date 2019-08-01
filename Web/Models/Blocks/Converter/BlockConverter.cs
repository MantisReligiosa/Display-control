using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Web.Models.Blocks.Converter
{
    public class BlockConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings _specifiedSubclassConversion = new JsonSerializerSettings()
        {
            ContractResolver = new BaseSpecifiedConcreteClassConverter()
        };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BlockDto));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            if (jObject["type"].Value<string>().Equals("meta", StringComparison.InvariantCultureIgnoreCase))
                return DeserializeMetaBlock(jObject);
            return DeserializeSimpleBlock(jObject);
        }

        private static BlockDto DeserializeSimpleBlock(JObject jObject)
        {
            switch (jObject["type"].Value<string>())
            {
                case "datetime":
                    return JsonConvert.DeserializeObject<DateTimeBlockDto>(jObject.ToString(), _specifiedSubclassConversion);
                case "picture":
                    return JsonConvert.DeserializeObject<PictureBlockDto>(jObject.ToString(), _specifiedSubclassConversion);
                case "table":
                    return JsonConvert.DeserializeObject<TableBlockDto>(jObject.ToString(), _specifiedSubclassConversion);
                case "text":
                    return JsonConvert.DeserializeObject<TextBlockDto>(jObject.ToString(), _specifiedSubclassConversion);
            }
            throw new NotImplementedException();
        }

        private static object DeserializeMetaBlock(JObject jMetablockObject)
        {
            var metablockDto = JsonConvert.DeserializeObject<MetaBlockDto>(jMetablockObject.ToString(), _specifiedSubclassConversion);
            foreach (var jFrame in jMetablockObject["frames"].AsJEnumerable())
            {
                var id = new Guid(jFrame["id"].Value<string>());
                var frameDto = metablockDto.Frames.First(f => f.Id.Equals(id));
                frameDto.Blocks = jFrame["blocks"].AsJEnumerable().Select(jBlock => DeserializeSimpleBlock(jBlock as JObject)).ToList();
            }
            return metablockDto;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Serialization
{
    public class GuidAsStringSerializer : SerializerBase<string?>
    {
        public override string? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();

            switch (type)
            {
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.ObjectId:
                    return context.Reader.ReadObjectId().ToString();
                default:
                    throw new NotSupportedException($"Cannot convert BsonType {type} to string");
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string? value)
        {
            if (value is null)
            {
                context.Writer.WriteNull();
                return;
            }

            if (ObjectId.TryParse(value, out _))
            {
                context.Writer.WriteObjectId(ObjectId.Parse(value));
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }
}

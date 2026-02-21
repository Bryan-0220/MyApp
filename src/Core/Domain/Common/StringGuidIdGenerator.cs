using MongoDB.Bson.Serialization;

namespace Domain.Common
{
    public class StringGuidIdGenerator : IIdGenerator
    {
        public object GenerateId(object container, object document)
        {
            return Guid.NewGuid().ToString();
        }

        public bool IsEmpty(object id)
        {
            return string.IsNullOrEmpty(id as string);
        }
    }
}

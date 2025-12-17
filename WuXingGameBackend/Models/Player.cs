using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WuXingGameBackend.Models
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PlayerId { get; set; }
        public int Yin { get; set; }
        public int Yang { get; set; }
        public Dictionary<string, int> Elements { get; set; } = new();
        public int PosX { get; set; }
        public int PosY { get; set; }
    }
}
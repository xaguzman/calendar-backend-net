// using MongoDB.Bson.Serialization.Attributes;

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CalendarBackend.Models;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } =  null!;
    
    public string Name { get; set; } =  null!;

    public string Email { get; set; } =  null!;

    public string Password {get; set;} =  null!;

    [JsonIgnore]
    public string Salt { get; set; } =  null!;
}

[BsonIgnoreExtraElements]
public class UserDisplayView {
    [BsonRepresentation(BsonType.ObjectId)]
    public String Id { get; set; } = null!;
    public String Name { get; set; } = null!;

}

public record Login(String Email, String Password);
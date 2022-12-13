using System.Text.Json.Serialization;
using CalendarBackend.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CalendarBackend.Models;

public class EventModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string Notes { get; set; } = null!;

    // [JsonConverter(typeof(UnixEpochDateTimeConverter))]
    public DateTime? Start { get; set; }

    // [JsonConverter(typeof(UnixEpochDateTimeConverter))]
    public DateTime? End { get; set; }

    [BsonElement("user"), JsonPropertyName("user")]
    [BsonRepresentation(BsonType.ObjectId)]
    public String UserId { get; set; } = null!;

}

[BsonIgnoreExtraElements]
public class EventModelView 
{

    public EventModelView(){}

    public EventModelView(EventModel from){
        this.Id = from.Id;
        this.Title = from.Title;
        this.Notes = from.Notes;
        this.Start = from.Start;
        this.End = from.End;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Notes { get; set; } = null!;

    // [JsonConverter(typeof(UnixEpochDateTimeConverter))]
    public DateTime? Start { get; set; }

    // [JsonConverter(typeof(UnixEpochDateTimeConverter))]
    public DateTime? End { get; set; }

    [BsonElement("user"), JsonPropertyName("user")]
    public UserDisplayView User { get; set; } = null!;
}
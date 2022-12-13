using System.Linq.Expressions;
using CalendarBackend.Models;
using CalendarBackend.Util;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace CalendarBackend.Services;


public class EventsService
{
    private readonly IMongoCollection<EventModel> eventCollection;

    public EventsService(IOptions<CalendarDBSettings> dbSettings)
    {
        var settings = dbSettings.Value;

        Console.WriteLine("CS: " + dbSettings.Value.ConnectionString);
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        this.eventCollection = db.GetCollection<EventModel>(settings.EventCollectionsName);
        // this.eventViewCollection = db.GetCollection<EventModelView>(settings.EventCollectionsName);
    }

    public async Task<List<EventModel>> GetAllAsync() => await GetAllAsync(_ => true);
    public async Task<List<EventModel>> GetAllAsync(Expression<Func<EventModel, bool>> filter) => await eventCollection.Find(filter).ToListAsync();

    public async Task<EventModel?> GetAsync(String id) => await GetAsync( x => x.Id == id );

    public async Task<EventModel?> GetAsync(Expression<Func<EventModel, bool>> filter) => await eventCollection.Find(filter).FirstOrDefaultAsync();

    public async Task<List<EventModelView>> GetViewAllAsync() => await GetViewAllAsync(_ => true);
    public async Task<List<EventModelView>> GetViewAllAsync(Expression<Func<EventModel, bool>> filter)
    {
        var res = await eventCollection.Aggregate()
            .Match( filter  )
            .Lookup(
                foreignCollectionName: "users",
                localField: "user",
                foreignField: "_id",
                @as: "user"
            )
            .Project( new BsonDocument{
                { "_id", 1 },
                { "title", 1 },
                { "start", 1 },
                { "end", 1 },
                { "notes", 1 },
                { "user", new BsonDocument("$arrayElemAt", new BsonArray(new BsonValue[] {"$user", 0} )) }
            })
            .ToListAsync();
               
        var result = res.Select( x => BsonSerializer.Deserialize<EventModelView>(x) ).ToList();
        return result;
    }

    public async Task<EventModelView?> GetViewAsync(String id) => await GetViewAsync(x => x.Id == id);

    public async Task<EventModelView?> GetViewAsync(Expression<Func<EventModel, bool>> filter){
        
        var res = await eventCollection.Aggregate()
            .Match( filter  )
            .Lookup(
                foreignCollectionName: "users",
                localField: "user",
                foreignField: "_id",
                @as: "user"
            )
            .Project( new BsonDocument{
                { "_id", 1 },
                { "title", 1 },
                { "start", 1 },
                { "end", 1 },
                { "notes", 1 },
                { "user", new BsonDocument("$arrayElemAt", new BsonArray(new BsonValue[] {"$user", 0} )) }
            })
            .FirstOrDefaultAsync();
        
        var result = BsonSerializer.Deserialize<EventModelView>(res);
        return result;
    }

    public async Task CreateAsync(EventModel newEvent)
    {
        await eventCollection.InsertOneAsync(newEvent);
    }

    public async Task<EventModel> UpdateAsync(string id, EventModel updatedEvent) =>
        await eventCollection.FindOneAndReplaceAsync<EventModel>(x => x.Id == id, updatedEvent, new FindOneAndReplaceOptions<EventModel>
        {
            ReturnDocument = ReturnDocument.After
        });

    public async Task RemoveAsync(string id) => await eventCollection.DeleteOneAsync(x => x.Id == id);
}
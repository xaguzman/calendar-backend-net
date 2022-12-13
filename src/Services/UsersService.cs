using System.Linq.Expressions;
using CalendarBackend.Models;
using CalendarBackend.Util;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CalendarBackend.Services;


public class UsersService
{
    private readonly IMongoCollection<UserModel> userCollection;

    public UsersService(IOptions<CalendarDBSettings> dbSettings)
    {
        var settings = dbSettings.Value;

        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        this.userCollection = db.GetCollection<UserModel>(settings.UserCollectionsName);
        var options = new CreateIndexOptions() { Unique = true };
        var field = new StringFieldDefinition<UserModel>("Email");
        var indexDefinition = new IndexKeysDefinitionBuilder<UserModel>().Ascending(field);
        var indexModel = new CreateIndexModel<UserModel>(indexDefinition, options);
        userCollection.Indexes.CreateOne(indexModel);
    }

    public async Task<List<UserModel>> GetAllAsync() => await userCollection.Find(_ => true).ToListAsync();
    public async Task<List<UserModel>> GetAllAsync(Expression<Func<UserModel, bool>> filter) => await userCollection.Find(filter).ToListAsync();

    public async Task<UserModel?> GetAsync(String id) => await userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<UserModel?> GetAsync(Expression<Func<UserModel, bool>> filter) => await userCollection.Find( filter ).FirstOrDefaultAsync();

    public async Task CreateAsync(UserModel newUser){
        var hasher = new Hasher();
        var salt = hasher.Create16BytesString();
        newUser.Password = hasher.CreateHash(newUser.Password + salt);
        newUser.Salt = salt;
        await userCollection.InsertOneAsync(newUser);
    } 

    public bool PasswordMatches(UserModel user, String password){
        var hasher = new Hasher();
        var hashed = hasher.CreateHash(password + user.Salt);
        
        return user.Password == hashed;
    }

    public async Task UpdateAsync(string id, UserModel updatedUser) => await userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) => await userCollection.DeleteOneAsync(x => x.Id == id);
}
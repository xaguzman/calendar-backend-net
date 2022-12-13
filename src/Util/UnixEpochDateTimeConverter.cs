using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalendarBackend.Util;

sealed class UnixEpochDateTimeConverter : JsonConverter<DateTime?>
{
    
    // static readonly DateTime sinceEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
    // static readonly Regex s_regex = new Regex("^/Date\\(([+-]*\\d+)\\)/$", RegexOptions.CultureInvariant);

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // DateTime.UnixEpoch
        try{
            long unixTime = reader.GetInt64();
            return DateTime.UnixEpoch.AddMilliseconds(unixTime);
        }catch {
            return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        long unixTime = ((long)(value!.Value - DateTime.UnixEpoch).TotalMilliseconds);
        // long unixTime = Convert.ToInt64((value!.Value - sinceEpoch).TotalMilliseconds);

        writer.WriteNumberValue(unixTime);
    }
}
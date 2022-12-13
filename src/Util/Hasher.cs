using System.Security.Cryptography;
using System.Text;

namespace CalendarBackend.Util;

public class Hasher
{
    public string Create16BytesString(){
        return CreateByteString(16);
    }

    public string Create32BytesString(){
        return CreateByteString(32);
    }

    private String CreateByteString(int length){
        byte[] randomBytes = new byte[length]; // 128/8
        using (var generator = RandomNumberGenerator.Create()){
            generator.GetBytes(randomBytes);
        }
        
        // return Convert.ToBase64String(randomBytes);
        return ConvertBytesToString(randomBytes);
    }

    public string CreateHash(string value){
        var bytes = CreateByteHash(value);
        
        return ConvertBytesToString(bytes);
    }

    public byte[] CreateByteHash(string value){
        byte[] hashedBytes;
        using (var sha256 = SHA256.Create()){
            hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        }
        
        
        return hashedBytes;
    }

    private string ConvertBytesToString(byte[] bytes){
        return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
    }

    
}
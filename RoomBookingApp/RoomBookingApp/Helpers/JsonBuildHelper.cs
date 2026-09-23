using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using RBA.Models.Request;
using RBA.Models.States;
using System.Text.Json;

namespace RoomBookingApp.Helpers
{
    public static class JsonBuildHelper
    {
        public static string BuildJsonResponse(BookRoomResponse bookRoomResponse)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            JsonSerializerOptions optionsCopy = new(options);
            optionsCopy.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            return JsonSerializer.Serialize(bookRoomResponse, optionsCopy);
        }
    }
}
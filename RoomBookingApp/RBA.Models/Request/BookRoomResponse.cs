using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RBA.Models.Request
{
    public class BookRoomResponse
    {
        public int? RoomId { get; set; }
        public BookState? BookState { get; set; }
        public RoomState? RoomState { get; set; }
        public string? Error_msg { get; set; }
    }
}
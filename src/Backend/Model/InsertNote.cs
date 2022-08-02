using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Model
{
    public class InsertNoteRequest
    {
        public int Id { get; set; }
        [Required]
        public string Note { get; set; }
        public string ScheduleDate { get; set; }
        public string ScheduleTime { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }

    public class InsertNoteResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

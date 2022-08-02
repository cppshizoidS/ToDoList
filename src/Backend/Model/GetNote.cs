using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Model
{
    public class GetNoteRequest
    {
        [Required]
        public int PageNumber { get; set; }

        [Required]
        public int NumberOfRecordPerPage { get; set; }

        [Required]
        public string SortBy { get; set; } // ASC, DESC
    }

    public class GetNoteResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public decimal TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<GetNote> data { get; set; }

    }

    public class GetNote
    {
        public int NoteId { get; set; }
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
}

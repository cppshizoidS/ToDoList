using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Model
{
    public class GetNoteByIdResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public InsertNoteRequest data { get; set; }
    }
}

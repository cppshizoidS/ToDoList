using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Model
{
    public class DeleteNoteResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

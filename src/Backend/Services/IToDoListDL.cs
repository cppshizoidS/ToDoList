using CommonLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IToDoListDL
    {
        public Task<InsertNoteResponse> InsertNote(InsertNoteRequest request);
        public Task<GetNoteResponse> GetNote(GetNoteRequest request);
        public Task<GetNoteByIdResponse> GetNoteById(string Id);
        public Task<UpdateNoteResponse> UpdateNote(InsertNoteRequest request);
        public Task<DeleteNoteResponse> DeleteNote(string Id);
    }
}

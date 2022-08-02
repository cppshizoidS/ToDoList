using CommonLayer.Model;
using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListBackEnd.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ToDoListController : ControllerBase
    {
        private readonly IToDoListDL _toDoListDL;
        public ToDoListController(IToDoListDL toDoListDL)
        {
            _toDoListDL = toDoListDL;
        }

        [HttpPost]
        public async Task<IActionResult> InsertNote(InsertNoteRequest request)
        {
            InsertNoteResponse response = new InsertNoteResponse();
            try
            {
                response = await _toDoListDL.InsertNote(request);

            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetNote(GetNoteRequest request)
        {
            GetNoteResponse response = new GetNoteResponse();
            try
            {
                response = await _toDoListDL.GetNote(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetNoteById([FromQuery] string Id)
        {
            GetNoteByIdResponse response = new GetNoteByIdResponse();
            try
            {
                response = await _toDoListDL.GetNoteById(Id);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNote(InsertNoteRequest request)
        {
            UpdateNoteResponse response = new UpdateNoteResponse();
            try
            {
                response = await _toDoListDL.UpdateNote(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNote([FromQuery] string Id)
        {
            DeleteNoteResponse response = new DeleteNoteResponse();
            try
            {
                response = await _toDoListDL.DeleteNote(Id);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return Ok(response);
        }
    }
}

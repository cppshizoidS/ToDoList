using CommonLayer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ToDoListDL : IToDoListDL
    {
        public readonly IConfiguration _configuration;
        public readonly ILogger<ToDoListDL> _logger;
        public readonly MySqlConnection _mySqlConnection;
        public ToDoListDL(IConfiguration configuration, ILogger<ToDoListDL> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _mySqlConnection = new MySqlConnection(_configuration["ConnectionStrings:MySqlDBString"]);
        }

        public async Task<InsertNoteResponse> InsertNote(InsertNoteRequest request)
        {
            InsertNoteResponse response = new InsertNoteResponse();
            response.IsSuccess = true;
            response.Message = "Insert Note Successfully.";

            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"INSERT INTO NoteDetails(CreatedDate, Note, ScheduleDate, ScheduleTime, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday) 
                                    VALUES (@CreatedDate, @Note, @ScheduleDate, @ScheduleTime, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, @Saturday, @Sunday)";

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@CreatedDate",DateTime.Now.ToString("MMMM, dd-MM-yyyy HH:mm tt"));
                    sqlCommand.Parameters.AddWithValue("@Note", request.Note);
                    sqlCommand.Parameters.AddWithValue("@ScheduleDate", String.IsNullOrEmpty(request.ScheduleDate)?null: request.ScheduleDate);
                    sqlCommand.Parameters.AddWithValue("@ScheduleTime", String.IsNullOrEmpty(request.ScheduleTime) ? null : request.ScheduleTime);
                    sqlCommand.Parameters.AddWithValue("@Monday", request.Monday);
                    sqlCommand.Parameters.AddWithValue("@Tuesday", request.Tuesday);
                    sqlCommand.Parameters.AddWithValue("@Wednesday", request.Wednesday);
                    sqlCommand.Parameters.AddWithValue("@Thursday", request.Thursday);
                    sqlCommand.Parameters.AddWithValue("@Friday", request.Friday);
                    sqlCommand.Parameters.AddWithValue("@Saturday", request.Saturday);
                    sqlCommand.Parameters.AddWithValue("@Sunday", request.Sunday);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Query Not Executed";
                        _logger.LogError("Error Occur : Query Not Executed");
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return response;
        }

        public async Task<GetNoteResponse> GetNote(GetNoteRequest request)
        {
            GetNoteResponse response = new GetNoteResponse();
            response.IsSuccess = true;
            response.Message = "Fetch Data Successfully.";

            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                int Offset = (request.PageNumber - 1) * request.NumberOfRecordPerPage;

                string SqlQuery = string.Empty;

                
                SqlQuery = @" SELECT Id, CreatedDate, Note, ScheduleDate, ScheduleTime, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,
                                  (SELECT COUNT(*) FROM NoteDetails) AS TotalRecord
                                  From NoteDetails 
                                  Order By Id " + request.SortBy.ToUpperInvariant()+@"
                                  LIMIT @Offset, @NumberOfRecordPerPage";
                
                

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@Offset", Offset);
                    sqlCommand.Parameters.AddWithValue("@NumberOfRecordPerPage", request.NumberOfRecordPerPage);
                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            int Count = 0;
                            response.data = new List<GetNote>();
                            while(await dataReader.ReadAsync())
                            {
                                response.data.Add(
                                    new GetNote()
                                    {
                                        NoteId = dataReader["Id"] != DBNull.Value ? (Int32)dataReader["Id"] : -1,
                                        Note = dataReader["Note"] != DBNull.Value ? (string)dataReader["Note"] : null,
                                        ScheduleDate = dataReader["ScheduleDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["ScheduleDate"]).ToString("dd/MM/yyyy") : null,
                                        ScheduleTime = dataReader["ScheduleTime"] != DBNull.Value ? Convert.ToDateTime(dataReader["ScheduleTime"]).ToString("hh:mm tt") : null,
                                        Monday= dataReader["Monday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Monday"]) : false,
                                        Tuesday= dataReader["Tuesday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Tuesday"]) : false,
                                        Wednesday= dataReader["Wednesday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Wednesday"]) : false,
                                        Thursday= dataReader["Thursday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Thursday"]) : false,
                                        Friday= dataReader["Friday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Friday"]) : false,
                                        Saturday= dataReader["Saturday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Saturday"]) : false,
                                        Sunday= dataReader["Sunday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Sunday"]) : false,
                                    }) ;

                                if (Count == 0)
                                {
                                    Count++;
                                    response.TotalRecords = dataReader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(dataReader["TotalRecord"]) : -1;
                                    response.TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                                    response.CurrentPage = request.PageNumber;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return response;
        }

        public async Task<GetNoteByIdResponse> GetNoteById(string Id)
        {
            GetNoteByIdResponse response = new GetNoteByIdResponse();
            response.IsSuccess = true;
            response.Message = "Get Note By Id Successfully";

            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"SELECT * FROM NoteDetails WHERE Id=@Id";//Id, CreatedDate, Note, ScheduleDateTime

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@Id", Id);
                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            await dataReader.ReadAsync();
                            response.data = new InsertNoteRequest();
                            response.data.Id = dataReader["Id"] != DBNull.Value ? (Int32)dataReader["Id"] : -1;
                            response.data.Note = dataReader["Note"] != DBNull.Value ? (string)dataReader["Note"] : null;
                            response.data.ScheduleDate = dataReader["ScheduleDate"] != DBNull.Value ? (string)dataReader["ScheduleDate"] : null;
                            response.data.ScheduleTime = dataReader["ScheduleTime"] != DBNull.Value ? (string)dataReader["ScheduleTime"] : null;
                            response.data.Monday = dataReader["Monday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Monday"]) : false;
                            response.data.Tuesday = dataReader["Tuesday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Tuesday"]) : false;
                            response.data.Wednesday = dataReader["Wednesday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Wednesday"]) : false;
                            response.data.Thursday = dataReader["Thursday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Thursday"]) : false;
                            response.data.Friday = dataReader["Friday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Friday"]) : false;
                            response.data.Saturday = dataReader["Saturday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Saturday"]) : false;
                            response.data.Sunday = dataReader["Sunday"] != DBNull.Value ? Convert.ToBoolean(dataReader["Sunday"]) : false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return response;
        }

        public async Task<UpdateNoteResponse> UpdateNote(InsertNoteRequest request)
        {
            UpdateNoteResponse response = new UpdateNoteResponse();
            response.IsSuccess = true;
            response.Message = "Update Note Successfully.";
            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    UPDATE todolist.notedetails
                                    SET UpdatedDate=@UpdatedDate, 
                                        Note=@Note, 
                                        ScheduleDate=@ScheduleDate, 
                                        ScheduleTime=@ScheduleTime,
                                        Monday=@Monday, 
                                        Tuesday=@Tuesday, 
                                        Wednesday=@Wednesday, 
                                        Thursday=@Thursday, 
                                        Friday=@Friday, 
                                        Saturday=@Saturday, 
                                        Sunday=@Sunday
                                    WHERE Id=@Id
                                    ";//Id, CreatedDate, Note, ScheduleDateTime

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@Id", request.Id);
                    sqlCommand.Parameters.AddWithValue("@UpdatedDate", DateTime.Now.ToString("MMMM, dd-MM-yyyy HH:mm tt"));
                    sqlCommand.Parameters.AddWithValue("@Note", request.Note);
                    sqlCommand.Parameters.AddWithValue("@ScheduleDate", request.ScheduleDate);
                    sqlCommand.Parameters.AddWithValue("@ScheduleTime", request.ScheduleTime);
                    sqlCommand.Parameters.AddWithValue("@Monday", request.Monday);
                    sqlCommand.Parameters.AddWithValue("@Tuesday", request.Tuesday);
                    sqlCommand.Parameters.AddWithValue("@Wednesday", request.Wednesday);
                    sqlCommand.Parameters.AddWithValue("@Thursday", request.Thursday);
                    sqlCommand.Parameters.AddWithValue("@Friday", request.Friday);
                    sqlCommand.Parameters.AddWithValue("@Saturday", request.Saturday);
                    sqlCommand.Parameters.AddWithValue("@Sunday", request.Sunday);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Query Not Executed";
                        _logger.LogError("Error Occur : Query Not Executed");
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return response;
        }

        public async Task<DeleteNoteResponse> DeleteNote(string Id)
        {
            DeleteNoteResponse response = new DeleteNoteResponse();
            response.IsSuccess = true;
            response.Message = "Delete Note Successfully";

            try
            {

                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"DELETE FROM todolist.notedetails WHERE Id=@Id";//Id, CreatedDate, Note, ScheduleDateTime

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@Id", Id);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Query Not Executed";
                        _logger.LogError("Error Occur : Query Not Executed");
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return response;
        }
    }
}

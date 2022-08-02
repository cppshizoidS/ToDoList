using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListBackEnd.Common_Utility
{
    public class SqlQueries
    {
        public static IConfiguration _configuration = new ConfigurationBuilder()
                                                          .AddXmlFile("SqlQueries.xml", true, true).Build();

        public static string InsertNote { get { return _configuration["InsertNote"]; } }
        public static string GetNote { get { return _configuration["GetNote"]; } }
        public static string GetNoteById { get { return _configuration["GetNoteById"]; } }
        public static string UpdateNote { get { return _configuration["UpdateNote"]; } }
        public static string DeleteNote { get { return _configuration["DeleteNote"]; } }


    }
}

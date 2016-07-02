using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MrCullDevTools.Controllers.Helper
{
    [DataContract]
    public class SqlFormatterDataContract
    {
        [DataMember(Name = "rspn_http_status")]
        public int rspn_http_status { get; set; }

        [DataMember(Name = "rspn_capacity")]
        public int rspn_capacity { get; set; }

        [DataMember(Name = "rspn_db_vendor")]
        public string rspn_db_vendor { get; set; }

        [DataMember(Name = "rspn_output_fmt")]
        public string rspn_output_fmt { get; set; }

        [DataMember(Name = "rspn_parse_sql_status")]
        public int rspn_parse_sql_status { get; set; }

        [DataMember(Name = "rspn_parse_sql_message")]
        public string rspn_parse_sql_message { get; set; }

        [DataMember(Name = "rspn_formatted_sql")]
        public string rspn_formatted_sql { get; set; }
    }


//    ({"rspn_http_status":200,"rspn_capacity":0,"rspn_db_vendor":"mssql","rspn_output_fmt":"","rspn_parse_sql_status":0,"rspn_parse_sql_message":"success","rspn_formatted_sql":"Input SQL is empty!"})
    
}
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

// start working SAT: 7pm
// finish work SAT: 11pm
namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Student
    {
        public string Pin { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        List<StudentCourses> Courses { get; set; }  
    }

    public struct StudentCourses
    {
        public string StudentPin { get; set; }
        public int CourseId { get; set; }

        public DateOnly ComletionDate{ get; set; }

    }

    public class Courses
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Instructur { get; set; }

        public int TotalTime { get; set; }

        public int Credit { get; set; }
    }

    public class Instructor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
    public enum OutputFormat
    {
        CSV,
        HTML,
        ALL
    }

    public class InputData
    {
        public string StudentPin { get; set; }

        public double MinCredit { get; set; }

        public string DateRangeFirst { get; set; }

        public string DateRangeLast { get; set; }

        public string DirectorysPath { get; set; }

        public InputData()
        {
        }
        public InputData(string studenPin, double minCredit, string dateRangeFirst, string dateRangeLast, string directorysPath)
        {
            StudentPin = studenPin;
            MinCredit = minCredit;
            DateRangeFirst = dateRangeFirst;
            DateRangeLast = dateRangeLast;
            DirectorysPath = directorysPath;
        }
    }

    public class OutputData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CourseName { get; set; }

        public int CourseTime { get; set; }

        public int CourseCredit { get; set; }

        public string InstructorFullName { get; set; }

    }

    public static class GlobalConfig
    {
        /// <summary>
        /// Default Server in PostgreSQL;
        /// </summary>
        private static string server = "localhost";

        /// <summary>
        /// Default Port in PostgreSQL;
        /// </summary>
        private static string port = "5434";

        /// <summary>
        /// Default User in PostgreSQL;
        /// </summary>
        private static string user = "postgres";

        /// <summary>
        /// Default Password in PostgreSQL;
        /// </summary>
        private static string pass = "dbpass";

        /// <summary>
        /// Default Password in PostgreSQL;
        /// </summary>
        private static string db = "coursera";

        public static string GetConnectionString()
        {
            string connString = string.Format(@"Host={0};Port={1};User Id={2};Password={3};Database={4}", server, port, user, pass, db);

            return connString;
        }
    }

    public static class SQL_Connector
    {
        public static List<T> GetQuery<T>(this string sqlQuery)
        {
            string output;

            using (var conn = new NpgsqlConnection(GlobalConfig.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlQuery, conn))
                {
                    output = cmd.ExecuteScalar().ToString();

                }
                conn.Close();
            }

            return JsonConvert.DeserializeObject<List<T>>(output);
        }

        public static List<OutputData> GetQuery(this string sqlQuery)
        {
            List<OutputData> output = new();

            using (var conn = new NpgsqlConnection(GlobalConfig.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlQuery, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OutputData currRow = new();
                            { 
                            currRow.FirstName = reader.GetString(0);
                            currRow.LastName = reader.GetString(1);
                            currRow.CourseName = reader.GetString(2);
                            currRow.CourseCredit = reader.GetInt32(3);
                            currRow.CourseTime = reader.GetInt32(4);
                            currRow.InstructorFullName = reader.GetString(5);

                            }
    

                            output.Add(currRow);
                        }
                    }

                }
                conn.Close();
            }

            return output;
        }
    }

    public static class QueriesExtensionMethots
    {
        public static string ReplaceParamaters(this string query, string minCredit, string dateRangeFirst, string dateRangeLast, string pinList )
        {
            string output = query;
            if (output.Contains("@minCredit"))
            {
                output = output.Replace("@minCredit", minCredit);
            }
            if (output.Contains("@dateRangeFirst"))
            {
                output = output.Replace("@dateRangeFirst", dateRangeFirst);
            }
            if (output.Contains("@dateRangeLast"))
            {
                output = output.Replace("@dateRangeLast", dateRangeLast);
            }
            if (output.Contains("@pinList"))
            {
                output = output.Replace("@pinList", pinList);
            }
            else
            {
                output = output.Replace("@pinList", "SELECT pin FROM students");
            }

            return output;
        }

        public static string ReplaceParamaters(this string query, InputData data)
        {
            string output = query;
            if (output.Contains("@minCredit"))
            {
                output = output.Replace("@minCredit", data.MinCredit.ToString());
            }
            if (output.Contains("@dateRangeFirst"))
            {
                output = output.Replace("@dateRangeFirst", data.DateRangeFirst);
            }
            if (output.Contains("@dateRangeLast"))
            {
                output = output.Replace("@dateRangeLast", data.DateRangeLast);
            }
            if (output.Contains("@pinList"))
            {
                if (data.StudentPin != "")
                {
                    output = output.Replace("@pinList", data.StudentPin);
                }
                else
                {
                    output = output.Replace("@pinList", "SELECT pin FROM students");
                }

            }
            

            return output;
        }
    }
    public static class sqlQueries
    {
        public static string requestStudentsAsJSON =
            @"SELECT array_to_json(array_agg(row_to_json(t2)))
            FROM (
	            SELECT st.first_name AS first_name,  st.last_name AS last_name,  co.name AS course_name, co.total_time AS course_time, co.credit AS course_credit, CONCAT (ins.first_name , ' ', ins.last_name) AS instructor 
	            FROM students st
		            INNER JOIN public.students_courses_xref sc ON sc.student_pin = st.pin
		            INNER JOIN public.courses co ON co.id = sc.course_id
		            INNER JOIN public.instructors ins ON ins.id = co.instructor_id
	            WHERE st.pin IN (
		            SELECT t1.pin 
		            FROM
			            (
			            SELECT st.pin AS pin, SUM(co.total_time) AS tcredit FROM students st
				            INNER JOIN public.students_courses_xref sc ON sc.student_pin = st.pin
				            INNER JOIN public.courses co ON co.id = sc.course_id
				            INNER JOIN public.instructors ins ON ins.id = co.instructor_id
			            WHERE pin  IN (@pinList) 
                            AND sc.completion_date BETWEEN DATE '@dateRangeFirst' AND DATE '@dateRangeLast'
			            GROUP BY st.pin
				            ) t1 
		            WHERE tcredit > @minCredit
		            ) 
	            ) t2";

        public static string requestStudentsAsView =
            @"SELECT st.first_name AS first_name,  st.last_name AS last_name,  co.name AS course_name, co.total_time AS course_time, co.credit AS course_credit, CONCAT (ins.first_name , ' ', ins.last_name) AS instructor 
	            FROM students st
		            INNER JOIN public.students_courses_xref sc ON sc.student_pin = st.pin
		            INNER JOIN public.courses co ON co.id = sc.course_id
		            INNER JOIN public.instructors ins ON ins.id = co.instructor_id
	            WHERE st.pin IN (
		            SELECT t1.pin 
		            FROM
			            (
			            SELECT st.pin AS pin, SUM(co.total_time) AS tcredit FROM students st
				            INNER JOIN public.students_courses_xref sc ON sc.student_pin = st.pin
				            INNER JOIN public.courses co ON co.id = sc.course_id
				            INNER JOIN public.instructors ins ON ins.id = co.instructor_id
			            WHERE pin  IN (@pinList) 
                            AND sc.completion_date BETWEEN DATE '@dateRangeFirst' AND DATE '@dateRangeLast'
			            GROUP BY st.pin
				            ) t1 
		            WHERE tcredit > @minCredit
		            )";
    }

    
    internal class Program
    {
        private static List<string>  ConvertStringToListOfPins(string pins)
        {
            List<string> pinsList = new ();

            foreach (var pin in pins.Split(','))
            {
                pinsList.Add(pin);
            }

            return pinsList;
        }
        private static OutputFormat ReadOutputFormat( string outputFormat)
        {
            switch (outputFormat.ToUpper())
            {
                case "CSV":
                    return OutputFormat.CSV;
                case "HTML":
                    return OutputFormat.HTML;
                default:
                    return OutputFormat.ALL;
            }          
        }
        public static InputData Request()
        {
            string pins = "";
            double minCredit = 0;
            string dateRangeFirst;
            string dateRangeLast;
            OutputFormat outputFormat;
            string directoriesPath;
            Console.WriteLine("Hello. To create a new request, please fill in the form: ");
            Console.WriteLine("(y/n) Would you like to search based on student's PINs.?  (If not, the Query will select all students from the DB).");
            if (Console.ReadLine().ToLower() == "y")
            {
                Console.WriteLine("Please enter the student's PINs, separated by a comma ',' (Example 876555 , 945532, 445465).");
                pins = Console.ReadLine();
            }

            Console.WriteLine("Please enter the required minimum credit: (Example: 10.5)");
            minCredit = int.Parse(Console.ReadLine());

            Console.WriteLine("Please enter the starting date of the time range: (Example: 2020-01-01)");
            dateRangeFirst = Console.ReadLine();

            Console.WriteLine("Please enter the end date of the time range: (Example: 2022-01-01)");
            dateRangeLast = Console.ReadLine();

            Console.WriteLine("(y/n) Would You like to specify an output format? ( If not, the report will be saved in both .CSV and .HTML files.");
            if (Console.ReadLine().ToLower() == "y")
            {
                Console.WriteLine("Please enter CSV for saving to .CSV file only or HTML for saving to .HTML only (Example: HTML)");
                outputFormat = ReadOutputFormat(Console.ReadLine());
            }

            Console.WriteLine("Please paste the directory's path, where the report will be saved without filename and fileextension (p.s.: Just Copy-Paste)");
            directoriesPath = Console.ReadLine();

            return new InputData(pins, minCredit, dateRangeFirst, dateRangeLast, directoriesPath) ;
        }

        static void Main(string[] args)
        {

            //InputData request = Request();

            InputData test = new("", 33, "2018-01-02", "2024-01-01", @"C:\Users\tatsi\OneDrive\Coding\haemoment\reports" );
            List<OutputData> output =  sqlQueries.requestStudentsAsView.ReplaceParamaters(test).GetQuery();
            int i = 0;
        }
    }
}
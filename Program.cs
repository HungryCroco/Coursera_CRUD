using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

// working SAT: 7pm - 11pm = 4h
// working SUN: 9am - 1pm  = 4h
// working SUN: 2pm - 5pm  = 3h
// total 11h
namespace MyApp // Note: actual namespace depends on the project name.
{
    /// <summary>
    /// Containing all the information about a Student.
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Student's identifyer. Unique.
        /// </summary>
        public string Pin { get; set; }
        /// <summary>
        /// Student's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Student's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Student's total time of the completed courses.
        /// </summary>
        public double TotalTime { get; set; }

        /// <summary>
        /// Student's total credits.
        /// </summary>
        public double TotalCredit { get; set; }

        /// <summary>
        /// A List containing all Courses done by a Student.
        /// </summary>
        public List<StudentCourse> Courses { get; set; }  


        public Student()
        {
            Courses = new ();
        }

        
    }

    /// <summary>
    /// Containing all the information about courses, taken by a Student.
    /// </summary>
    public struct StudentCourse
    {
        /// <summary>
        /// Information about the Course.
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Date, when the course was completed.
        /// </summary>
        public DateOnly ComletionDate{ get; set; }

    }

    /// <summary>
    /// Containing all the information about a course.
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Course Id. For now not used.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Course Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The instructor of the course.
        /// </summary>
        public Instructor Instructur { get; set; }

        /// <summary>
        /// Time, needed for complition of the course.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Credit, that will be earned after the complition of the course.
        /// </summary>
        public int Credit { get; set; }
    }

    /// <summary>
    /// Containing all the information about an instructor.
    /// </summary>
    public class Instructor
    {
        /// <summary>
        /// Instructor's id. For now not used.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Instructor's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Instructor's last name
        /// </summary>
        public string LastName { get; set; }
    }

    /// <summary>
    /// The format of the file, which will save the report.
    /// </summary>
    public enum OutputFormat
    {
        CSV,
        HTML,
        ALL
    }

    /// <summary>
    /// The Data, necessary to be inputed by the user to run a report.
    /// </summary>
    public class InputData
    {
        /// <summary>
        /// A List of Pins, separated by a comma. Example: 1232 , 3434 , 545455; The Report will be filtered to contain only Student included in this list if its != "";
        /// </summary>
        public string StudentPin { get; set; }

        /// <summary>
        /// The minimum credit earned by the Student, to be included in the report. Example: 43
        /// </summary>
        public double MinCredit { get; set; }

        /// <summary>
        /// The first date of the DateRange filtering the time, when the course was complited. Example: 1900-01-01
        /// </summary>
        public string DateRangeFirst { get; set; }

        /// <summary>
        /// The last date of the DateRange filtering the time, when the course was complited. Example: 2024-01-01
        /// </summary>
        public string DateRangeLast { get; set; }

        /// <summary>
        /// Directory's path without FileName, where the report will be saved. Example: C:\Users\tatsi\OneDrive\Coding\haemoment\reports2
        /// </summary>
        public string DirectorysPath { get; set; }

        /// <summary>
        /// The Format of the output file. CSV, HTML or both.
        /// </summary>
        public OutputFormat OutputFormat { get; set; }

        /// <summary>
        /// The Data, necessary to be inputed by the user to run a report.
        /// </summary>
        public InputData()
        {
        }

        /// <summary>
        /// The Data, necessary to be inputed by the user to run a report.
        /// </summary>
        /// <param name="studenPin">A List of Pins, separated by a comma. Example: 1232 , 3434 , 545455; The Report will be filtered to contain only Student included in this list if its != "";</param>
        /// <param name="minCredit">The minimum credit earned by the Student, to be included in the report. Example: 43</param>
        /// <param name="dateRangeFirst">The first date of the DateRange filtering the time, when the course was complited. Example: 2024-01-01</param>
        /// <param name="dateRangeLast">The last date of the DateRange filtering the time, when the course was complited. Example: 1900-01-01</param>
        /// <param name="directorysPath">Directory's path without FileName, where the report will be saved. Example: C:\Users\tatsi\OneDrive\Coding\haemoment\reports2</param>
        /// <param name="outputFormat">The Format of the output file. CSV, HTML or both.</param>
        public InputData(string studenPin, double minCredit, string dateRangeFirst, string dateRangeLast, string directorysPath, OutputFormat outputFormat)
        {
            StudentPin = studenPin;
            MinCredit = minCredit;
            DateRangeFirst = dateRangeFirst;
            DateRangeLast = dateRangeLast;
            DirectorysPath = directorysPath;
            OutputFormat = outputFormat;

            
        }
    }

    /// <summary>
    /// The output Data, necessary to run a report. Will be extracted from the SQL; Used only for the SQL-Variant of the App.
    /// </summary>
    public class OutputData
    {
        /// <summary>
        /// Student's identifyer. Unique.
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// Student's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Student's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Name of the completed course
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Time needed by the course, to be completed.
        /// </summary>
        public int CourseTime { get; set; }

        /// <summary>
        /// Credits earned by the course when completed.
        /// </summary>
        public int CourseCredit { get; set; }

        /// <summary>
        /// Total credits earned by the Student.
        /// </summary>
        public int TotalCourseCredit { get; set; }

        /// <summary>
        /// Full name of the course's instructor. 
        /// </summary>
        public string InstructorFullName { get; set; }

        

    }

    /// <summary>
    /// This Methods are used from the CodeFirst-part of the App to calculate the data, that was not directly extracted from the DB.
    /// </summary>
    public static class StudentExtensionMethods
    {
        /// <summary>
        /// Calculates the data, that was not directly extracted from the DB.
        /// </summary>
        /// <param name="students">List of Students, containing all students.</param>
        /// <param name="data">InputData, containing the filters</param>
        public static void CalculatePlayerProperties(this List<Student> students, InputData data)
        {
            //students.CalculateTotalTime(data);
            students.CalculateTotalCredit(data);
        }

        /// <summary>
        /// Filtering out all the Students, which were not included in the PIN's list by the user. 
        /// TO_DO: refactore.
        /// </summary>
        /// <param name="students">List of Students, containing all students.</param>
        /// <param name="data">InputData, containing the filters</param>
        /// <returns>A new List of Students, containing only the Students which pins were included in the search.</returns>
        public static List<Student> FilterByPins(this List<Student> students, InputData data)
        {
            // Edit the user's input to not contain spaces;
            // check if the Student is included in the list and if yes - add to the included student's list;

            List<Student> filteredList = new();

            string[] pins = data.StudentPin.Split(',');
            for (int i = 0; i < pins.Length; i++)
            {
                pins[i] = pins[i].Replace(" ", "");
            }

            foreach (var student in students)
            {
 
                if (pins.Contains(student.Pin))
                {
                    filteredList.Add(student);
                }
                else if (data.StudentPin == "")
                {
                    filteredList.Add(student);
                }
                
            }

            return filteredList;
        }

        /// <summary>
        /// This Method is optional, not needed for the output so far, but generally would be needed. Calculating the Total Time spend by the student for all completed courses.
        /// </summary>
        /// <param name="students">List of Students, containing all students.</param>
        /// <param name="data">InputData, containing the filters</param>
        private static void CalculateTotalTime(this List<Student> students, InputData data)
        {
            foreach (var student in students)
            {
                foreach (var course in student.Courses)
                {
                    if (course.ComletionDate > DateOnly.Parse(data.DateRangeFirst) && course.ComletionDate < DateOnly.Parse(data.DateRangeLast))
                    {
                        student.TotalTime += course.Course.Time;
                    }  
                }
            }
        }

        /// <summary>
        /// Calculates the total amount of credit earned by the student.
        /// </summary>
        /// <param name="students">List of Students, containing all students.</param>
        /// <param name="data">InputData, containing the filters</param>
        private static void CalculateTotalCredit(this List<Student> students, InputData data)
        {
            foreach (var student in students)
            {
                foreach (var course in student.Courses)
                {
                    if (course.ComletionDate > DateOnly.Parse(data.DateRangeFirst) && course.ComletionDate < DateOnly.Parse(data.DateRangeLast))
                    {
                        student.TotalCredit += course.Course.Credit;
                    }
                }
            }
        }
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
        private static string db = "coursera2";

        /// <summary>
        /// Writes a connection string to the Database.
        /// </summary>
        /// <returns>connection string to the Database.</returns>
        public static string GetConnectionStringToDb()
        {
            string connString = string.Format(@"Host={0};Port={1};User Id={2};Password={3};Database={4}", server, port, user, pass, db);

            return connString;
        }

        /// <summary>
        /// Writes a connection string to the Server.
        /// </summary>
        /// <returns>connection string to the Server.</returns>
        public static string GetConnectionStringToServer()
        {
            string connString = string.Format(@"Host={0};Port={1};User Id={2};Password={3};", server, port, user, pass);

            return connString;
        }
    }

    /// <summary>
    /// Containing all the necessary data to connect to PostgreSQL and perform CRUD operations.
    /// </summary>
    public static class SQL_Connector
    {
        /// <summary>
        /// Creates the Database.
        /// </summary>
        public static void CreateDatabase()
        {
            NpgsqlConnection conn = new();


            try
            {
                conn = new NpgsqlConnection(GlobalConfig.GetConnectionStringToServer());
                var cmd_createDB = new NpgsqlCommand(sqlQueries.CreateDatabase, conn);


                conn.Open();
                cmd_createDB.ExecuteNonQuery();
                conn.Close();

                conn = new NpgsqlConnection(GlobalConfig.GetConnectionStringToDb());

 
                var cmd_CreateTables = new NpgsqlCommand(sqlQueries.createTables, conn);


                conn.Open();
                cmd_CreateTables.ExecuteNonQuery();


            }
            catch (Exception)
            {
            }
            conn.Close();
        }

        /// <summary>
        /// Inserts the sample data to the DB.
        /// </summary>
        public static void InsertToDatabase()
        {
            NpgsqlConnection conn = new();

            try
            {
                conn = new NpgsqlConnection(GlobalConfig.GetConnectionStringToDb());

 
                var cmd_Insert = new NpgsqlCommand(sqlQueries.InsertToDatabase, conn);


                conn.Open();
                cmd_Insert.ExecuteNonQuery();


            }
            catch (Exception)
            {
            }
            conn.Close();
        }

        /// <summary>
        /// Performs a sql query.Used only to run the SQL-Part of the App.
        /// </summary>
        /// <param name="sqlQuery">The query, that will be performed without parameters.</param>
        /// <returns> A List of OutputData, necessary to run the SQL-part of the App.</returns>
        public static List<OutputData> GetQuery(this string sqlQuery)
        {
            List<OutputData> output = new();

            using (var conn = new NpgsqlConnection(GlobalConfig.GetConnectionStringToDb()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlQuery, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //SELECT st.pin AS pin,
                            //st.first_name AS first_name,
                            //st.last_name AS last_name,
                            //co.name AS course_name,
                            //co.total_time AS course_time,
                            //co.credit AS course_credit,
                            //CONCAT(ins.first_name, ' ', ins.last_name) AS instructor,
                            //total.tcredit AS total_credit
                            OutputData currRow = new();
                            { 
                                currRow.Pin = reader.GetString(0);
                                currRow.FirstName = reader.GetString(1);
                                currRow.LastName = reader.GetString(2);
                                currRow.CourseName = reader.GetString(3);
                                currRow.CourseCredit = reader.GetInt32(4);
                                currRow.CourseTime = reader.GetInt32(5);
                                currRow.InstructorFullName = reader.GetString(6);
                                currRow.TotalCourseCredit = reader.GetInt32(7);

                            }
    

                            output.Add(currRow);
                        }
                    }

                }
                conn.Close();
            }

            return output;
        }

        /// <summary>
        /// Performs a sql query.Used only to run the CodeFirst-Part of the App.
        /// </summary>
        /// <param name="sqlQuery">The query, that will be performed without parameters.</param>
        /// <returns>A List of Students, needed to run the CodeFirst part of the App.</returns>
        public static List<Student> GetQueryAsStudetsList(this string sqlQuery)
        {
            List<Student> students = new();
            using (var conn = new NpgsqlConnection(GlobalConfig.GetConnectionStringToDb()))
            {
                
                conn.Open();
                using (var cmd = new NpgsqlCommand(sqlQuery, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Student currStudent = new();
                            Course currCourse = new ();
                            StudentCourse currStudentCourse = new();
                            Instructor currInstructor = new ();
                            {
                                //0 - st.pin as pin,
                                //1 - st.first_name AS first_name,
                                //2 - st.last_name AS last_name,
                                //3 - co.Id AS course_id,
                                //4 - co.name AS course_name,
                                //5 - co.total_time AS course_time,
                                //6 - co.credit AS course_credit,
                                //7 - sc.completion_date AS completion_date,
                                //8 - ins.Id AS inst_id,
                                //9 - ins.first_name AS inst_first_name,
                                //10 - ins.first_name AS inst_last_name

                                currStudent.Pin = reader.GetString(0);
                                currStudent.FirstName = reader.GetString(1);
                                currStudent.LastName = reader.GetString(2);

                                currCourse.Id = reader.GetInt32(3);
                                currCourse.Name = reader.GetString(4);
                                currCourse.Credit = reader.GetInt32(5);
                                currCourse.Time = reader.GetInt32(6);

                                currStudentCourse.ComletionDate = DateOnly.Parse(reader.GetString(7));
                                currInstructor.Id = reader.GetInt32(8);
                                currInstructor.FirstName= reader.GetString(9);
                                currInstructor.LastName = reader.GetString(10);

                                currCourse.Instructur = currInstructor;
                                currStudentCourse.Course = currCourse;
                                currStudent.Courses.Add(currStudentCourse);

                                //If the student is not present in the list, add him.
                                //If a student is present in the List, just add the new course.
                                Student matchingStudent = students.Find(s => s.Pin == currStudent.Pin);
                                if (matchingStudent != null)
                                { 
                                    for (int i = 0; i < students.Count; i++)
                                    {
                                        if (students[i].Pin == currStudent.Pin)
                                        {
                                            // We could continue checking for unique IDs here, i'll leave it like that for now cause it's not logical to have 2 same courses completed by the same Student and it's not written what to do if it happens;
                                            // However, i did extract the Ids to be able to check for unique data, as it's important in most cases.
                                            students[i].Courses.Add(currStudentCourse);
                                        }
                                    }
                                }
                                else
                                {
                                    students.Add(currStudent);
                                }


                            }
                            
                        }
                    }

                }
                conn.Close();
            }

            return students;
        }


    }

    /// <summary>
    /// Containing Extension Methots for parametrizing the SQL queries.
    /// </summary>
    public static class QueriesExtensionMethots
    {
        /// <summary>
        /// Replacing the parameters in the query.
        /// </summary>
        /// <param name="query">Query with parameters.</param>
        /// <param name="minCredit">The minimum Credit necessary for a Student to be included in the search.</param>
        /// <param name="dateRangeFirst">The first date of the DateRange filtering the time, when the course was complited. Example: 2001-01-01</param>
        /// <param name="dateRangeLast">The last date of the DateRange filtering the time, when the course was complited. Example: 2024-01-01</param>
        /// <param name="pinList">A List of Pins, separated by a comma. Example: 1232 , 3434 , 545455; The Report will be filtered to contain only Student included in this list if its != ""; </param>
        /// <returns>A query with  replaced parameters.</returns>
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

        /// <summary>
        /// Replacing the parameters in the query. Used for the CodeFirst-part of the App.
        /// </summary>
        /// <param name="query">Query with parameters.</param>
        /// <param name="data">The inputData from the user, containing the filters for the search.</param>
        /// <returns>A query with  replaced parameters.</returns>
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
                //TO DO: Refactore.
                //Deletes spaces, surrounds the pin with 'pin', adds a comma between the pins and saves to a string.
                if (data.StudentPin != "")
                {
                    string[] pins = data.StudentPin.Split(',');
                    string[] editedPins = new string[pins.Length];
                    string editedPinsAsString = "";

                    for (int i = 0; i < pins.Length; i++)
                    {
                        pins[i] = pins[i].Replace(" ", "");
                    }

                    for (int i = 0; i < pins.Length; i++)
                    {
                        editedPins[i] = "'" + pins[i] + "'";
                    }

                    editedPinsAsString = string.Join(',', editedPins);
                    output = output.Replace("@pinList", editedPinsAsString);
                }
                else
                {
                    output = output.Replace("@pinList", "SELECT pin FROM students");
                }

            }
            

            return output;
        }
    }
    /// <summary>
    /// This class is containing all the necessary sql queries.
    /// </summary>
    public static class sqlQueries
    {
        /// <summary>
        /// A query returning a Dataview of all Students and courses, done by them. No logic in the SQL. Used for the CodeFirst-part of the App.
        /// </summary>
        public static string requestAllStudentsView =
            @"SELECT st.pin as pin, st.first_name AS first_name,  st.last_name AS last_name, co.Id AS course_id,  co.name AS course_name, co.total_time AS course_time, co.credit AS course_credit, sc.completion_date::text AS completion_date, ins.Id AS inst_id, ins.first_name AS inst_first_name, ins.last_name AS inst_last_name
	            FROM students st
		            INNER JOIN public.students_courses_xref sc ON sc.student_pin = st.pin
		            INNER JOIN public.courses co ON co.id = sc.course_id
		            INNER JOIN public.instructors ins ON ins.id = co.instructor_id
				ORDER BY pin";

        /// <summary>
        /// A query filtering the Students in Postgre and returning a Dataview with the OutputData. Used from the SQL-part of the app. Almost all of the logic is done in the SQL.
        /// </summary>
        public static string requestFilteredStudentsView =
            @"SELECT st.pin AS pin, st.first_name AS first_name,  st.last_name AS last_name,  co.name AS course_name, co.total_time AS course_time, co.credit AS course_credit, CONCAT (ins.first_name , ' ', ins.last_name) AS instructor , total.tcredit AS total_credit
	            FROM students st
		            INNER JOIN public.students_courses_xref sc ON sc.student_pin = st.pin
		            INNER JOIN public.courses co ON co.id = sc.course_id
		            INNER JOIN public.instructors ins ON ins.id = co.instructor_id
					INNER JOIN (
						SELECT st.pin AS pin, SUM(co.total_time) AS tcredit FROM students st
				            INNER JOIN public.students_courses_xref sc ON sc.student_pin = st.pin
				            INNER JOIN public.courses co ON co.id = sc.course_id
				            INNER JOIN public.instructors ins ON ins.id = co.instructor_id
			            WHERE pin  IN (@pinList) 
                            AND sc.completion_date BETWEEN DATE '2018-01-02' AND DATE '2024-01-01'
			            GROUP BY st.pin) total ON total.pin = st.pin

		        WHERE tcredit > 33 
		          
				ORDER BY st.pin";

        /// <summary>
        /// TO DO: DBname needs to be parametrized
        /// A query creating the DB;
        /// </summary>
        public static string CreateDatabase = @"-- Create the database
                                            CREATE DATABASE coursera2;";

        /// <summary>
        /// A query creating all tables in the DB;
        /// </summary>
        public static string createTables = @"-- Create the table ""courses""
                                            CREATE TABLE courses (
                                              id SERIAL PRIMARY KEY,
                                              name VARCHAR(150) NOT NULL,
                                              instructor_id INTEGER NOT NULL,
                                              total_time SMALLINT NOT NULL,
                                              credit SMALLINT NOT NULL,
                                              time_created TIMESTAMP NOT NULL DEFAULT NOW()
                                            );

                                            -- Create the table ""instructors""
                                            CREATE TABLE instructors (
                                              id SERIAL PRIMARY KEY,
                                              first_name VARCHAR(100) NOT NULL,
                                              last_name VARCHAR(100) NOT NULL,
                                              time_created TIMESTAMP NOT NULL DEFAULT NOW()
                                            );

                                            -- Create the table ""students""
                                            CREATE TABLE students (
                                              pin CHAR(10) PRIMARY KEY,
                                              first_name VARCHAR(50) NOT NULL,
                                              last_name VARCHAR(50) NOT NULL,
                                              time_created TIMESTAMP NOT NULL DEFAULT NOW()
                                            );

                                            -- Create the table ""students_courses_xref""
                                            CREATE TABLE students_courses_xref (
                                              student_pin CHAR(10) NOT NULL,
                                              course_id INTEGER NOT NULL,
                                              completion_date DATE,
                                              PRIMARY KEY (student_pin, course_id),
                                              FOREIGN KEY (student_pin) REFERENCES students (pin),
                                              FOREIGN KEY (course_id) REFERENCES courses (id)
                                            );";
        /// <summary>
        /// A Query importing the sampleData to the DB.
        /// </summary>
        public static string InsertToDatabase = @"-- Insert data into the ""courses"" table
                                            INSERT INTO courses (name, instructor_id, total_time, credit) VALUES
                                              ('Analysis', 1, 20, 10),
                                              ('Linear Algebra', 1, 30, 15),
                                              ('Statistics', 2, 30, 15),
                                              ('Geometry', 3, 35, 20);

                                            -- Insert data into the ""instructors"" table
                                            INSERT INTO instructors (first_name, last_name) VALUES
                                              ('Neno', 'Dimitrov'),
                                              ('Petko', 'Valchev'),
                                              ('Petar', 'Penchev');

                                            -- Insert data into the ""students"" table
                                            INSERT INTO students (pin, first_name, last_name) VALUES
                                              ('9412011005', 'Krasimir', 'Petrov'),
                                              ('9501011014', 'Elena', 'Foteva'),
                                              ('9507141009', 'Ivan', 'Ivanov');

                                            -- Insert data into the ""students_courses_xref"" table
                                            INSERT INTO students_courses_xref (student_pin, course_id, completion_date) VALUES
                                              ('9412011005', 1, '2019-07-16'),
                                              ('9412011005', 2, '2019-08-20'),
                                              ('9501011014', 1, '2019-07-16'),
                                              ('9501011014', 2, '2019-08-01'),
                                              ('9501011014', 3, '2019-10-01'),
                                              ('9501011014', 4, '2019-12-05'),
                                              ('9507141009', 4, '2019-08-20');";
    }

    /// <summary>
    /// This clas is containing all of the code necessary to build the HTML page.
    /// </summary>
    public static class HtmlScripts
    {
        /// <summary>
        /// Create the Head.
        /// </summary>
        public static string htmlHead = @"<!DOCTYPE html>
                                        <html>
                                        <head>
                                            <title>Student Information</title>
                                            <style>
                                                table {
                                                    border-collapse: collapse;
                                                    width: 100%;
                                                }
                                                th, td {
                                                    border: 1px solid black;
                                                    padding: 8px;
                                                    text-align: left;
                                                }
                                            </style>
                                        </head>
                                        <body>
                                            <h1>Student Information</h1>
                                            <table>
                                                <tr>
                                                    <tr style='background-color: #FFCCBB;'>
                                                    <th>Student</th>
                                                    <th>Total Credit</th>
                                                    <th> </th>
                                                    <th> </th>
                                                    <th> </th>
                                                </tr>                        
                                                <tr>
                                                    <tr style='background-color: #FFCCBB;
                                                    <th> </th>
                                                    <th>Course Name</th>
                                                    <th>Time</th>
                                                    <th>Credit</th>
                                                    <th>Instructor</th>
                                                </tr>";

        /// <summary>
        /// Insert a Student.
        /// </summary>
        public static string htmlStudentTable = @"
                                                <tr>
                                                    <tr style='background-color: #EECC77;'>
                                                    <td>{0}</td>
                                                    <td>{1}</td>
                                                    <td> </td>
                                                    <th> </th>
                                                    <td> </td>
                                                </tr>";

        /// <summary>
        /// Insert the Courses, done by the student.
        /// </summary>
        public static string htmlCourseTable = @"<tr>
                                                    <tr style='background-color: #FAF1DA;'>
                                                    <th> </th>
                                                    <td>{0}</td>
                                                    <td>{1}</td>
                                                    <td>{2}</td>
                                                    <td>{3}</td>
                                                </tr>";
    }

    /// <summary>
    /// This class is containing all the Methods required to Convert the data to the desired format and save to a file.
    /// TO DO: Refactore.
    /// </summary>
    public static class File_Connector
    {
        /// <summary>
        /// CodeFirst Method to save a Student's list to CSV.
        /// </summary>
        /// <param name="fullFilePath"> Directory's path.</param>
        /// <param name="fileName">The full name + extension of the file.</param>
        /// <param name="students"> A list of students, that will be converted and saved.</param>
        public static void WriteOutputDataToCsvFile(string fullFilePath, string fileName, List<Student> students)
        {
            try
            {
                //// Check if file already exists. If yes, delete it.     
                if (File.Exists(fullFilePath))
                {
                    //Delete if the file is existing;
                    File.Delete(fullFilePath);
                }

                // Create a new file
                // Convert to CSV format
                // Save to the file
                using (StreamWriter sw = File.CreateText(fullFilePath + "\\" + fileName))
                {

                    sw.WriteLine("Student , ToatalCredit");
                    sw.WriteLine(" , Course Name, Time, Credit, Instructor");

                    string currLine = "";
                    List<string> pins = new();
                    foreach (var student in students)
                    {
                            currLine = string.Join(" , ", student.FirstName + " " + student.LastName, student.TotalCredit);
                            sw.WriteLine(currLine);

                        foreach (var course in student.Courses)
                        {
                            currLine = string.Join(" , ", "", course.Course.Name, course.Course.Time, course.Course.Credit, course.Course.Instructur.FirstName + " " + course.Course.Instructur.LastName);
                            sw.WriteLine(currLine);
                        }

                    }

                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        /// <summary>
        /// CodeFirst Method to save a Student's list to HTML.
        /// </summary>
        /// <param name="fullFilePath"> Directory's path.</param>
        /// <param name="fileName">The full name + extension of the file.</param>
        /// <param name="students"> A list of students, that will be converted and saved.</param>
        public static void WriteOutputDataToHtmlFile(string fullFilePath, string fileName, List<Student> students)
        {
            try
            {
                //// Check if file already exists. If yes, delete it.     
                if (File.Exists(fullFilePath))
                {
                    //Delete if the file is existing;
                    File.Delete(fullFilePath);
                }

                // Create a new file
                // Convert to HTML format
                // Save to the file  
                using (StreamWriter sw = File.CreateText(fullFilePath + "\\" + fileName))
                {

                    sw.WriteLine(HtmlScripts.htmlHead);
                    string currLine = "";

                    foreach (var student in students)
                    {

                        sw.WriteLine(String.Format(HtmlScripts.htmlStudentTable, student.FirstName + " " + student.LastName, student.TotalCredit));
                        foreach (var course in student.Courses)
                        {
                            sw.WriteLine(String.Format(HtmlScripts.htmlCourseTable, course.Course.Name, course.Course.Time, course.Course.Credit, course.Course.Instructur.FirstName + " " + course.Course.Instructur.LastName));
                        }

                    }
                    

                    

                    sw.WriteLine("</table>\r\n</body>\r\n</html>");

                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        /// <summary>
        /// SQL Method to save an outputsdatalist's list to CSV.
        /// </summary>
        /// <param name="fullFilePath"> Directory's path.</param>
        /// <param name="fileName">The full name + extension of the file.</param>
        /// <param name="outputsList">A list of outputdata, that will be converted and saved.</param>
        public static void WriteOutputDataToCsvFile(string fullFilePath, string fileName, List<OutputData> outputsList)
        {
            try
            {
                //// Check if file already exists. If yes, delete it.     
                if (File.Exists(fullFilePath))
                {
                    //Delete if the file is existing;
                    File.Delete(fullFilePath);
                }

                // Create a new file
                // Convert to CSV format
                // Save to the file     
                using (StreamWriter sw = File.CreateText(fullFilePath + "\\" + fileName))
                {

                    sw.WriteLine("Student , ToatalCredit");
                    sw.WriteLine(" , Course Name, Time, Credit, Instructor");

                    string currLine = "";
                    List<string> pins = new();
                    foreach (var output in outputsList)
                    {
                        if (!pins.Contains(output.Pin))
                        {
                            
                            pins.Add(output.Pin);
                            currLine = string.Join(" , ", output.FirstName + " " + output.LastName, output.TotalCourseCredit);
                            sw.WriteLine(currLine);
                        }

                            currLine = string.Join(" , ", "", output.CourseName, output.CourseTime, output.CourseCredit, output.InstructorFullName);
                            sw.WriteLine(currLine);
                        

                    }
                    
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        /// <summary>
        /// SQL Method to save an outputsdatalist's list to HTML.
        /// </summary>
        /// <param name="fullFilePath"> Directory's path.</param>
        /// <param name="fileName">The full name + extension of the file.</param>
        /// <param name="outputsList">A list of outputdata, that will be converted and saved.</param>
        public static void WriteOutputDataToHtmlFile(string fullFilePath, string fileName, List<OutputData> outputsList)
        {
            try
            {
                //// Check if file already exists. If yes, delete it.     
                if (File.Exists(fullFilePath))
                {
                    //Delete if the file is existing;
                    File.Delete(fullFilePath);
                }

                // Create a new file
                // Convert to HTML format
                // Save to the file     
                using (StreamWriter sw = File.CreateText(fullFilePath + "\\" + fileName))
                {

                    sw.WriteLine(HtmlScripts.htmlHead);
                    string currLine = "";
                    List<string> pins = new();
                    foreach (var output in outputsList)
                    {
                        if (!pins.Contains(output.Pin))
                        {
                            pins.Add(output.Pin);

                            sw.WriteLine(String.Format(HtmlScripts.htmlStudentTable, output.FirstName + " " + output.LastName, output.TotalCourseCredit));

                        }

                        sw.WriteLine(String.Format(HtmlScripts.htmlCourseTable, output.CourseName, output.CourseTime, output.CourseCredit, output.InstructorFullName));

                    }

                    sw.WriteLine("</table>\r\n</body>\r\n</html>");

                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

    }

    /// <summary>
    /// This class is containing all filters required to run a new query.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Converting the user's input tp OutputFormat
        /// </summary>
        /// <param name="outputFormat">outputFormat as user's input.</param>
        /// <returns>outputFormat as ENUM.</returns>
        private static OutputFormat ReadOutputFormat(string outputFormat)
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

        /// <summary>
        /// Console WL/RL Method asking and validating a list of pins.
        /// </summary>
        /// <returns>A string containing a list of pins, separated by a comma. Example: 123 , 1233, 343</returns>
        private static string InputPins()
        {
            string output = "";
            Console.WriteLine();
            Console.WriteLine("(y/n) Would you like to search based on student's PINs.?  (If not, the Query will select all students from the DB).");
            if (Console.ReadLine().ToLower() == "y")
            {
                Console.WriteLine("Please enter the student's PINs, separated by a comma ',' (Example 876555 , 945532, 445465).");
                output = Console.ReadLine();

            }
            return output;
        }

        /// <summary>
        /// Console WL/RL Method asking and validating the minimum Credit needed to include a Student in the search.
        /// </summary>
        /// <returns>minimum Credit needed to include a Student in the search. Example: 33</returns>
        private static int InputMinCredit()
        {
            int minCredit = 0;
            bool isValidInput = false;

            while (!isValidInput)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the required minimum credit: (Example: 10)");
                string input = Console.ReadLine();

                if (int.TryParse(input, out minCredit))
                {
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
            }

            return minCredit;
        }

        /// <summary>
        /// Console WL/RL Method asking and validating the first Data of the date range in which the course needs to be finished to earn credits.
        /// </summary>
        /// <returns>the first Data of the date range in which the course needs to be finished to earn credits. Example: 2011-01-01</returns>
        private static string InputDateRangeFirst()
        {

            
            DateOnly output;
            bool isValidInput = false;

            while (!isValidInput)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the starting date of the time range: (Example: 2020-01-01)"); 
                string input = Console.ReadLine();

                if (DateOnly.TryParse(input, out output))
                {
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid date.");
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Console WL/RL Method asking and validating the last Data of the date range in which the course needs to be finished to earn credits.
        /// </summary>
        /// <returns>the last Data of the date range in which the course needs to be finished to earn credits. Example: 2044-01-01</returns>
        private static string InputDateRangeLast()
        {


            DateOnly output;
            bool isValidInput = false;

            while (!isValidInput)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the end date of the time range: (Example: 2022-01-01)");
                string input = Console.ReadLine();

                if (DateOnly.TryParse(input, out output))
                {
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid date.");
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Console WL/RL Method asking and validating the output format, in which the report will be saved.
        /// </summary>
        /// <returns> HTM / CSV / ALL</returns>
        private static OutputFormat InputOutputFormat()
        {
            OutputFormat outputFormat = OutputFormat.ALL;
            Console.WriteLine();
            Console.WriteLine("(y/n) Would You like to specify an output format? ( If not, the report will be saved in both .CSV and .HTML files.");
            string response = Console.ReadLine().ToLower();
            if (response == "y")
            {
                Console.WriteLine();
                Console.WriteLine("Please enter CSV for saving to .CSV file only or HTML for saving to .HTML only (Example: HTML)");
                response = Console.ReadLine().ToUpper();
                if (response == "HTML" || response == "CSV")
                {
                    outputFormat = ReadOutputFormat(response);
                }
                else
                {
                    Console.WriteLine("Wrong OutputFormat. The Report will be saved to all formats.");
                    outputFormat = OutputFormat.ALL;
                }
                
            }


            return outputFormat;
        }

        /// <summary>
        /// Console WL/RL Method asking and validating the Directory's path, where the reports will be saved.
        /// </summary>
        /// <returns>A string containing the directory's path without the file name.</returns>
        private static string InputDirectorysPath()
        {


            string output = "";
            bool isValidInput = false;

            while (!isValidInput)
            {
                Console.WriteLine();
                Console.WriteLine("Please paste the directory's path, where the report will be saved without filename and fileextension (p.s.: Just Copy-Paste)");
                output = Console.ReadLine();

                if (Directory.Exists(output))
                {
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid directory");
                }
            }

            return output;
        }

        /// <summary>
        /// Console WL/RL Method asking the user about all the filters, needed to perform a query.
        /// </summary>
        /// <returns>A full set of input's data</returns>
        public static InputData Request()
        {
                  
            Console.WriteLine("Hello. To create a new request, please fill in the form: ");
            string pins = InputPins();

            int minCredit = InputMinCredit();

         
            string dateRangeFirst = InputDateRangeFirst();


            string dateRangeLast = InputDateRangeLast();

            OutputFormat outputFormat = InputOutputFormat();

            string directoriesPath = InputDirectorysPath();

            return new InputData(pins, minCredit, dateRangeFirst, dateRangeLast, directoriesPath, outputFormat);
        }
    }

        internal class Program
    {


        static void Main(string[] args)
        {
            // Create the Database if not created.
            SQL_Connector.CreateDatabase();

            //Insert the datasample if not inserted.
            SQL_Connector.InsertToDatabase();


            // Ask the user for the required filters in the Console.
            InputData request = Input.Request();
            //InputData request = new("9412011005 , 9501011014", 33, "2018-01-02", "2024-01-01", @"C:\Users\tatsi\OneDrive\Coding\haemoment\reports2" , OutputFormat.ALL);
            //InputData request = new("", 33, "2018-01-02", "2024-01-01", @"C:\Users\tatsi\OneDrive\Coding\haemoment\reports2", OutputFormat.ALL);

            // Perform SQL query to get the Outputdata. SQL-Version.
            List<OutputData> outputSQL = sqlQueries.requestFilteredStudentsView.ReplaceParamaters(request).GetQuery();

            // Perform SQL query to get List<Student>. CodeFirst-Version.
            var outputCodeFirst = sqlQueries.requestAllStudentsView.GetQueryAsStudetsList();

            // Calculates the properties, that were not extracted from the SQL and edit's the List<Students>. CodeFirst-Version.
            outputCodeFirst.CalculatePlayerProperties(request);

            // Filters out all Students, that were not selected in the PIN's list; CodeFirst-Version.
            var filteredOutputCodeFirst = outputCodeFirst.FilterByPins(request);

            // Converting the Outputdata and saving reports to the directory.
            if (request.OutputFormat == OutputFormat.CSV || request.OutputFormat == OutputFormat.ALL)
            {
                File_Connector.WriteOutputDataToCsvFile(request.DirectorysPath, "reportSql.csv", outputSQL);
                
                File_Connector.WriteOutputDataToCsvFile(request.DirectorysPath, "reportCodeFirst.csv", filteredOutputCodeFirst);
                
            }
            if (request.OutputFormat == OutputFormat.HTML|| request.OutputFormat == OutputFormat.ALL)
            {
                File_Connector.WriteOutputDataToHtmlFile(request.DirectorysPath, "reportSql.html", outputSQL);
                File_Connector.WriteOutputDataToHtmlFile(request.DirectorysPath, "reportCodeFirst.html", filteredOutputCodeFirst);
            }


        }
    }
}
using Microsoft.Data.Sqlite;

public static class DatabaseHelper
{
    private const string ConnectionString = "Data Source=students.db";

    public static void Initialize() // Esta función crea la base de datos si no existe
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Students (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                CurrentGoal TEXT,
                StartDate TEXT,
                BeginPeriod TEXT,
                EndPeriod TEXT,
                CurrentLevel TEXT,
                LessonCount REAL,
                HasHomework INTEGER,
                HomeworkSent INTEGER,
                ReviewPending INTEGER,
                ReviewReceived INTEGER,
                EvaluationPending INTEGER,
                EvaluationTaken INTEGER,
                EvaluationAverage REAL
            );

            CREATE TABLE IF NOT EXISTS EvaluationResults (
                StudentId INTEGER,
                EvaluationDate TEXT,
                Score REAL,
                PRIMARY KEY (StudentId, EvaluationDate),
                FOREIGN KEY (StudentId) REFERENCES Students(Id)
            );

            CREATE TABLE IF NOT EXISTS LearningHistory (
                StudentId INTEGER,
                EntryId INTEGER,
                Description TEXT,
                PRIMARY KEY (StudentId, EntryId),
                FOREIGN KEY (StudentId) REFERENCES Students(Id)
            );";

        command.ExecuteNonQuery();
    }

    /*public static void AddStudent(Student s)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open(); // abre una conexión a la base de datos

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Students (Id, Name, CurrentGoal, StartDate, CurrentLevel, LessonCount, HasHomework, EvaluationAverage)
";

    }*/

}
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

    public static void AddStudent(Student s) // Esta función agregará a un estudiante a la base de datos
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open(); // abre una conexión a la base de datos

        // Esto insertará el record del estudiante
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Students (
                Id, Name, CurrentGoal, StartDate, BeginPeriod, EndPeriod,
                CurrentLevel, LessonCount, HasHomework, HomeworkSent,
                ReviewPending, ReviewReceived, EvaluationPending,
                EvaluationTaken, EvaluationAverage)
            VALUES (
                $id, $name, $goal, $start, $begin, $end, $level,
                $lessons, $hw, $hwsent, $revpending, $revreceived, 
                $evalpending, $evaltaken, $avg);";

        command.Parameters.AddWithValue("$id", s.Id);
        command.Parameters.AddWithValue("$name", s.Name);
        command.Parameters.AddWithValue("$goal", s.CurrentGoal ?? ""); // si CurrentGoal está nulo, se usara "", si no, se usa el valor
        command.Parameters.AddWithValue("$start", s.StartDate.ToString("yyy-MM-dd")); // convierte la fecha a este formato. Mayúscula y minúscula importan
        command.Parameters.AddWithValue("$begin", s.BeginPeriod.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$end", s.EndPeriod.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$level", s.CurrentLevel ?? "");
        command.Parameters.AddWithValue("$lessons", s.LessonCount);
        command.Parameters.AddWithValue("$hw", s.HasHomework ? 1 : 0); // operador ternario. Si es verdadero, se asigna 1, si no, 0
        command.Parameters.AddWithValue("$hwsent", s.HomeworkSent ? 1 : 0);
        command.Parameters.AddWithValue("$revpending", s.ReviewPending ? 1 : 0);
        command.Parameters.AddWithValue("$revreceived", s.ReviewReceived ? 1 : 0);
        command.Parameters.AddWithValue("$evalpending", s.EvaluationPending ? 1 : 0);
        command.Parameters.AddWithValue("evaltaken", s.EvaluationTaken ? 1 : 0);
        command.Parameters.AddWithValue("$avg", s.EvaluationAverage);
        command.ExecuteNonQuery();

        // Inserta los resultados de evaluaciones
        if (s.EvaluationResults != null)
        {
            foreach (var entry in s.EvaluationResults)
            {
                var evalCmd = connection.CreateCommand();
                evalCmd.CommandText = @"
                    INSERT INTO EvaluationResults (StudentId, EvaluationDate, Score)
                    VALUES($sid, $date, $score);";
                evalCmd.Parameters.AddWithValue("$sid", s.Id);
                evalCmd.Parameters.AddWithValue("$date", entry.Key.ToString("yyyy-MM-dd"));
                evalCmd.Parameters.AddWithValue("$score", entry.Value);
                evalCmd.ExecuteNonQuery();
            }
        }

        // Esto inserta el registro de aprendizaje del estudiante
        if (s.LearningHistory != null)
        {
            foreach (var entry in s.LearningHistory)
            {
                var histCmd = connection.CreateCommand();
                histCmd.CommandText = @"
                    INSERT INTO LearningHistory (StudentId, EntryId, Description)
                    VALUES ($sid, $entryid, $desc);";
                histCmd.Parameters.AddWithValue("$sid", s.Id);
                histCmd.Parameters.AddWithValue("$entryid", entry.Key);
                histCmd.Parameters.AddWithValue("$desc", entry.Value);
                histCmd.ExecuteNonQuery();
            }
        }
    }

}
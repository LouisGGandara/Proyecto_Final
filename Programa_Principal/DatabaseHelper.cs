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
                EvaluationAverage REAL,
                IsArchived INTEGER DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS EvaluationResults (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER,
                EvaluationDate TEXT,
                Score REAL,
                BeginPeriod TEXT,
                EndPeriod TEXT,
                FOREIGN KEY (StudentId) REFERENCES Students(Id)
            );

            CREATE TABLE IF NOT EXISTS LearningHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER,
                EntryId INTEGER,
                Description TEXT,
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
                    INSERT INTO EvaluationResults (StudentId, EvaluationDate, Score, BeginPeriod, EndPeriod)
                    VALUES($sid, $date, $score, $begin, $end);";
                evalCmd.Parameters.AddWithValue("$sid", s.Id);
                evalCmd.Parameters.AddWithValue("$date", entry.EvaluationDate.ToString("yyyy-MM-dd"));
                evalCmd.Parameters.AddWithValue("$score", entry.Score);
                evalCmd.Parameters.AddWithValue("$begin", entry.BeginPeriod.ToString("yyyy-MM-dd"));
                evalCmd.Parameters.AddWithValue("$end", entry.EndPeriod.ToString("yyyy-MM-dd"));
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
                    INSERT INTO LearningHistory (StudentId, Description)
                    VALUES ($sid, $desc);";
                histCmd.Parameters.AddWithValue("$sid", s.Id);
                histCmd.Parameters.AddWithValue("$desc", entry);
                histCmd.ExecuteNonQuery();
            }
        }
    }

    public static Student? GetStudent(long id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // Esto trae el recod principal del estudiante
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Students WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var student = new Student();
        student.Id = reader.GetInt64(reader.GetOrdinal("Id"));
        student.Name = reader.GetString(reader.GetOrdinal("Name"));
        student.CurrentGoal = reader.GetString(reader.GetOrdinal("CurrentGoal"));
        student.StartDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("StartDate")));
        student.BeginPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("BeginPeriod")));
        student.EndPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("EndPeriod")));
        student.CurrentLevel = reader.GetString(reader.GetOrdinal("CurrentLevel"));
        student.LessonCount = reader.GetDouble(reader.GetOrdinal("LessonCount"));
        student.HasHomework = reader.GetInt32(reader.GetOrdinal("HasHomework")) == 1;
        student.HomeworkSent = reader.GetInt32(reader.GetOrdinal("HomeworkSent")) == 1;
        student.ReviewPending = reader.GetInt32(reader.GetOrdinal("ReviewPending")) == 1;
        student.ReviewReceived = reader.GetInt32(reader.GetOrdinal("ReviewReceived")) == 1;
        student.EvaluationPending = reader.GetInt32(reader.GetOrdinal("EvaluationPending")) == 1;
        student.EvaluationTaken = reader.GetInt32(reader.GetOrdinal("EvaluationTaken")) == 1;
        student.EvaluationAverage = reader.GetDouble(reader.GetOrdinal("EvaluationAverage"));


        // Esto trae los resultados de las evaluaciones
        var evalCmd = connection.CreateCommand();
        evalCmd.CommandText = "SELECT * FROM EvaluationResults WHERE StudentId = $id;";
        evalCmd.Parameters.AddWithValue("$id", id);

        student.EvaluationResults = new List<EvaluationResult>();
        using var evalReader = evalCmd.ExecuteReader();
        while (evalReader.Read())
        {
            student.EvaluationResults.Add(new EvaluationResult
            {
                EvaluationDate = DateOnly.Parse(evalReader.GetString(evalReader.GetOrdinal("EvaluationDate"))),
                Score = evalReader.GetDouble(evalReader.GetOrdinal("Score")),
                BeginPeriod = DateOnly.Parse(evalReader.GetString(evalReader.GetOrdinal("BeginPeriod"))),
                EndPeriod = DateOnly.Parse(evalReader.GetString(evalReader.GetOrdinal("EndPeriod")))
            });
        }

        // Esto trae la historia del aprendizaje
        var histCmd = connection.CreateCommand();
        histCmd.CommandText = "SELECT * FROM LearningHistory WHERE StudentId = $id;";
        histCmd.Parameters.AddWithValue("$id", id);

        student.LearningHistory = new List<string>();
        using var histReader = histCmd.ExecuteReader();
        while (histReader.Read())
        {
            student.LearningHistory.Add(histReader.GetString(histReader.GetOrdinal("Description")));
        }

        return student;

    }

    public static List<Student> GetAllStudents()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Students WHERE IsArchived = 0;";

        var students = new List<Student>();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var student = new Student();
            student.Id = reader.GetInt64(reader.GetOrdinal("Id"));
            student.Name = reader.GetString(reader.GetOrdinal("Name"));
            student.CurrentGoal = reader.GetString(reader.GetOrdinal("CurrentGoal"));
            student.StartDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("StartDate")));
            student.BeginPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("BeginPeriod")));
            student.EndPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("EndPeriod")));
            student.CurrentLevel = reader.GetString(reader.GetOrdinal("CurrentLevel"));
            student.LessonCount = reader.GetDouble(reader.GetOrdinal("LessonCount"));
            student.HasHomework = reader.GetInt32(reader.GetOrdinal("HasHomework")) == 1;
            student.HomeworkSent = reader.GetInt32(reader.GetOrdinal("HomeworkSent")) == 1;
            student.ReviewPending = reader.GetInt32(reader.GetOrdinal("ReviewPending")) == 1;
            student.ReviewReceived = reader.GetInt32(reader.GetOrdinal("ReviewReceived")) == 1;
            student.EvaluationPending = reader.GetInt32(reader.GetOrdinal("EvaluationPending")) == 1;
            student.EvaluationTaken = reader.GetInt32(reader.GetOrdinal("EvaluationTaken")) == 1;
            student.EvaluationAverage = reader.GetDouble(reader.GetOrdinal("EvaluationAverage"));
            student.EvaluationResults = new List<EvaluationResult>();
            student.LearningHistory = new List<string>();

            students.Add(student);
        }
        return students;
    }

    public static void IncrementLessonCount(long studentId, double amount)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Students
            SET LessonCount = LessonCount + $amount
            WHERE Id = $id;";
        command.Parameters.AddWithValue("$amount", amount);
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }

    public static void UpdateHomeworkStatus(long studentId, bool hasHomework, bool homeworkSent)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Students
            SET HasHomework = $hw, HomeworkSent = $hwsent
            WHERE Id = $id;";
        command.Parameters.AddWithValue("$hw", hasHomework ? 1 : 0);
        command.Parameters.AddWithValue("$hwsent", homeworkSent ? 1 : 0);
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }

    public static void UpdateReviewStatus(long studentId, bool reviewPending, bool reviewReceived)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Students
            SET ReviewPending = $pending, ReviewReceived = $received
            WHERE Id = $id;";
        command.Parameters.AddWithValue("$pending", reviewPending ? 1 : 0);
        command.Parameters.AddWithValue("$received", reviewReceived ? 1 : 0);
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }
    
    public static void UpdateEvaluationStatus(long studentId, bool evaluationPending, bool evaluationTaken, double average)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Students
            SET EvaluationPending = $pending, EvaluationTaken = $taken, EvaluationAverage = $avg
            WHERE Id = $id;";
        command.Parameters.AddWithValue("$pending", evaluationPending ? 1 : 0);
        command.Parameters.AddWithValue("$taken", evaluationTaken ? 1 : 0);
        command.Parameters.AddWithValue("$avg", average);
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }

    public static void AddLearningHistoryEntry(long studentId, string description)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO LearningHistory (StudentId, Description)
            VALUES ($sid, $desc);";
        command.Parameters.AddWithValue("$sid", studentId);
        command.Parameters.AddWithValue("$desc", description);
        command.ExecuteNonQuery();
    }

    public static void AddEvaluationResult(long studentId, EvaluationResult result)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO EvaluationResults (StudentId, EvaluationDate, Score, BeginPeriod, EndPeriod)
        VALUES ($sid, $date, $score, $begin, $end);";
        command.Parameters.AddWithValue("$sid", studentId);
        command.Parameters.AddWithValue("$date", result.EvaluationDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$score", result.Score);
        command.Parameters.AddWithValue("$begin", result.BeginPeriod.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$end", result.EndPeriod.ToString("yyyy-MM-dd"));
        command.ExecuteNonQuery();
    }

    public static void UpdateBeginPeriod(long studentId, DateOnly date)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE Students 
        SET BeginPeriod = $date 
        WHERE Id = $id;";
        command.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }

    public static void UpdateEndPeriod(long studentId, DateOnly date)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE Students 
        SET EndPeriod = $date 
        WHERE Id = $id;";
        command.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }

    public static List<EvaluationResult> GetEvaluationResults(long studentId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM EvaluationResults WHERE StudentId = $id;";
        command.Parameters.AddWithValue("$id", studentId);

        var results = new List<EvaluationResult>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new EvaluationResult
            {
                EvaluationDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("EvaluationDate"))),
                Score = reader.GetDouble(reader.GetOrdinal("Score")),
                BeginPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("BeginPeriod"))),
                EndPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("EndPeriod")))
            });
        }
        return results;
    }

    public static void ArchiveStudent(long studentId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Students SET IsArchived = 1 WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }

    public static void UnarchiveStudent(long studentId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Students SET IsArchived = 0 WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }

    public static List<Student> GetArchivedStudents()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Students WHERE IsArchived = 1;";

        var students = new List<Student>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var student = new Student();
            student.Id = reader.GetInt64(reader.GetOrdinal("Id"));
            student.Name = reader.GetString(reader.GetOrdinal("Name"));
            student.CurrentGoal = reader.GetString(reader.GetOrdinal("CurrentGoal"));
            student.StartDate = DateOnly.Parse(reader.GetString(reader.GetOrdinal("StartDate")));
            student.BeginPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("BeginPeriod")));
            student.EndPeriod = DateOnly.Parse(reader.GetString(reader.GetOrdinal("EndPeriod")));
            student.CurrentLevel = reader.GetString(reader.GetOrdinal("CurrentLevel"));
            student.LessonCount = reader.GetDouble(reader.GetOrdinal("LessonCount"));
            student.HasHomework = reader.GetInt32(reader.GetOrdinal("HasHomework")) == 1;
            student.HomeworkSent = reader.GetInt32(reader.GetOrdinal("HomeworkSent")) == 1;
            student.ReviewPending = reader.GetInt32(reader.GetOrdinal("ReviewPending")) == 1;
            student.ReviewReceived = reader.GetInt32(reader.GetOrdinal("ReviewReceived")) == 1;
            student.EvaluationPending = reader.GetInt32(reader.GetOrdinal("EvaluationPending")) == 1;
            student.EvaluationTaken = reader.GetInt32(reader.GetOrdinal("EvaluationTaken")) == 1;
            student.EvaluationAverage = reader.GetDouble(reader.GetOrdinal("EvaluationAverage"));
            student.EvaluationResults = new List<EvaluationResult>();
            student.LearningHistory = new List<string>();
            students.Add(student);
        }
        return students;
    }

    public static void UpdateBasicInfo(long studentId, string currentGoal, string currentLevel)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE Students 
        SET CurrentGoal = $goal, CurrentLevel = $level 
        WHERE Id = $id;";
        command.Parameters.AddWithValue("$goal", currentGoal);
        command.Parameters.AddWithValue("$level", currentLevel);
        command.Parameters.AddWithValue("$id", studentId);
        command.ExecuteNonQuery();
    }
}
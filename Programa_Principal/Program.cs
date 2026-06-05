public class Student // clase principal
{
    private string name = "";
    private long id;
    private string currentGoal = "";
    private DateOnly startDate;
    private DateOnly beginPeriod;
    private DateOnly endPeriod;
    private string currentLevel = "";
    private double lessonCount;
    private bool hasHomework;
    private bool homeworkSent;
    private bool reviewPending;
    private bool reviewReceived;
    private bool evaluationPending;
    private bool evaluationTaken;
    private List<EvaluationResult> evaluationResults = new();
    private double evaluationAverage;
    private List<string> learningHistory = new();

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public long Id
    {
        get { return id; }
        set { id = value; }
    }

    public string CurrentGoal
    {
        get { return currentGoal; }
        set { currentGoal = value; }
    }

    public DateOnly StartDate
    {
        get { return startDate; }
        set { startDate = value; }
    }

    public DateOnly BeginPeriod
    {
        get { return beginPeriod; }
        set { beginPeriod = value; }
    }

    public DateOnly EndPeriod
    {
        get { return endPeriod; }
        set { endPeriod = value; }
    }

    public string CurrentLevel
    {
        get { return currentLevel; }
        set { currentLevel = value; }
    }

    public double LessonCount
    {
        get { return lessonCount; }
        set { lessonCount = value; }
    }

    public bool HasHomework
    {
        get { return hasHomework; }
        set {  hasHomework = value; }
    }

    public bool HomeworkSent
    {
        get { return homeworkSent; }
        set {  homeworkSent = value; }
    }
    public bool ReviewPending
    {
        get { return reviewPending; }
        set { reviewPending = value; }

    }

    public bool ReviewReceived
    {
        get { return reviewReceived; }
        set { reviewReceived = value; }
    }

    public bool EvaluationPending
    {
        get { return evaluationPending; }
        set { evaluationPending = value; }
    }

    public bool EvaluationTaken
    {
        get { return evaluationTaken; }
        set { evaluationTaken = value; }
    }

    public List<EvaluationResult> EvaluationResults { 
        get { return evaluationResults; } 
        set { evaluationResults = value; }
    }

    public double EvaluationAverage
    {
        get { return evaluationAverage; }
        set {  evaluationAverage = value; }
    }

    public List<string> LearningHistory 
    { 
        get { return learningHistory; }  
        set { learningHistory = value; } 
    }
}

public class EvaluationResult
{
    private DateOnly evaluationDate;
    private double score;
    private DateOnly beginPeriod;
    private DateOnly endPeriod;

    public DateOnly EvaluationDate
    {
        get { return evaluationDate; }
        set { evaluationDate = value; }
    }

    public double Score
    {
        get { return score; }
        set { score = value; }
    }

    public DateOnly BeginPeriod
    {
        get { return beginPeriod; }
        set { beginPeriod = value; }
    }

    public DateOnly EndPeriod
    {
        get { return endPeriod; }
        set { endPeriod = value; }
    }
}

class Program
{
    static void Main()
    {
        DatabaseHelper.Initialize(); // Added the database here. Tuve que agregar un archivo con una class que es el DatabaseHelper para poder inicializar la base de datos. 

        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== MAIN MENU ===");
            Console.WriteLine("1. Start daily session");
            Console.WriteLine("2. Add student");
            Console.WriteLine("3. View all students");
            Console.WriteLine("4. Archive student");
            Console.WriteLine("5. View archived students");
            Console.WriteLine("6. Update student info");
            Console.WriteLine("0. Exit");
            Console.Write("\nChoice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    DailySessionFlow();
                    break;

                case "2":
                    AddStudentFlow();
                    break;

                case "3":
                    ListStudentsFlow();
                    break;
                case "4": 
                    ArchiveStudentFlow(); 
                    break;
                case "5": 
                    ArchivedStudentsFlow(); 
                    break;
                case "6": 
                    UpdateBasicInfoFlow();
                    break;
                case "0":
                    running = false;
                    break;
            }
        }
    }

    static void DailySessionFlow()
    {
        Console.Clear();
        Console.WriteLine("=== DAILY SESSION ===\n");

        var students = DatabaseHelper.GetAllStudents();
        if (students.Count == 0)
        {
            Console.WriteLine("There are no students registered. Press Enter...");
            Console.ReadLine();
            return;
        }

        // Esta parte muestra los estudiantes
        Console.WriteLine($"{"ID", -10} {"Name", -20} {"Level", -15} {"Lesson Count", -10}");
        Console.WriteLine(new string('-', 55));
        foreach (var st in students)
        {
            Console.WriteLine($"{st.Id, -10} {st.Name, -20} {st.CurrentLevel, -15} {st.LessonCount, -10}");
        }

        long id = ReadLong("\nStudent ID (press 0 to cancel): ");
        if (id == 0)
        {
            return;
        }

        var student = DatabaseHelper.GetStudent(id);
        if (student == null)
        {
            Console.WriteLine("Student not found. Press Enter...");
            Console.ReadLine();
            return;
        }
        StudentSessionMenu(student);
    }

    static void StudentSessionMenu(Student s)
    {
        bool sessionRunning = true;
        while (sessionRunning)
        {
            Console.Clear();
            Console.WriteLine($"=== {s.Name.ToUpper()} ===");
            Console.WriteLine($"Level: {s.CurrentLevel} | Lesson Count: {s.LessonCount} | Average: {s.EvaluationAverage:F2}");
            Console.WriteLine($"Has homework: {(s.HasHomework ? "Yes" : "No")} | Homework sent: {(s.HomeworkSent ? "Yes" : "No")}");
            Console.WriteLine($"Review pending: {(s.ReviewPending ? "Yes" : "No")} | Review received: {(s.ReviewReceived ? "Yes" : "No")}");
            Console.WriteLine($"Evaluation pending: {(s.EvaluationPending ? "Yes" : "No")} | Evaluation taken: {(s.EvaluationTaken ? "Yes" : "No")}");
            Console.WriteLine();
            Console.WriteLine("1. Register lesson (1 or 0.5)");
            Console.WriteLine("2. Update homework status");
            Console.WriteLine("3. Update review status");
            Console.WriteLine("4. Update evaluation status");
            Console.WriteLine("5. Add entry to learning history");
            Console.WriteLine("6. View learning history");
            Console.WriteLine("7. View evaluation results");
            Console.WriteLine("0. Go back");
            Console.Write("\nChoice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    double amount;
                    while (true)
                    {
                        amount = ReadDouble("Full or half lesson? (1 / 0.5): ");
                        if (amount == 1 || amount == 0.5)
                        {
                            break;
                        }
                        Console.WriteLine("Please enter 1 or 0.5.");
                    }
                    DatabaseHelper.IncrementLessonCount(s.Id, amount);
                    s.LessonCount += amount;
                    Console.WriteLine("Lesson registered. Press Enter...");
                    Console.ReadLine();
                    break;
                case "2":
                    Console.Write("Do they have homework? (y/n): ");
                    bool hw = Console.ReadLine()?.ToLower() == "y";
                    Console.Write("Has the homework been sent? (y/n): ");
                    bool hwSent = Console.ReadLine()?.ToLower() == "y";
                    DatabaseHelper.UpdateHomeworkStatus(s.Id, hw, hwSent);
                    s.HasHomework = hw;
                    s.HomeworkSent = hwSent;
                    Console.WriteLine("Updated. Press Enter...");
                    Console.ReadLine();
                    break;
                case "3":
                    Console.Write("Do they have a review pending to receive? (y/n): ");
                    bool revPending = Console.ReadLine()?.ToLower() == "y";
                    Console.Write("Have they received their review? (y/n): ");
                    bool revReceived = Console.ReadLine()?.ToLower() == "y";
                    DatabaseHelper.UpdateReviewStatus(s.Id, revPending, revReceived);
                    if (revPending)
                    {
                        DatabaseHelper.UpdateEndPeriod(s.Id, DateOnly.FromDateTime(DateTime.Now));
                    }
                    s.ReviewPending = revPending;
                    s.ReviewReceived = revReceived;
                    Console.WriteLine("Updated. Press Enter...");
                    Console.ReadLine();
                    break;
                case "4":
                    Console.Write("Evaluation Pending? (y/n): ");
                    bool evalPending = Console.ReadLine()?.ToLower() == "y";
                    Console.Write("Evaluation taken? (y/n): ");
                    bool evalTaken = Console.ReadLine()?.ToLower() == "y";

                    if (evalTaken)
                    {
                        double score;
                        while (true)
                        {
                            score = ReadDouble("Score for this evaluation: ");
                            if (score >= 0 && score <= 100)
                            {
                                break;
                            }
                            Console.WriteLine("Score must be between 0 and 100.");
                        }

                        // Esto guarda el resultado con el periodo actual
                        var result = new EvaluationResult
                        {
                            EvaluationDate = DateOnly.FromDateTime(DateTime.Now),
                            Score = score,
                            BeginPeriod = s.BeginPeriod,
                            EndPeriod = s.EndPeriod
                        };
                        DatabaseHelper.AddEvaluationResult(s.Id, result);

                        // Esto hace el promedio
                        var allResults = DatabaseHelper.GetEvaluationResults(s.Id);
                        double avg = allResults.Sum(r => r.Score) / allResults.Count;

                        // Esto guarda todo
                        DatabaseHelper.UpdateEvaluationStatus(s.Id, evalPending, evalTaken, avg);
                        DatabaseHelper.UpdateBeginPeriod(s.Id, DateOnly.FromDateTime(DateTime.Now));

                        s.EvaluationPending = evalPending;
                        s.EvaluationTaken = evalTaken;
                        s.EvaluationAverage = avg;
                        s.BeginPeriod = DateOnly.FromDateTime(DateTime.Now);

                        Console.WriteLine($"Score registered. New average: {avg:F2}");
                    }
                    else
                    {
                        DatabaseHelper.UpdateEvaluationStatus(s.Id, evalPending, evalTaken, s.EvaluationAverage);
                        s.EvaluationPending = evalPending;
                        s.EvaluationTaken = evalTaken;
                        Console.WriteLine("Updated.");
                    }

                    Console.WriteLine("Press Enter...");
                    Console.ReadLine();
                    break;
                case "5":
                    Console.Write("Description: ");
                    string desc = Console.ReadLine() ?? "";
                    DatabaseHelper.AddLearningHistoryEntry(s.Id, desc);
                    Console.WriteLine("Entry added. Press Enter...");
                    Console.ReadLine();
                    break;
                case "6":
                    Console.Clear();
                    Console.WriteLine($"=== LEARNING HISTORY: {s.Name.ToUpper()} ===\n");
                    var history = DatabaseHelper.GetStudent(s.Id)?.LearningHistory;
                    if (history == null || history.Count == 0)
                        Console.WriteLine("No entries yet.");
                    else
                        foreach (var entry in history)
                            Console.WriteLine($"- {entry}");
                    Console.WriteLine("\nPress Enter...");
                    Console.ReadLine();
                    break;

                case "7":
                    Console.Clear();
                    Console.WriteLine($"=== EVALUATION RESULTS: {s.Name.ToUpper()} ===\n");
                    var results = DatabaseHelper.GetEvaluationResults(s.Id);
                    if (results.Count == 0)
                        Console.WriteLine("No evaluations yet.");
                    else
                    {
                        Console.WriteLine($"{"Date",-15} {"Score",-10} {"Period Start",-15} {"Period End",-15}");
                        Console.WriteLine(new string('-', 55));
                        foreach (var r in results)
                            Console.WriteLine($"{r.EvaluationDate,-15} {r.Score,-10:F2} {r.BeginPeriod,-15} {r.EndPeriod,-15}");
                        Console.WriteLine($"\nCurrent Average: {s.EvaluationAverage:F2}");
                    }
                    Console.WriteLine("\nPress Enter...");
                    Console.ReadLine();
                    break;
                case "0":
                    sessionRunning = false;
                    break;
                default:
                    break;
            }
        }
    }

    static void AddStudentFlow()
    {
        Console.Clear();
        Console.WriteLine("=== ADD STUDENT ===\n");

        var s = new Student();

        Console.Write("Name: ");
        s.Name = Console.ReadLine() ?? "";

        s.Id = ReadLong("ID: ");

        if (DatabaseHelper.GetStudent(s.Id) != null)
        {
            Console.WriteLine("A student with that ID already exists. Press Enter...");
            Console.ReadLine();
            return;
        }

        Console.Write("Current goal: ");
        s.CurrentGoal = Console.ReadLine() ?? "";

        s.StartDate = DateOnly.FromDateTime(DateTime.Now);
        s.BeginPeriod = DateOnly.FromDateTime(DateTime.Now);
        s.EndPeriod = DateOnly.FromDateTime(DateTime.Now);
        s.EvaluationResults = new List<EvaluationResult>();
        s.LearningHistory = new List<string>();

        DatabaseHelper.AddStudent(s);
        DatabaseHelper.UpdateBeginPeriod(s.Id, DateOnly.FromDateTime(DateTime.Now));
        Console.WriteLine("\nStudent added. Press Enter...");
        Console.ReadLine();
    }

    static void ListStudentsFlow()
    {
        Console.Clear();
        Console.WriteLine("=== ALL STUDENTS ===\n");

        var all = DatabaseHelper.GetAllStudents();
        if (all.Count == 0)
        {
            Console.WriteLine("There are no students.");
        }
        else
        {
            Console.WriteLine($"{"ID", -10} {"Name", -20} {"Level", -15} {"Lesson Count", -10}");
            Console.WriteLine(new string('-', 55));
            foreach (var st in all)
            {
                Console.WriteLine($"{st.Id, -10} {st.Name, -20} {st.CurrentLevel, -15} {st.LessonCount, -10}");
            }
        }
        Console.WriteLine("\nPress Enter...");
        Console.ReadLine();
    }

    static void ArchiveStudentFlow()
    {
        Console.Clear();
        Console.WriteLine("=== ARCHIVE STUDENT ===\n");

        var students = DatabaseHelper.GetAllStudents();
        if (students.Count == 0)
        {
            Console.WriteLine("No active students. Press Enter...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine($"{"ID",-10} {"Name",-20} {"Level",-15}");
        Console.WriteLine(new string('-', 45));
        foreach (var st in students)
            Console.WriteLine($"{st.Id,-10} {st.Name,-20} {st.CurrentLevel,-15}");

        long id = ReadLong("\nStudent ID to archive (0 to cancel): ");
        if (id == 0) return;

        var student = DatabaseHelper.GetStudent(id);
        if (student == null)
        {
            Console.WriteLine("Student not found. Press Enter...");
            Console.ReadLine();
            return;
        }

        Console.Write($"Archive {student.Name}? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y") 
            return;

        DatabaseHelper.ArchiveStudent(id);
        Console.WriteLine($"{student.Name} archived. Press Enter...");
        Console.ReadLine();
    }

    static void ArchivedStudentsFlow()
    {
        Console.Clear();
        Console.WriteLine("=== ARCHIVED STUDENTS ===\n");

        var students = DatabaseHelper.GetArchivedStudents();
        if (students.Count == 0)
        {
            Console.WriteLine("No archived students. Press Enter...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine($"{"ID",-10} {"Name",-20} {"Level",-15}");
        Console.WriteLine(new string('-', 45));
        foreach (var st in students)
            Console.WriteLine($"{st.Id,-10} {st.Name,-20} {st.CurrentLevel,-15}");

        long id = ReadLong("\nEnter ID to reactivate (0 to cancel): ");
        if (id == 0) return;

        var student = DatabaseHelper.GetStudent(id);
        if (student == null)
        {
            Console.WriteLine("Student not found. Press Enter...");
            Console.ReadLine();
            return;
        }

        Console.Write($"Reactivate {student.Name}? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y") return;

        DatabaseHelper.UnarchiveStudent(id);
        Console.WriteLine($"{student.Name} reactivated. Press Enter...");
        Console.ReadLine();
    }

    static void UpdateBasicInfoFlow()
    {
        Console.Clear();
        Console.WriteLine("=== UPDATE STUDENT INFO ===\n");

        var students = DatabaseHelper.GetAllStudents();
        if (students.Count == 0)
        {
            Console.WriteLine("No active students. Press Enter...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine($"{"ID",-10} {"Name",-20} {"Goal",-20} {"Level",-15}");
        Console.WriteLine(new string('-', 65));
        foreach (var st in students)
            Console.WriteLine($"{st.Id,-10} {st.Name,-20} {st.CurrentGoal,-20} {st.CurrentLevel,-15}");

        long id = ReadLong("\nStudent ID (0 to cancel): ");
        if (id == 0) return;

        var student = DatabaseHelper.GetStudent(id);
        if (student == null)
        {
            Console.WriteLine("Student not found. Press Enter...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine($"\nCurrent goal: {student.CurrentGoal}");
        Console.Write("New goal (press Enter to keep current): ");
        string goal = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(goal)) goal = student.CurrentGoal;

        Console.WriteLine($"\nCurrent level: {student.CurrentLevel}");
        Console.Write("New level (press Enter to keep current): ");
        string level = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(level)) level = student.CurrentLevel;

        DatabaseHelper.UpdateBasicInfo(id, goal, level);
        Console.WriteLine("\nUpdated. Press Enter...");
        Console.ReadLine();
    }

    static long ReadLong(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (long.TryParse(Console.ReadLine(), out long value))
                return value;
            Console.WriteLine("Invalid number. Try again.");
        }
    }

    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out double value))
                return value;
            Console.WriteLine("Invalid number. Try again.");
        }
    }
}

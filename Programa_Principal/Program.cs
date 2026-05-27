
using System.Runtime.CompilerServices;

public class Student // clase principal
{
    private string name;
    private long id;
    private string currentGoal;
    private DateOnly startDate;
    private DateOnly beginPeriod;
    private DateOnly endPeriod;
    private string currentLevel;
    private double lessonCount;
    private bool hasHomework;
    private bool homeworkSent;
    private bool reviewPending;
    private bool reviewReceived;
    private bool evaluationPending;
    private bool evaluationTaken;
    private Dictionary<DateOnly, double> evaluationResults;
    private double evaluationAverage;
    private Dictionary<long, string> learningHistory;

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

    public Dictionary<DateOnly, double> EvaluationResults { 
        get { return evaluationResults; } 
        set { evaluationResults = value; }
    }

    public double EvaluationAverage
    {
        get { return evaluationAverage; }
        set {  evaluationAverage = value; }
    }

    public Dictionary<long, string> LearningHistory 
    { 
        get { return learningHistory; }  
        set { learningHistory = value; } 
    }

    // TODO: Add relevant functions
}

class Program
{
    static void Main()
    {
        DatabaseHelper.Initialize(); // Added the database here. Tuve que agregar un archivo tipo class que es el dbhelper para poder inicializar la base de datos. 

        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== MENU ===");
            Console.WriteLine("1. Add student");
            Console.WriteLine("2. Look up student by ID");
            Console.WriteLine("3. List all students");
            Console.WriteLine("0. Salir");
            Console.Write("\nOpción: ");

            switch (Console.ReadLine())
            {
                case "1":
                    var s = new Student();

                    Console.Write("Name: ");
                    s.Name = Console.ReadLine() ?? "";

                    Console.Write("ID: ");
                    s.Id = long.Parse(Console.ReadLine() ?? "0");

                    Console.Write("Current Objective: ");
                    s.CurrentGoal = Console.ReadLine() ?? "";

                    Console.Write("Current level: ");
                    s.CurrentLevel = Console.ReadLine() ?? "";

                    s.StartDate = DateOnly.FromDateTime(DateTime.Now);
                    s.BeginPeriod = DateOnly.FromDateTime(DateTime.Now);
                    s.EndPeriod = DateOnly.FromDateTime(DateTime.Now);
                    s.EvaluationResults = new Dictionary<DateOnly, double>();
                    s.LearningHistory = new Dictionary<long, string>();

                    DatabaseHelper.AddStudent(s);
                    Console.WriteLine("Student saved! Press Enter...");
                    Console.ReadLine();
                    break;

                case "2":
                    Console.Write("ID: ");
                    long id = long.Parse((Console.ReadLine() ?? "0"));

                    var found = DatabaseHelper.GetStudent(id);
                    if (found == null)
                    {
                        Console.WriteLine("Not found.");
                    } else
                    {
                        Console.WriteLine($"\nName: {found.Name}");
                        Console.WriteLine($"\nID: {found.Id}");
                        Console.WriteLine($"\nGoal: {found.CurrentGoal}");
                        Console.WriteLine($"\nLevel: {found.CurrentLevel}");
                        Console.WriteLine($"\nStart date: {found.StartDate}");
                    }
                    Console.WriteLine("\nPress Enter...");
                    Console.ReadLine();
                    break;
                case "3":
                    var all = DatabaseHelper.GetAllStudents();
                    if (all.Count == 0)
                    {
                        Console.WriteLine("There are no students");
                    }
                    else
                    {
                        Console.WriteLine($"\n{"ID", -10} {"Name", -20} {"Level", -15} {"Lessons", -10}");
                        Console.WriteLine(new string('-', 55));
                        foreach (var st in all)
                        {
                            Console.WriteLine($"{st.Id, -10} {st.Name, -20} {st.CurrentLevel, -15} {st.LessonCount, -10}");
                        }
                    }
                    Console.WriteLine("\nPress Enter...");
                    Console.ReadLine();
                    break;

                case "0":
                    running = false;
                    break;
            }
        }
    }
}

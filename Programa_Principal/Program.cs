
using System.Runtime.CompilerServices;

class Student // clase principal
{
    private string name;
    private long id;
    private string currentGoal;
    private DateOnly startDate;
    private DateOnly beginPeriod;
    private DateOnly endPeriod;
    private string currentLevel;
    private double lessonCount;
    private bool reviewPending;
    private bool reviewReceived;
    private bool evaluationPending;
    private bool evaluationTaken;
    private Dictionary<long, double> evaluationResults;
    private double evaluationAverage;

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

    public Dictionary<long, double> EvaluationResults { 
        get { return evaluationResults; } 
        set { evaluationResults = value; }
    }

    public double EvaluationAverage
    {
        get { return EvaluationAverage; }
        set {  EvaluationAverage = value; }
    }

    // TODO: Add relevant functions
}

class Program
{
    static void Main()
    {

    }
}

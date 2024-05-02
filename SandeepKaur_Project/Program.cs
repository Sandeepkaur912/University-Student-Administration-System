using System;                         // Root Namespace
using System.Collections.Generic;     // Namespace for Interface and Classes
using System.Xml.Linq; // Namespace to provide class to create Langugae Integrated Query Syntax
using System.IO;       // Namespace to support Input/Output operations
using System.Linq;     // Namespace that provides classes and interface that support LINQ in C#
using System.Text.RegularExpressions; // Namespace to provide Regular Expression sipport in C#

class Assessment
{
    public int MaxMark { get; }
    public int Mark { get; set; }

    public Assessment(int maxMark)
    {
        MaxMark = maxMark;
        Mark = -1; // -1 represents an undefined mark here
    }
}

class Module
{
    public string Code { get; }
    public string Title { get; }
    public List<Assessment> Assessments { get; }

    public Module(string code, string title)
    {
        Code = code;
        Title = title;
        Assessments = new List<Assessment>();
    }

    public void AddAssessment(int maxMark)
    {
        Assessments.Add(new Assessment(maxMark));
    }

    public string CalculateModuleResult()
    {
        if (Assessments.Count == 0)
            return "Undefined";

        int totalMarks = 0;
        int definedAssessments = 0;

        foreach (var assessment in Assessments)
        {
            if (assessment.Mark != -1)
            {
                totalMarks += assessment.Mark;
                definedAssessments++;
            }
        }

        int averageMark = totalMarks / definedAssessments;

        if (averageMark >= 50)
            return "Pass";
        if (averageMark >= 45)
            return "PassCompensation";

        return "Fail";
    }
}

class DegreeProgram
{
    public string ProgramId { get; }
    public string ProgramTitle { get; }
    public int Duration { get; }
    public List<Module> Modules { get; }

    public DegreeProgram(string programId, string programTitle, int duration)
    {
        ProgramId = programId;
        ProgramTitle = programTitle;
        Duration = duration;
        Modules = new List<Module>();
    }

    public void AddModule(string code, string title)
    {
        if (Modules.Exists(module => module.Code == code))
        {
            Console.WriteLine("A module with the same code already exists in this program.");
        }
        else
        {
            Modules.Add(new Module(code, title));
        }
    }

    public void AddModule1()
    {
        Modules.Insert(0, new Module("20221", "Social and Professional Ethics"));
    }

    public string CalculateProgramResult()
    {
        if (Modules.Count == 0)
            return "Undefined";

        int totalMarks = 0;
        int definedModules = 0;

        foreach (var module in Modules)
        {
            string moduleResult = module.CalculateModuleResult();

            if (moduleResult == "Undefined")
                return "Undefined";

            if (moduleResult == "Pass" || moduleResult == "PassCompensation")
            {
                totalMarks += 100;
                definedModules++;
            }
        }

        if (definedModules == 0)
            return "Undefined";

        int averageModuleResult = (totalMarks * 100) / (definedModules * Modules.Count);
        if (averageModuleResult >= 70)
            return "Distinction";
        if (averageModuleResult >= 50)
            return "Pass";
        return "Fail";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("1. Enter a new student record.");
        Console.WriteLine("2. Read the existing saved students records.");
        Console.WriteLine("3. Show all saved records.");
        Console.Write("Select an option (1, 2, or 3): ");

        string choice = Console.ReadLine();

        if (choice == "1")
        {
            EnterNewStudentData();
        }
        else if (choice == "2")
        {
            Console.WriteLine("1. Read by student's name");
            Console.WriteLine("2. Read by program's name");
            Console.Write("Select an option (1 or 2): ");
            string readChoice = Console.ReadLine();

            if (readChoice == "1")
            {
                ReadByStudentName();
            }
            else if (readChoice == "2")
            {
                ReadByProgramName();
            }
            else
            {
                Console.WriteLine("Invalid choice. Please select option 1 or 2.");
            }
        }
        else if (choice == "3")
        {
            ShowAllRecords("student_results.xml");
        }
        else
        {
            Console.WriteLine("Invalid choice. Please select option 1, 2, or 3.");
        }
    }

    static void EnterNewStudentData()
    {
        do
        {
            Console.Write("Enter the student's name: ");
            string studentName = Console.ReadLine();

            string studentId;
            do
            {
                Console.Write("Enter the student's ID (Cohort Year followed by 6 unique digits, e.g., 2023123456): ");
                studentId = Console.ReadLine();

                if (!ValidateStudentId(studentId))
                {
                    Console.WriteLine("Invalid student ID. Please enter it as Cohort Year followed by 6 unique digits.");
                }
            } while (!ValidateStudentId(studentId));

            string programId;
            do
            {
                Console.Write("Enter the degree program ID (6 unique digits): ");
                programId = Console.ReadLine();

                if (!ValidateProgramId(programId))
                {
                    Console.WriteLine("Invalid program ID. Please enter 6 unique digits string.");
                }
            } while (!ValidateProgramId(programId));

            Console.Write("Enter the degree program title: ");
            string programTitle = Console.ReadLine();

            Console.Write("Enter the program duration (1 for one year, 2 for two years): ");
            int programDuration = int.Parse(Console.ReadLine());

            DegreeProgram degreeProgram = new DegreeProgram(programId, programTitle, programDuration);

            // Adding Module 1 (Social and Professional Ethics)
            degreeProgram.AddModule1();

            // To display Module 1 information
            Module module1 = degreeProgram.Modules.First(); // Get the first module (Module 1)
            Console.WriteLine($"Module 1 - Code: {module1.Code}, Title: {module1.Title}");

            // below, I'm adding assessments to Module 1
            while (true)
            {
                Console.Write("Enter the maximum mark for Module 1 assessment (or type 'exit' to finish adding assessments): ");
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    break;
                }

                if (int.TryParse(input, out int maxMark))
                {
                    module1.AddAssessment(maxMark);

                    Console.Write("Enter the mark for the assessment: ");
                    int mark = int.Parse(Console.ReadLine());

                    module1.Assessments[module1.Assessments.Count - 1].Mark = mark;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a numeric maximum mark or 'exit'.");
                }
            }

            // The below is a loop to add remaining modules based on the program duration (6 for 1 year, 12 for 2 Years)
            int totalModules = programDuration * 6;
            for (int i = 2; i <= totalModules; i++)
            {
                string moduleCode;
                do
                {
                    Console.Write($"Enter the module {i} code (5 digits): ");
                    moduleCode = Console.ReadLine();

                    if (!ValidateModuleCode(moduleCode))
                    {
                        Console.WriteLine("Invalid module code. Please enter 5 unique digits string.");
                    }
                } while (!ValidateModuleCode(moduleCode));

                Console.Write($"Enter the module {i} title: ");
                string moduleTitle = Console.ReadLine();

                degreeProgram.AddModule(moduleCode, moduleTitle);

                // To display the module information
                Module currentModule = degreeProgram.Modules[degreeProgram.Modules.Count - 1];
                Console.WriteLine($"Module {currentModule.Code} - Code: {currentModule.Code}, Title: {currentModule.Title}");

                // To add assessments to the modules
                while (true)
                {
                    Console.Write("Enter the maximum mark for the assessment (or type 'exit' to finish adding assessments): ");
                    string input = Console.ReadLine();

                    if (input.ToLower() == "exit")
                    {
                        break;
                    }

                    if (int.TryParse(input, out int maxMark))
                    {
                        currentModule.AddAssessment(maxMark);

                        Console.Write("Enter the mark for the assessment: ");
                        int mark = int.Parse(Console.ReadLine());

                        currentModule.Assessments[currentModule.Assessments.Count - 1].Mark = mark;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a numeric maximum mark or 'exit'.");
                    }
                }
            }

            Console.WriteLine("Module Results:");

            foreach (var module in degreeProgram.Modules)
            {
                string moduleResult = module.CalculateModuleResult();
                Console.WriteLine($"Module {module.Code} - {module.Title}: {moduleResult}");
            }

            // To calculate and show overall program result
            string programResult = degreeProgram.CalculateProgramResult();
            Console.WriteLine($"Program Result: {programResult}");

            // To save data to an XML file
            SaveDataToXml(degreeProgram, studentName, studentId, "student_results.xml");

            Console.Write("Do you want to enter more students? (Yes/No): ");
        } while (Console.ReadLine().Trim().ToLower() == "yes");
    }

    static bool ValidateStudentId(string studentId)
    {
        // To Validate student ID: CohortYear (4 digits) followed by 6 unique digits.
        return Regex.IsMatch(studentId, @"^\d{4}\d{6}$");
    }

    static bool ValidateProgramId(string programId)
    {
        // To Validate if the program ID is 6 unique digits.
        return Regex.IsMatch(programId, @"^\d{6}$");
    }
    static bool ValidateModuleCode(string moduleCode)
    {
        // To Validate module code: 5 digits.
        return Regex.IsMatch(moduleCode, @"^\d{5}$");
    }
    static void SaveDataToXml(DegreeProgram degreeProgram, string studentName, string studentId, string fileName)
    {
        XDocument xmlDoc;

        if (File.Exists(fileName))
        {
            xmlDoc = XDocument.Load(fileName);
        }
        else
        {
            xmlDoc = new XDocument(new XElement("Students"));
        }

        XElement student = new XElement("Student",
            new XElement("Name", studentName),
            new XElement("ID", studentId),
            new XElement("Program",
                new XElement("ID", degreeProgram.ProgramId),
                new XElement("Title", degreeProgram.ProgramTitle),
                new XElement("Duration", degreeProgram.Duration),
                new XElement("Modules", from module in degreeProgram.Modules
                                        select new XElement("Module",
                                            new XElement("Code", module.Code),
                                            new XElement("Title", module.Title),
                                            new XElement("Result", module.CalculateModuleResult()),
                                            new XElement("Assessments", from assessment in module.Assessments
                                                                        select new XElement("Assessment",
                                                                            new XElement("MaxMark", assessment.MaxMark),
                                                                            new XElement("Mark", assessment.Mark)
                                                     )
                                     )
                                 )
                       )
                    )
                );

        xmlDoc.Root?.Add(student);
        xmlDoc.Save(fileName);
        Console.WriteLine($"Data saved to {fileName}");
    }

    static void ReadByStudentName()
    {
        Console.Write("Enter the student's name: ");
        string studentName = Console.ReadLine();

        XDocument xmlDoc = XDocument.Load("student_results.xml");
        var studentElements = xmlDoc.Descendants("Student")
                                    .Where(s => s.Element("Name").Value == studentName);

        foreach (var studentElement in studentElements)
        {
            DisplayStudentRecord(studentElement);
        }
    }

    static void ReadByProgramName()
    {
        Console.Write("Enter the Program Title: ");
        string programTitle = Console.ReadLine();

        XDocument xmlDoc = XDocument.Load("student_results.xml");
        var programElements = xmlDoc.Descendants("Student")
                                    .Where(s => s.Element("Program").Element("Title").Value == programTitle);

        foreach (var programElement in programElements)
        {
            DisplayStudentRecord(programElement);
        }
    }

    static void ShowAllRecords(string xmlFilePath)
    {
        XDocument xmlDoc = XDocument.Load(xmlFilePath);
        var studentElements = xmlDoc.Descendants("Student");

        foreach (var studentElement in studentElements)
        {
            string studentName = studentElement.Element("Name").Value;
            string studentId = studentElement.Element("ID").Value;
            string programId = studentElement.Element("Program").Element("ID").Value;
            string programTitle = studentElement.Element("Program").Element("Title").Value;
            int programDuration = int.Parse(studentElement.Element("Program").Element("Duration").Value);

            Console.WriteLine($"Student Name: {studentName}");
            Console.WriteLine($"Student ID: {studentId}");
            Console.WriteLine($"Program ID: {programId}");
            Console.WriteLine($"Program Title: {programTitle}");
            Console.WriteLine($"Program Duration: {programDuration} years");

            var moduleElements = studentElement.Element("Program").Element("Modules").Elements("Module");
            foreach (var moduleElement in moduleElements)
            {
                string moduleCode = moduleElement.Element("Code").Value;
                string moduleTitle = moduleElement.Element("Title").Value;
                string moduleResult = moduleElement.Element("Result").Value;

                Console.WriteLine($"Module Code: {moduleCode}");
                Console.WriteLine($"Module Title: {moduleTitle}");
                Console.WriteLine($"Module Result: {moduleResult}");

                var assessmentElements = moduleElement.Element("Assessments").Elements("Assessment");
                foreach (var assessmentElement in assessmentElements)
                {
                    int maxMark = int.Parse(assessmentElement.Element("MaxMark").Value);
                    int mark = int.Parse(assessmentElement.Element("Mark").Value);

                    Console.WriteLine($"Assessment MaxMark: {maxMark}");
                    Console.WriteLine($"Assessment Mark: {mark}");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }

    static void DisplayStudentRecord(XElement studentElement)
    {
        string studentName = studentElement.Element("Name").Value;
        string studentId = studentElement.Element("ID").Value;

        Console.WriteLine($"Student Name: {studentName}");
        Console.WriteLine($"Student ID: {studentId}");

        Console.WriteLine("Program Information:");

        string programId = studentElement.Descendants("Program").ElementAt(0).Element("ID").Value;
        string programTitle = studentElement.Descendants("Program").ElementAt(0).Element("Title").Value;
        int programDuration = int.Parse(studentElement.Descendants("Program").ElementAt(0).Element("Duration").Value);

        Console.WriteLine($"Program ID: {programId}");
        Console.WriteLine($"Program Title: {programTitle}");
        Console.WriteLine($"Program Duration: {programDuration} years");

        var moduleElements = studentElement.Descendants("Module");
        foreach (var moduleElement in moduleElements)
        {
            string moduleCode = moduleElement.Element("Code").Value;
            string moduleTitle = moduleElement.Element("Title").Value;
            string moduleResult = moduleElement.Element("Result").Value;

            Console.WriteLine($"Module Code: {moduleCode}");
            Console.WriteLine($"Module Title: {moduleTitle}");
            Console.WriteLine($"Module Result: {moduleResult}");

            var assessmentElements = moduleElement.Descendants("Assessment");
            foreach (var assessmentElement in assessmentElements)
            {
                int maxMark = int.Parse(assessmentElement.Element("MaxMark").Value);
                int mark = int.Parse(assessmentElement.Element("Mark").Value);

                Console.WriteLine($"Assessment MaxMark: {maxMark}");
                Console.WriteLine($"Assessment Mark: {mark}");
            }

            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
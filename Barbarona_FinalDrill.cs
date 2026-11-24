using System;
using System.Collections.Generic;
using System.Linq;

namespace AseanPhonebook
{
    class Entry
    {
        public string StudentNumber = "";
        public string Surname = "";
        public string FirstName = "";
        public string Occupation = "";
        public char Gender = 'M';
        public int CountryCode;
        public string AreaCode = "";
        public string PhoneNumber = "";

        public string Pronoun => Gender == 'M' ? "His" : "Her";
        public string FullPhone => $"{CountryCode}-{AreaCode}-{PhoneNumber}";
    }

    class Program
    {
        // Country code -> country name (limited to five ASEAN countries)
        static readonly Dictionary<int, string> Countries = new Dictionary<int, string>
        {
            { 60, "Malaysia" },
            { 62, "Indonesia" },
            { 63, "Philippines" },
            { 65, "Singapore" },
            { 66, "Thailand" }
        };

        // Search menu choice -> country code
        static readonly Dictionary<int, int> SearchChoices = new Dictionary<int, int>
        {
            { 1, 63 }, 
            { 2, 66 }, 
            { 3, 65 }, 
            { 4, 62 }, 
            { 5, 60 }  
        };

        // No fixed limit on the number of entries
        static readonly List<Entry> Phonebook = new List<Entry>();

        static void Main()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("[1] Store to ASEAN phonebook");
                Console.WriteLine("[2] Edit entry in ASEAN phonebook");
                Console.WriteLine("[3] Search ASEAN phonebook by country");
                Console.WriteLine("[4] Exit");
                Console.Write("Enter choice: ");

                string choice = (Console.ReadLine() ?? "").Trim();
                switch (choice)
                {
                    case "1": StoreEntries(); break;
                    case "2": EditEntry(); break;
                    case "3": SearchByCountry(); break;
                    case "4": return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // ---------- Input helpers ----------

        static string ReadNonEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim();
                if (s.Length > 0) return s;
                Console.WriteLine("Input cannot be empty.");
            }
        }

        static string ReadDigits(string prompt)
        {
            while (true)
            {
                string s = ReadNonEmpty(prompt);
                if (s.All(char.IsDigit)) return s;
                Console.WriteLine("Please enter digits only.");
            }
        }

        static char ReadGender(string prompt)
        {
            while (true)
            {
                string s = ReadNonEmpty(prompt).ToUpper();
                if (s == "M" || s == "F") return s[0];
                Console.WriteLine("Please enter M for male or F for female.");
            }
        }

        static int ReadCountryCode(string prompt)
        {
            while (true)
            {
                string s = ReadDigits(prompt);
                if (int.TryParse(s, out int code) && Countries.ContainsKey(code))
                    return code;
                Console.WriteLine("Invalid country code. Valid codes: " +
                    string.Join(", ", Countries.Select(c => $"{c.Key} ({c.Value})")));
            }
        }

        static string ReadUniqueStudentNumber(string prompt, Entry? ignore = null)
        {
            while (true)
            {
                string s = ReadNonEmpty(prompt);
                bool taken = Phonebook.Any(e => e != ignore &&
                    e.StudentNumber.Equals(s, StringComparison.OrdinalIgnoreCase));
                if (!taken) return s;
                Console.WriteLine("That student number already exists.");
            }
        }

        static bool AskYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim().ToUpper();
                if (s == "Y") return true;
                if (s == "N") return false;
                Console.WriteLine("Please enter Y or N.");
            }
        }

        static Entry? FindByStudentNumber(string number) =>
            Phonebook.FirstOrDefault(e =>
                e.StudentNumber.Equals(number, StringComparison.OrdinalIgnoreCase));

        // ---------- [1] Store ----------

        static void StoreEntries()
        {
            do
            {
                var e = new Entry();
                Console.WriteLine();
                e.StudentNumber = ReadUniqueStudentNumber("Enter student number: ");
                e.Surname = ReadNonEmpty("Enter surname: ");
                e.FirstName = ReadNonEmpty("Enter first name: ");
                e.Occupation = ReadNonEmpty("Enter occupation: ");
                e.Gender = ReadGender("Enter gender (M for male, F for female): ");
                e.CountryCode = ReadCountryCode("Enter country code: ");
                e.AreaCode = ReadDigits("Enter area code: ");
                e.PhoneNumber = ReadDigits("Enter number: ");
                Phonebook.Add(e);
                Console.WriteLine("Entry stored.");
            }
            while (AskYesNo("Do you want to enter another entry [Y/N]? "));
        }

        // ---------- [2] Edit ----------

        static void EditEntry()
        {
            Console.WriteLine();
            Console.Write("Enter student number: ");
            string number = (Console.ReadLine() ?? "").Trim();
            Entry? e = FindByStudentNumber(number);
            if (e == null)
            {
                Console.WriteLine("Error: student number does not exist.");
                return;
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"Here is the existing information about {e.StudentNumber}:");
                Console.WriteLine($"{e.FirstName} {e.Surname} is a {e.Occupation}. " +
                                  $"{e.Pronoun} number is {e.FullPhone}");
                Console.WriteLine("Which of the following information do you wish to change?");
                Console.WriteLine("[1] Student number [2] Surname [3] Gender [4] Occupation");
                Console.WriteLine("[5] Country code [6] Area code [7] Phone number");
                Console.WriteLine("[8] None - Go back to main menu");
                Console.Write("Enter choice: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": e.StudentNumber = ReadUniqueStudentNumber("Enter new student number: ", e); break;
                    case "2": e.Surname = ReadNonEmpty("Enter new surname: "); break;
                    case "3": e.Gender = ReadGender("Enter new gender (M for male, F for female): "); break;
                    case "4": e.Occupation = ReadNonEmpty("Enter new occupation: "); break;
                    case "5": e.CountryCode = ReadCountryCode("Enter new country code: "); break;
                    case "6": e.AreaCode = ReadDigits("Enter new area code: "); break;
                    case "7": e.PhoneNumber = ReadDigits("Enter new phone number: "); break;
                    case "8": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        // ---------- [3] Search ----------

        static void SearchByCountry()
        {
            Console.WriteLine();
            Console.WriteLine("From which country:");
            Console.WriteLine("[1] Philippines [2] Thailand [3] Singapore [4] Indonesia");
            Console.WriteLine("[5] Malaysia [6] ALL [0] No More");

            var selected = new List<int>(); // country codes
            bool all = false;
            int n = 1;

            while (true)
            {
                Console.Write($"Enter choice {n}: ");
                string s = (Console.ReadLine() ?? "").Trim();

                if (s == "0") break;
                if (s == "6") { all = true; break; }

                if (int.TryParse(s, out int c) && SearchChoices.TryGetValue(c, out int code))
                {
                    if (selected.Contains(code))
                        Console.WriteLine("Country already chosen.");
                    else
                    {
                        selected.Add(code);
                        n++;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }

            if (!all && selected.Count == 0)
            {
                Console.WriteLine("No countries selected.");
                return;
            }

            IEnumerable<Entry> results = all
                ? Phonebook
                : Phonebook.Where(e => selected.Contains(e.CountryCode));

            var sorted = results
                .OrderBy(e => e.Surname, StringComparer.OrdinalIgnoreCase)
                .ThenBy(e => e.FirstName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            string names = all
                ? "all countries"
                : JoinNames(selected.Select(c => Countries[c]).ToList());

            Console.WriteLine();
            Console.WriteLine($"Here are the students from {names}:");

            if (sorted.Count == 0)
            {
                Console.WriteLine("(no entries found)");
                return;
            }

            foreach (var e in sorted)
            {
                Console.WriteLine($"{e.Surname}, {e.FirstName}, with student number {e.StudentNumber}, " +
                                  $"is a {e.Occupation}. {e.Pronoun} phone number is {e.FullPhone}");
            }
        }

        static string JoinNames(List<string> names)
        {
            if (names.Count == 1) return names[0];
            if (names.Count == 2) return $"{names[0]} and {names[1]}";
            return string.Join(", ", names.Take(names.Count - 1)) + " and " + names[names.Count - 1];
        }
    }
}
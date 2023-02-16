using PasswordTools.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordTools
{
    internal class LogicPasswordCracker
    {
        Configuration configuration = new Configuration();
        public string path;
        public void Run()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("This cracker isn't actually intended or able to crack a password on a website or app!\nYou can only enter a password and see, if it is possible to crack it, with information about the password owning person or a database of commonly used passwords.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "LogicPasswordCracker";


            configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(@".\config.txt"));
            path = configuration.CrackGenerationsTemporary;

            /*
            HandleDatabase hd = new HandleDatabase();
            hd.DeleteTable(hd.connection);
            hd.CreateTable(hd.connection);
            */


            input();
        }

        void input()
        {
            Console.Write("What do you want to do? (? for help)\n>");
            switch (Console.ReadLine())
            {
                case string str when new List<string>() { "exit", "e" }.Contains(str):
                    {
                        return;
                    }
                case string str when new List<string>() { "help", "?" }.Contains(str):
                    {
                        Console.WriteLine("___Help___\n\n" +
                            " exit\tgo back to the main menue\n" +
                            " clear\tclear the console\n" +
                            " add\tadds a new profile\n" +
                            " what\tinfo how this logic pswd cracker operates\n" +
                            " test\ttest if a password can be cracked with the given information\n" +
                            " d\tadd, list and remove delimiters\n" +
                            " alias\tshows you all aliases for the commands\n");
                        break;
                    }
                case string str when new List<string>() { "clear", "c" }.Contains(str):
                    {
                        Console.Clear();
                        break;
                    }
                case "what":
                    {
                        Console.WriteLine("___What___\n\n" +
                            "This logic password cracker has two options:\n\n" +
                            " 1.\t This method checks if the given password can be found in a database of the 13 million most commonly used passwords.\n" +
                            " 2.\t This method uses information about the password owner to create possible passwords. For example <forname>_<surname> or <surname>.<bith year>\n");
                        break;
                    }
                case "add":
                    {
                        add();
                        break;
                    }
                case "delete":
                    {
                        delete();
                        break;
                    }
                case string str when new List<string>() { "list", "l" }.Contains(str):
                    {
                        HandleDatabase handle = new HandleDatabase();
                        Console.WriteLine("\n" + handle.GetPersonList(handle.connection));
                        break;
                    }
                case "get":
                    {
                        get();
                        break;
                    }

                case "test":
                    {

                        CrackGenerator crack = new CrackGenerator();

                        HandleDatabase handle = new HandleDatabase();
                        Console.WriteLine("\n" + handle.GetPersonList(handle.connection));

                        Console.Write("Which profile should be used? Input the id\n>");
                        HandleDatabase handleDatabase = new HandleDatabase();
                        int id = 0;
                        try
                        {
                            id = int.Parse(Console.ReadLine());
                        }
                        catch
                        {
                            Console.WriteLine("Only input numbers!");
                            return;
                        }
                        Person getPerson = handleDatabase.GetPerson(handleDatabase.connection, id);
                        if (getPerson == null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("\nThere is no profile with the id " + id + "! Enter list to see all profiles and theire IDs\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }


                        Console.Write("How manny maxArguments should the password have at maximum?\n>");
                        string input = Console.ReadLine();
                        if (!int.TryParse(input, out int max))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Please only input numbers!");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        }
                        int maxArguments = int.Parse(input);
                        Console.WriteLine("There are {0} possible combinations!", crack.combinationCalculator(new Person(), maxArguments, crack.delimiters.Count()).ToString("#,##0"));

                        Console.Write("Please input the password you want to test!\n>");
                        string password = Console.ReadLine();
                        List<string> passwords = crack.generation(new Person(), maxArguments, crack.delimiters, path);
                        if (passwords.Contains(password))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Your password can be cracked if someone knows some facts about you!\n" +
                                "You should change your password to something random, especially if these facts can be found on your social media accounts.\n" +
                                "You can use this application to generate a secure password!");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Congratulations! Your  password cannot  be cracked with the given information!\n" +
                                "But if it has personal information, you should change it anyway because a more advanced program might be able to crack it!\n" +
                                "So if you don't already have a randomly generated password, you can create one with the password generator of this application.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    break;
                case string str when str.StartsWith("delimiter") || str.StartsWith("d -"):
                    {
                        if (str.Contains("-"))
                        {
                            char c = str.Split('-')[1].First();
                            delimiter(c);
                            break;
                        }
                        Console.Write("What do you want to do? Input 'a' for add a delimiter 'r' to remove on or 'l' to get a list of all delimiters!\n>");
                        delimiter(Console.ReadLine().First());
                        break;
                    }
                case string str when new List<string>() { "alias", "a" }.Contains(str):
                    {
                        Console.WriteLine("___Aliases___\n\n" +
                            " exit:\te\n" +
                            " help:\t?\n" +
                            " clear:\tc\n" +
                            " list:\tl\n" +
                            " d:\tdelimiter\n" +
                            " alias:\ta\n");
                        break;
                    }
            }


            input();
        }

        //profile management
        void add()
        {
            Person person = new Person();
            HandleDatabase handleDatabase = new HandleDatabase();

            Console.WriteLine("PLease input as many information as possible! \n");

            Console.Write("Persons firstname:\n>");
            person.FirstName = Console.ReadLine();

            Console.Write("Persons lastname:\n>");
            person.LastName = Console.ReadLine();

            Console.Write("Persons birthday(dd.mm.yyyy):\n>");
            person.Birthday = Console.ReadLine();

            Console.Write("Birthdays of friends or family (dd.mm.yyyy;dd.mm.yyyy...):\n>");
            person.OtherBirthdays = Console.ReadLine().Split(';').ToList();

            Console.Write("Persons nicknames:\n>");
            person.Nickname = Console.ReadLine();

            Console.Write("Persons home city:\n>");
            person.City = Console.ReadLine();

            Console.Write("Persons home city aliases (<alias>;<alias>;...):\n>");
            person.CityAliases = Console.ReadLine().Split(';').ToList();

            Console.Write("Persons home Country:\n>");
            person.Country = Console.ReadLine();

            Console.Write("Persons pets name:\n>");
            person.PetsName = Console.ReadLine();

            Console.Write("Persons pets birthday(dd.mm.yyyy):\n>");
            person.PetsBirtday = Console.ReadLine();

            Console.Write("Persons pet type (dog/cat/etc.):\n>");
            person.PetType = Console.ReadLine();

            Console.Write("Persons pet breed (golden retriever/American Bobtail Cat/etc.):\n>");
            person.PetBreed = Console.ReadLine();


            handleDatabase.AddPerson(handleDatabase.connection, person);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Succesfully added new profile!");
            Console.ForegroundColor = ConsoleColor.White;
        }
        void delete()
        {
            HandleDatabase handleDatabase = new HandleDatabase();
            Console.Write("Type in the name of the person who you want to delete\n>");
            string name = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            List<Person> persons = handleDatabase.GetPerson(handleDatabase.connection, name);
            StringBuilder sb = new StringBuilder();
            foreach (Person person in persons)
            {
                sb.AppendLine(person.shortInfos());
            }
            Console.WriteLine("\n" + sb.ToString() + "\n");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Which profile of " + name + " do you want to delete? Type in the id (if you want to delete all type in *)\n>");
            string id = Console.ReadLine();
            if (!int.TryParse(id, out int ID))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Only input numbers!");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            ID = int.Parse(id);

            if (id == "*")
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Do you really wish to delete all profiles of " + name + " ? This is not undoable! If you want to continue, type in  " + id + "\n>");
                if (Console.ReadLine() != id)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Canceled");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                handleDatabase.DeletePerson(handleDatabase.connection, name);
                Console.WriteLine("Done!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Do you really wish to delete this profile:\n\n" + handleDatabase.GetPerson(handleDatabase.connection, ID).formatInfos() + "\nThis is not undoable! If you want to continue, type in  " + id + "\n>");
                if (Console.ReadLine() != id)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Canceled");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                handleDatabase.DeletePerson(handleDatabase.connection, ID);
                Console.WriteLine("Done!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        void get()
        {
            Console.Write("Enter the ID of the profile you want to see all information about\n>");
            HandleDatabase handleDatabase = new HandleDatabase();
            int id = 0;
            try
            {
                id = int.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Only input numbers!");
                return;
            }


            Person getPerson = handleDatabase.GetPerson(handleDatabase.connection, id);
            if (getPerson == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nThere is no profile with the id " + id + "! Enter list to see all profiles and theire IDs\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n" + getPerson.formatInfos() + "\n");
            Console.ForegroundColor = ConsoleColor.White;


        }

        void delimiter(char i)
        {
            switch (i)
            {
                case 'a':
                    {
                        cAdd();
                        break;
                    }
                case 'r':
                    {
                        cDelete();
                        break;
                    }
                case 'l':
                    {
                        cList();
                        break;
                    }
            }


            void cDelete()
            {
                {
                    configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(@".\config.txt"));
                    Console.Write("Which delimiter do you want to remove from the list?\n>");
                    string input = Console.ReadLine();
                    if (!configuration.LPCDelimiters.Contains(input.First()))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This delimiter isn't in the list!");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    configuration.LPCDelimiters.Remove(input.First());
                    File.WriteAllText(@".\config.txt", JsonSerializer.Serialize(configuration));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"'{input.First()}' was removed from the list!");
                    Console.ForegroundColor = ConsoleColor.White;

                }
            }
            void cAdd()
            {
                {
                    configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(@".\config.txt"));
                    Console.Write("Which delimiter do you want to add? (only input one character)\n>");
                    string input = Console.ReadLine();
                    if (configuration.LPCDelimiters.Contains(input.First()))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This delimiter already exists! For a list of existing delimiters input 'delimiters' and than 'l'.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    configuration.LPCDelimiters.Add(input.First());
                    File.WriteAllText(@".\config.txt", JsonSerializer.Serialize(configuration));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{input.First()} added to the list of delimiters");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            void cList()
            {
                {
                    configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(@".\config.txt"));
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" " + String.Join("\n ", configuration.LPCDelimiters));
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }


    }
    public class CrackGenerator
    {
        public List<char> delimiters = new List<char>() { '.', '-', '_', '+', '~' };

        private int countArguments(Person person)
        {
            int arguments = 0;
            if (!String.IsNullOrEmpty(person.FirstName)) arguments++;
            if (!String.IsNullOrEmpty(person.LastName)) arguments++;
            if (!String.IsNullOrEmpty(person.Birthday)) arguments++;
            if (!String.IsNullOrEmpty(person.Nickname)) arguments++;
            if (!String.IsNullOrEmpty(person.City)) arguments++;
            if (!String.IsNullOrEmpty(person.Country)) arguments++;
            if (!String.IsNullOrEmpty(person.PetsName)) arguments++;
            if (!String.IsNullOrEmpty(person.PetType)) arguments++;
            if (!String.IsNullOrEmpty(person.PetBreed)) arguments++;
            if (!String.IsNullOrEmpty(person.PetsBirtday)) arguments++;
            arguments += person.OtherBirthdays.Count;
            arguments += person.CityAliases.Count;
            return arguments;
        }

        private List<string> getArgumentList(Person person)
        {
            List<string> arguments = new List<string>();
            if (!String.IsNullOrEmpty(person.FirstName)) arguments.Add(person.FirstName);
            if (!String.IsNullOrEmpty(person.LastName)) arguments.Add(person.LastName);
            if (!String.IsNullOrEmpty(person.Birthday)) arguments.Add(person.Birthday);
            if (!String.IsNullOrEmpty(person.Nickname)) arguments.Add(person.Nickname);
            if (!String.IsNullOrEmpty(person.City)) arguments.Add(person.City);
            if (!String.IsNullOrEmpty(person.Country)) arguments.Add(person.Country);
            if (!String.IsNullOrEmpty(person.PetsName)) arguments.Add(person.PetsName);
            if (!String.IsNullOrEmpty(person.PetType)) arguments.Add(person.PetType);
            if (!String.IsNullOrEmpty(person.PetBreed)) arguments.Add(person.PetBreed);
            if (!String.IsNullOrEmpty(person.PetsBirtday)) arguments.Add(person.PetsBirtday);
            arguments.AddRange(person.OtherBirthdays);
            arguments.AddRange(person.CityAliases);
            return arguments;
        }

        public ulong combinationCalculator(Person person, int maxArguments, int delimiters)
        {
            int arguments = countArguments(person);


            ulong output = 0;

            for (int i = 1; i <= maxArguments; i++)
            {
                output += (ulong)Math.Pow(arguments, i) * (ulong)Math.Pow(delimiters, i - 1);
            }
            return output;
        }
        public ulong oneCombinationCalculator(Person person, int maxArguments, int delimiters)
        {
            int arguments = countArguments(person);


            ulong output = 0;


            output += (ulong)Math.Pow(arguments, maxArguments) * (ulong)Math.Pow(delimiters, maxArguments - 1);

            return output;
        }


        //generates combinations for one specific number of arguments
        public (List<List<int>> combinations, ulong progress) generateCombination(Person person, int delimiters, int maxArguments, ulong startProgress, string path)
        {
            ulong maxCount = oneCombinationCalculator(person, maxArguments, delimiters);
            ulong progress = startProgress;
            var cancellationTokenSource = new CancellationTokenSource();
            bool fileWritingDone = false;

            File.Delete(path);

            List<List<int>> results = new List<List<int>>();
            int args = countArguments(person);

            //Task to generate the combinations
            Task.Run(() =>
            {
                int[] lastResult = new int[(2 * maxArguments) - 1];
                for (int i = 0; i < lastResult.Length; i++)
                {
                    lastResult[i] = 0;
                }

                while (true)
                {
                    List<int> currentResult = lastResult.ToList();
                    currentResult[currentResult.Count - 1]++;

                    bool containsArgMax = currentResult.Contains(args) && currentResult.FindLastIndex(X => X == args) % 2 == 0 && currentResult.FindLastIndex(X => X == args) != 0;
                    bool containsDelimiterMax = currentResult.Contains(delimiters) && currentResult.FindLastIndex(X => X == delimiters) % 2 == 1;
                    while (containsArgMax | containsDelimiterMax)
                    {

                        int index = currentResult.FindLastIndex(X => X == args);
                        if (index > 0 && index % 2 == 0)
                        {

                            currentResult[index - 1]++;
                            currentResult[index] = 0;
                        }
                        index = 0;
                        index = currentResult.FindLastIndex(X => X == delimiters);
                        if (index > 0 && index % 2 == 1)
                        {
                            currentResult[index - 1]++;
                            currentResult[index] = 0;
                        }

                        containsArgMax = currentResult.Contains(args) && currentResult.FindLastIndex(X => X == args) % 2 == 0 && currentResult.FindLastIndex(X => X == args) != 0;
                        containsDelimiterMax = currentResult.Contains(delimiters) && currentResult.FindLastIndex(X => X == delimiters) % 2 == 1;
                    }


                    if (currentResult[0] == args)
                    {
                        break;
                    }

                    if (maxCount > 3000000)
                    {
                        results.Add(currentResult);
                        lastResult = currentResult.ToArray();
                        progress++;
                        if (results.Count > 5000000)
                        {
                            fileWritingDone = false;
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (List<int> result in results)
                            {
                                stringBuilder.Append(String.Join("|", result) + "\n");
                            }
                            File.AppendAllText(path, stringBuilder.ToString());
                            results.Clear();
                            fileWritingDone = true;
                        }
                        if (progress == maxCount - 1)
                        {
                            fileWritingDone = false;
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (List<int> result in results)
                            {
                                stringBuilder.Append(String.Join("|", result) + "\n");
                            }
                            File.AppendAllText(path, stringBuilder.ToString());
                            results.Clear();
                            fileWritingDone = true;
                        }
                    }
                    else
                    {

                        results.Add(currentResult);
                        lastResult = currentResult.ToArray();
                        progress++;
                    }

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }

                }
            }, cancellationTokenSource.Token);

            bool pause = false;

            //progress
            Task.Run(() =>
            {
                DateTime startTime = DateTime.Now;
                while (progress < maxCount - 1)
                {

                    while (pause)
                    {
                        Thread.Sleep(1000);
                    }

                    int totalLength = 20; // The total length of the progress bar

                    int doneLength = (int)Math.Floor((double)progress / maxCount * totalLength);
                    int undoneLength = totalLength - doneLength;

                    // Output the progress bar
                    DateTime currentTime = DateTime.Now;
                    TimeSpan eta = TimeSpan.FromSeconds((currentTime - startTime).TotalSeconds * (maxCount - progress) / progress);
                    Console.SetCursorPosition(0, Console.CursorTop - 1);

                    Console.Write("[");
                    for (int i = 0; i < doneLength; i++)
                    {
                        Console.Write("%");
                    }
                    for (int i = 0; i < undoneLength; i++)
                    {
                        Console.Write("-");
                    }
                    Console.Write($"] {Math.Round((double)progress / maxCount * 100, 2)}% ({progress.ToString("N0")} / {maxCount.ToString("N0")}) ETA: {eta.ToString(@"hh\:mm\:ss")}");
                    Console.WriteLine();

                    Thread.Sleep(100);
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                }
                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new String(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write($"[%%%%%%%%%%%%%%%%%%%%] 100% / {maxCount.ToString("N0")}");
                    Console.WriteLine("");
                    Console.WriteLine("Waiting for file writing.");
                    while (!fileWritingDone)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write(new String(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine("Waiting for file writing");
                        Thread.Sleep(500);
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine("Waiting for file writing.");
                        Thread.Sleep(500);
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine("Waiting for file writing..");
                        Thread.Sleep(500);
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine("Waiting for file writing...");
                        Thread.Sleep(500);
                    }
                    Console.WriteLine("Press anny key to continue!");
                }
                return (results, progress);

            }, cancellationTokenSource.Token);
            while (progress < maxCount - 1)
            {
                Console.ReadKey();
                if (progress < maxCount - 1)
                {
                    pause = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\nDo you really want to cancel? The program has to regenerate everything if you want to do it again! If you want to canel, input y.\n>");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Y)
                    {
                        Console.WriteLine("Generation Canceled!");
                        break;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Proggram will continue!");
                    pause = false;
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }

            Console.ForegroundColor = ConsoleColor.White;
            cancellationTokenSource.Cancel();
            return (results, progress);
        }






        //generates combinations from 1 to <maxarguments> arguments
        public List<List<int>> generateCombinations(Person person, int maxarguments, List<char> delimiters, string path)
        {

            List<List<int>> results = new List<List<int>>();
            ulong progress = 0;

            for (int i = 1; i <= maxarguments; i++)
            {
                var values = generateCombination(person, delimiters.Count, i, progress, path);
                results.AddRange(values.combinations);
                progress = values.progress;

            }

            List<List<int>> filteredResults = results
            .Where(l => l.Any())
            .Distinct(new ListComparer<int>())
            .ToList();


            return results;
        }

        //generates the passwords out of the combinations
        public List<string> generation(Person person, int maxArguments, List<char> delimiters, string path)
        {
            List<string> results = new List<string>();

            List<List<int>> allCombinations = generateCombinations(person, maxArguments, delimiters, path);

            List<string> arguments = getArgumentList(person);

            foreach (List<int> combination in allCombinations)
            {
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < combination.Count; i++)
                {
                    if ((i + 1) % 2 == 0)
                    {
                        stringBuilder.Append(delimiters[combination[i]]);
                    }
                    else
                    {
                        stringBuilder.Append(arguments[combination[i]]);
                    }
                }
                results.Add(stringBuilder.ToString());
            }

            return results;
        }
    }

    class ListComparer<T> : EqualityComparer<List<T>>
    {
        public override bool Equals(List<T> x, List<T> y)
        {
            return x.SequenceEqual(y);
        }

        public override int GetHashCode(List<T> obj)
        {
            int hash = 17;
            foreach (T item in obj)
            {
                hash = (hash * 31) + item.GetHashCode();
            }
            return hash;
        }
    }

}

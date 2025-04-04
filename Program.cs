using System;
using System.Collections; // Added for ArrayList
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Media;
using CybersecurityChatbot.Voice_Greeting;
using CybersecurityChatbot.Logo;

namespace CybersecurityChatbot // Unified namespace
{
    class Program
    {
        // UI Constants
        private const string BORDER = "══════════════════════════════════════════════════════════════";
        private const string SECTION = "──────────────────────────────────────────────────────────────";

        // Valid commands for reference
        private static readonly HashSet<string> ValidCommands = new HashSet<string>
        { "phishing", "password", "firewall", "exit" };

        // ArrayList to store question-answer history
        private static readonly ArrayList QuestionAnswerHistory = new ArrayList();

        /// <summary>
        /// Simulates typing effect with delay
        /// </summary>
        private static void TypeEffect(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays welcome message with enhanced UI, without clearing the screen
        /// </summary>
        private static void DisplayWelcome()
        {
            Console.WriteLine(); // Add a blank line for spacing after the logo
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(BORDER);
            Console.WriteLine("║ Welcome to CyberSecurity Chatbot - Your Security Assistant ║");
            Console.WriteLine(BORDER);
            Console.ResetColor();
            TypeEffect("Greetings! I'm CyberSecurity Chatbot, your cybersecurity helper. How can I assist you?");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(SECTION, "Security Options");
            Console.ResetColor();
        }

        /// <summary>
        /// Validates and gets user input with error handling, using the user's name
        /// </summary>
        private static string GetValidInput(string prompt, string name)
        {
            while (true)
            {
                try
                {
                    TypeEffect(prompt);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{name}: ");
                    string input = Console.ReadLine()?.Trim();
                    Console.ResetColor();

                    if (string.IsNullOrEmpty(input))
                        throw new ArgumentException("Please enter a valid response!");

                    return input;
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Processes user's name with string manipulation
        /// </summary>
        private static string ProcessName(string name)
        {
            return char.ToUpper(name.Trim()[0]) + name.Trim().Substring(1).ToLower();
        }

        /// <summary>
        /// Returns predefined cybersecurity responses and stores the question-answer pair in history
        /// </summary>
        private static string GetPredefinedResponse(List<string> queries, string name)
        {
            var responses = new Dictionary<string, string>
            {
                { "phishing", $"{name}, phishing is when attackers trick you into giving sensitive info. Watch for suspicious emails!" },
                { "password", $"For strong passwords, {name}, use 12+ characters, mix cases, numbers, and symbols!" },
                { "firewall", $"A firewall protects your network, {name}. It filters incoming and outgoing traffic." },
                { "exit", $"Stay safe out there, {name}! Signing off!" }
            };

            string response;
            // If multiple keywords are detected, combine responses
            if (queries.Count > 1)
            {
                string combinedResponse = $"{name}, Here's what I know:\n";
                foreach (var query in queries)
                {
                    if (responses.ContainsKey(query))
                    {
                        combinedResponse += $"- {responses[query]}\n";
                    }
                }
                response = combinedResponse;
            }
            else if (queries.Count == 1)
            {
                response = responses[queries[0]];
            }
            else
            {
                return null; // Return null if no valid queries are found
            }

            // Store the question (keywords) and answer in the history, unless the query is "exit"
            if (!queries.Contains("exit"))
            {
                string question = string.Join(", ", queries); // Combine keywords into a single string
                QuestionAnswerHistory.Add(new { Question = question, Answer = response });
            }

            return response;
        }

        /// <summary>
        /// Extracts valid query keywords from user input
        /// </summary>
        private static List<string> ExtractQueryKeywords(string input, string name)
        {
            // Split on both spaces and commas, and remove empty entries
            string[] words = input.ToLower().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> foundCommands = words.Where(word => ValidCommands.Contains(word)).Distinct().ToList();

            if (foundCommands.Any())
            {
                return foundCommands;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                TypeEffect($"Sorry {name}, I didn’t recognize any keywords. Please include one of these:");
                Console.ForegroundColor = ConsoleColor.Magenta;
                TypeEffect(string.Join(" | ", ValidCommands));
                Console.ResetColor();
                Thread.Sleep(1000);
                return new List<string>(); // Return an empty list if no keywords are found
            }
        }

        /// <summary>
        /// Gets valid queries from the user, handling sentences
        /// </summary>
        private static List<string> GetValidQueries(string prompt, string name)
        {
            while (true)
            {
                string input = GetValidInput(prompt, name);
                List<string> queries = ExtractQueryKeywords(input, name);

                if (queries.Any())
                {
                    return queries;
                }
                // If no valid queries are found, loop back to prompt the user again
            }
        }

        /// <summary>
        /// Main chatbot logic
        /// </summary>
        private static void RunChatbot()
        {
            DisplayWelcome();

            string name = ProcessName(GetValidInput("What's your name?", "User"));
            TypeEffect($"Great to meet you, {name}! Let's talk security.");
            Thread.Sleep(1000);

            // Ask "How can I help you today?" only once at the start
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(SECTION, $"Chat with {name}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Options: phishing | password | firewall | exit");
            Console.ResetColor();

            string prompt = $"How can I help you today, {name}?";
            List<string> queries = GetValidQueries(prompt, name);

            while (true)
            {
                // Only display the response if there are valid queries
                if (queries.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("CyberSecurity Chatbot: ");
                    Console.ResetColor();
                    string response = GetPredefinedResponse(queries, name);
                    if (response != null) // Check to avoid passing null to TypeEffect
                    {
                        TypeEffect(response);
                    }

                    if (queries.Contains("exit"))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(BORDER);
                        TypeEffect($"Closing down... Stay secure, {name}!");
                        break;
                    }

                    Thread.Sleep(1000);

                    // After a valid response, ask the new prompt
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(SECTION, $"Chat with {name}");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Options: phishing | password | firewall | exit");
                    Console.ResetColor();

                    prompt = " If you have further questions, feel free to ask and if not, type exit";
                }

                // Get the next input
                string nextAction = GetValidInput(prompt, name);
                queries = ExtractQueryKeywords(nextAction, name);

                if (queries.Contains("exit"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(BORDER);
                    TypeEffect($"Thanks for the chat, {name}! Stay vigilant!");
                    break;
                }
            }

            
        }

        /// <summary>
        /// Program entry point
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                // Play welcome audio
                new welcome_message();

                // Display logo (this will now persist)
                new logo_design();

                // Run chatbot without clearing the screen
                RunChatbot();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Thank you for using CyberSecurity Chatbot!");
                Console.ResetColor();
                Thread.Sleep(2000);
            }
        }
    }

    namespace Voice_Greeting
    {
        public class welcome_message
        {
            // Constructor
            public welcome_message()
            {
                // Getting full location of the project
                string full_location = AppDomain.CurrentDomain.BaseDirectory;

                // Replace Bin/Debug in file location
                string new_path = full_location.Replace("bin\\Debug\\", "");

                // Try and catch
                try
                {
                    // Catch the Path
                    string full_path = Path.Combine(new_path, "Voice Greeting.wav");

                    // Try or catch the instance for the SoundPlayer class
                    using (SoundPlayer play = new SoundPlayer(full_path))
                    {
                        // Play the file
                        play.PlaySync(); // Plays synchronously so logo waits for audio to finish
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine($"Audio Error: {error.Message}");
                }
            }
        }//end of welcome_messsage class
    }//end of voice_greeting namespace

    namespace Logo
    {
        public class logo_design
        {
            // Constructor
            public logo_design()
            {
                //getting the full path
                string path_project = AppDomain.CurrentDomain.BaseDirectory;

                //then replace the bin\\debug\\
                string new_path_project = path_project.Replace("bin\\Debug\\", "");

                //combining the prject full path with the image name
                string full_path = Path.Combine(new_path_project, "Logo.jpg");

                Bitmap image = new Bitmap(full_path);
                image = new Bitmap(image, new Size(210, 85));

                //For loop,for inner and outer
                for (int height = 0; height < image.Height; height++)
                {
                    for (int width = 0; width < image.Width; width++)
                    {
                        Color pixelColor = image.GetPixel(width, height);
                        int color = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                        //using char
                        char ascii_design = color > 200 ? '.' : color > 150 ? '*' : color > 100 ? '0' : color > 50 ? '#' : '@';
                        Console.Write(ascii_design);//output of design

                    }//end of the inner for loop
                    Console.WriteLine();
                }//end of the for loop outer

            }
        }//end of logo_design class
    }//end of Logo namespace
}//end of CybersecurityChatbot namespace
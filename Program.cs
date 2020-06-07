using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace ProjectGracenoteAlpha1Point1
{
    class Program
    {
        /// <Evaluation Points>
        /// EVALUATION POINTS
        /// Mention how I declared all variables at the start and uncommented them one by one
        /// Variables had to be made static or they were unaccessible
        /// Had to add error variable
        /// ConsoleKey had to be changed to ConsoleKeyInfo
        /// Included extra validation for main menu
        /// Included Easter egg in clip loader to include delete SQL statement
        /// Had to use validation 1 for every menu
        /// Added footer to variables
        /// Added error flag
        /// Added arrangementNameInput
        /// Footer could not be displayed on every form
        /// Changed time signature to short
        /// Changed tonic note to string
        /// Added new variable for tonic note after it's been converted
        /// Added arrangement object
        /// Added another variable for major
        /// Added extra variable for time signature for after conversion, as a short
        /// Time signature must be greater than 0 and a whole number
        /// Introduced custom write functions to show colours
        /// Added name to clip
        /// Changed accidentals array to short
        /// Added saved bool to Clip class
        /// Had to change scope of properties in Arrangement from protected to private
        /// Program does not contain provision for all possible notes
        /// Had to change sharps so now it's only a variable instead of a function
        /// Changed syncopation randomisation to static so it doesn't have to be declared again
        /// Pseudocode does not allow for creating rests
        /// Had to change some of the note generation as it was making index out of bounds errors
        /// Added Selected Index variable for clip selection to class scope
        /// MENTION TO RESTS ALLOWED!!!!!!!!!
        /// Changed note list assignment so it is after the loop
        /// Bar numbers don't display properly
        /// Could limit clip viewer to only the notes that were specified, but this would be needlessly complicated and the client didn't ask for that feature
        /// Added new class - Octave
        /// Should have made each behaviour inter-compatible with each other - example is the displaying notes problem
        /// Had to add 0 on to the end of the pitch while converting it to a string
        /// Changed while loop in creating clip to do while and decremented spaceLeft
        /// LEARN THAT YOU CAN'T USE STATIC FOR EVERYTHING
        /// Changed time signature treatment
        /// Closing the DB connection must come after extracting data
        /// Chose to make user choose numbers in the arrangement list rather than the name (easier to type)
        /// Had to change the clip encoding to using letters rather than numbers
        /// Ended up writing part of the same functon twice - could have used overloading?
        /// Clips can only be saved if their arrangements have been saved
        /// The order of clips is not really accurate
        /// Kept passing username as a reference throughout all of the functions
        /// Shouldn't have used varchar(50) for password
        /// </Evaluation points>
        
        #region Public variables

        public static MySqlConnection SqlConnection;
        public static int CurrentUserID;

        #endregion

        #region Private variables

        private static ConsoleKeyInfo newAccountChoice;
        private static string usernameInput;
        private static string passwordInput;
        private static string newUsername;
        private static string newPassword;
        private static string confirmNewPassword;
        private static ConsoleKeyInfo optionChoice;
        private static string arrangementNameInput;
        private static string tonicNoteInput;
        private static string tonicNote;
        private static string majorMinorInput;
        private static bool major;
        private static string timeSignatureInput;
        private static short timeSignature;
        //private int arrangementIDChoice;
        //private string oldPassword;
        //private int clipIDChoice;
        private static string error;
        private static string footer;
        private static bool errorFlag;
        private static HashAlgorithm hash;

        #endregion

        #region Functions and methods

        #region Pre main menu

        /// <summary>
        /// Leads to Welcome
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Instantiate connection and set correct credentials
            SqlConnection = new MySqlConnection("SERVER=127.0.0.1; DATABASE='ProjectProNote'; UID='root'; PASSWORD=''");
            hash = new SHA256Managed();
            Welcome();
        }

        /// <summary>
        /// Displays welcome window
        /// Leads to ValidateLogIn
        /// </summary>
        /// <param name="args"></param>
        static void Welcome()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n\n\n\n\n\n");
            WriteLineCyan("                         Welcome to Project ProNote\n");
            WriteLineRed("           WARNING: This is version Alpha 1.1. There will be bugs!");
            Console.WriteLine();
            Console.WriteLine();
            WriteLineCyan("                      Press F1 to create a new account");
            WriteLineCyan("              or any other key to log in to an existing account");
            // Check input key for creating new account
            newAccountChoice = Console.ReadKey();
            // If it is F1 then create new account
            if (newAccountChoice.Key == ConsoleKey.F1)
            {
                ValidateNewAccount();
            }
            else
            {
                ValidateLogIn(0);
            }
        }

        /// <summary>
        /// Validation to meet: 1, 9
        /// Leads to ValidateLogIn
        /// </summary>
        private static void ValidateNewAccount()
        {
            error = "";
            while (true)
            {
                Console.Clear();
                WriteLineCyan("                             New Account Wizard");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n" + error + "\n");
                Console.ForegroundColor = ConsoleColor.White;
                WriteLineCyan("Please enter the following details");
                Console.WriteLine();
                // User enters username
                WriteMagenta("Username: ");
                newUsername = Console.ReadLine();
                // Check if username isn't blank
                if (newUsername == "")
                {
                    error = "Please don't leave the username blank";
                }
                else
                {
                    // User enters password
                    WriteMagenta("Password: ");
                    newPassword = Console.ReadLine();
                    if (newPassword == "")
                    {
                        error = "Please don't leave the password blank";
                    }
                    else
                    {
                        // User enters confirm password
                        WriteMagenta("Confirm password: ");
                        confirmNewPassword = Console.ReadLine();
                        if (confirmNewPassword == "")
                        {
                            error = "Please don't leave the confirm password blank";
                        }
                        else
                        {
                            // If both passwords do not match
                            if (confirmNewPassword != newPassword)
                            {
                                error = "Your two passwords do not match";
                            }
                            // If there are no problems
                            else
                            {
                                // Hash password
                                byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(newPassword);
                                byte[] hashBytes = hash.ComputeHash(plainTextBytes);
                                string hashedPassword = Convert.ToBase64String(hashBytes);
                                
                                // Only instantiated when all validation passes, hence why not at the start of the class
                                NewAccount newAccount = new NewAccount();
                                newAccount.SetDetails(ref newUsername, ref hashedPassword);
                                if (!newAccount.SaveDetails())
                                {
                                    error = "Account could not be created. Please try again later";
                                }
                                else
                                {
                                    ValidateLogIn(1);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validation to meet: 1
        /// Leads to LogIn
        /// If mode is set to 0, it displays as if it's an existing user If it's set as 1, it displays as if a user has just created an account
        /// If it's 2, then a username hasn't been entered correctly
        /// If it's 3, the password isn't correct
        /// </summary>
        private static void ValidateLogIn(short mode)
        {
            error = "";
            // Loop until details are entered correctly
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\n\n\n\n");
                // If any errors need to be shown before the user enters any details, they are shown here
                if (mode == 1)
                {
                    WriteLineCyan("                 Account successfully created. Log in below");
                }
                else if (mode == 2)
                {
                    WriteLineCyan("           The username you entered doesn't exist. Please try again:");
                }
                else if (mode == 3)
                {
                    WriteLineCyan("                  The password is incorrect. Please try again:");
                }
                // If there are no problems (mode would be 0)
                else
                {
                    WriteLineCyan("                                     Login");
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n" + error + "\n");
                Console.ForegroundColor = ConsoleColor.White;
                // User enters password
                WriteMagenta("                         Username: ");
                usernameInput = Console.ReadLine();
                // Check if username is not empty
                if (usernameInput == "")
                {
                    error = "                    Please don't leave the username blank";
                }
                else
                {
                    // User enters password
                    WriteMagenta("                         Password: ");
                    passwordInput = Console.ReadLine();
                    if (passwordInput == "")
                    {
                        error = "                    Please don't leave the password blank";
                    }
                    else
                    {
                        // Hash password before using it
                        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(passwordInput);
                        byte[] hashBytes = hash.ComputeHash(plainTextBytes);
                        string hashedPasswordInput = Convert.ToBase64String(hashBytes);
                        
                        LogIn(ref usernameInput, ref hashedPasswordInput);
                        break;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Leads to CreateNewArrangement, LoadArrangement, ChangePassword, Help or LogOut
        /// </summary>
        /// <param name="username"></param>
        /// 
        public static void MainMenu(ref string username)
        {
            #region Input
            errorFlag = false;
            footer = "Logged in as " + username + " ";
            // Runs until valid option is input
            do
            {
                Console.Clear();
                WriteLineCyan("                                    Main Menu");
                Console.WriteLine();
                WriteLineCyan("Please choose an option by pressing its number:");
                Console.WriteLine();
                WriteLineMagenta("1. Create a new arrangement");
                WriteLineMagenta("2. Load an arrangement");
                WriteLineMagenta("3. Change password");
                WriteLineMagenta("4. Help");
                WriteLineMagenta("5. Log out");
                Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                // If there is no error, then show the footer as yellow
                if (errorFlag == false)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                // If there is an error, show the footer as red
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(footer);
                Console.ForegroundColor = ConsoleColor.White;
                optionChoice = Console.ReadKey();
                #endregion
                // Perform select case on pressed key
                switch (optionChoice.Key)
                {
                    case ConsoleKey.D1:
                        CreateNewArrangement(ref username);
                        break;
                    case ConsoleKey.D2:
                        LoadArrangement(0, ref username);
                        break;
                    case ConsoleKey.D3:
                        ChangePassword(ref username);
                        break;
                    case ConsoleKey.D4:
                        Help(ref username);
                        break;
                    case ConsoleKey.D5:
                        LogOut();
                        break;
                    default:
                        // If no valid option chosen, error shown
                        errorFlag = true;
                        footer = "Please enter a valid option ";
                        break;
                }
            } while (errorFlag);
        }

        #region Main menu items

        /// <summary>
        /// Validation to meet: 1, 4, 5, 6
        /// Leads to Arrangement.DisplayArrangement()
        /// </summary>
        private static void CreateNewArrangement(ref string username)
        {
            bool errorFlag2 = false;
            error = "";
            do
            {
                Console.Clear();
                WriteLineCyan("                            Create a New Arrangement");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                WriteLineCyan("Please enter:");
                Console.WriteLine();
                WriteMagenta("Arrangement name: ");
                // Input arrangement name
                arrangementNameInput = Console.ReadLine();
                // Validate input for each solution
                // If name is blank, show error
                if (arrangementNameInput == "")
                {
                    error = "Please don't leave the name blank";
                }
                else
                {
                    // Instantiate arrangement object
                    WriteMagenta("Tonic note (use # for sharp and b for flat): ");
                    // Input tonic note
                    tonicNoteInput = Console.ReadLine();
                    // If the tonic note is blank, say so
                    if (tonicNoteInput == "")
                    {
                        error = "Please don't leave the tonic note blank";
                    }
                    // If the note is invalid, say so
                    // In the meantime, this function attempts to convert the tonic note into its numerical value
                    else if (!IsNoteValid(ref tonicNoteInput, ref tonicNote))
                    {
                        error = "Invalid tonic note. Please try again";
                    }
                    else
                    {
                        // Input major/minor
                        WriteMagenta("Major/minor: ");
                        majorMinorInput = Console.ReadLine();
                        // If the major/minor is blank or it's not valid, show error
                        // In the meantime, if valid, store the value
                        if (!IsMajorMinorValid(majorMinorInput))
                        {
                            error = "Please choose either major or minor";
                        }
                        else
                        {
                            // Input time signature
                            WriteMagenta("Time signature (top number only, bottom number is 4): ");
                            timeSignatureInput = Console.ReadLine();
                            // If time signature is invalid, show error
                            // If valid, store value
                            if (!IsTimeSignatureValid(ref timeSignatureInput, ref timeSignature))
                            {
                                error = "Please enter a whole number greater than 0 for time signature";
                            }
                            else
                            {
                                // If all details entered are correct, create arrangement
                                errorFlag2 = true;
                                Arrangement arrangement = new Arrangement();
                                arrangement.SetDetails(ref arrangementNameInput, ref tonicNote, ref major, ref timeSignature);
                                arrangement.CreateArrangement();
                                // Once created, display the arrangement to the user
                                arrangement.DisplayArrangement(ref username, 0);
                            }
                        }
                    }
                }
            } while (!errorFlag2);
        }
        #region New arrangement validation

        private static bool IsTimeSignatureValid(ref string timeSignatureInput, ref short timeSignature)
        {
            bool isNumberValid = Int16.TryParse(timeSignatureInput, out timeSignature);
            // Check if the input is blank, or if the input is not a number. If so, return false
            if (timeSignatureInput == "" || !isNumberValid)
            {
                return false;
            }
            // If the input is a valid number, return true
            else
            {
                return true;
            }
        }

        // Passing in majorMinorInput by val so that it can be converted to lowercase
        private static bool IsMajorMinorValid(string majorMinorInput)
        {
            bool flag = true;
            majorMinorInput = majorMinorInput.ToLower();
            if (majorMinorInput == "major")
            {
                major = true;
            }
            else if (majorMinorInput == "minor")
            {
                major = false;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        // Function both validates note and converts note to its numerical value (if it's valid)
        private static bool IsNoteValid(ref string tonicNoteInput, ref string tonicNote)
        {
            bool flag = false;
            // Take note input and convert to lowercase
            string tonicNoteInputLower = tonicNoteInput.ToLower();
            // The note must be less than 3 characters, otherwise it is not a note
            if (tonicNoteInputLower.Length < 3)
            {
                // Take the first character of the input note. This should be a note letter from A to G. Convert it to its numerical value from 01-07
                switch (tonicNoteInputLower[0])
                {
                    // As each of these letters are valid, if the chosen note is one of these, make flag true
                    case 'c':
                        tonicNote = "1";
                        flag = true;
                        break;
                    case 'd':
                        tonicNote = "2";
                        flag = true;
                        break;
                    case 'e':
                        tonicNote = "3";
                        flag = true;
                        break;
                    case 'f':
                        tonicNote = "4";
                        flag = true;
                        break;
                    case 'g':
                        tonicNote = "5";
                        flag = true;
                        break;
                    case 'a':
                        tonicNote = "6";
                        flag = true;
                        break;
                    case 'b':
                        tonicNote = "7";
                        flag = true;
                        break;
                }
                // If the note is valid, check if it has a sharp or flat, and convert that accordingly
                // If it has a sharp or flat, the input string will have 2 characters
                if (flag)
                {
                    if (tonicNoteInputLower.Length == 2)
                    {
                        switch (tonicNoteInputLower[1])
                        {
                            // Sharps are represented as 1, flats as 2
                            case '#':
                                tonicNote = "1" + tonicNote;
                                break;
                            case 'b':
                                tonicNote = "2" + tonicNote;
                                break;
                            default:
                                flag = false;
                                break;
                        }
                    }
                    // If there are no accidentals, put a 0 on the front
                    else
                    {
                        tonicNote = "0" + tonicNote;
                    }
                }
            }
            return flag;
        }

        #endregion
        private static void Help(ref string username)
        {
            Console.Clear();
            WriteLineCyan("                                      Help\n\nIf you have any questions, feel free to send me an email:\nross.hugh.duncan@gmail.com\n\n© Ross Duncan 2015-2016\n");
            WriteMagenta("Press any key to return to the main menu\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            WriteYellow("Logged in as " + username + " ");
            Console.ReadKey();
            MainMenu(ref username);
        }

        /// <summary>
        /// Leads to Welcome
        /// </summary>
        private static void LogOut()
        {
            // Go back to login screen and clear current account ID
            CurrentUserID = -1;
            // Main loads the first welcome screen
            Console.Clear();
            Welcome();
        }

        #endregion

        #region Colour changing

        // All are public so they can be used in the other classes

        // Function for writing text as green, then changing back to white
        public static void WriteGreen(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Function for cyan text
        public static void WriteLineCyan(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteYellow(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLineRed(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteGrey(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLineMagenta(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteMagenta(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteRed(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteBlue(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteCyan(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLineGreen(string toDisplay)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(toDisplay);
            Console.ForegroundColor = ConsoleColor.White;
        }

        #endregion

        /// <summary>
        /// Validation to meet: 2
        /// Leads to MainMenu
        /// </summary>
        private static void LogIn(ref string username, ref string password)
        {
            // Retrieve details
            // First, check that username exists. If it doesn't, an exception will thrown, which we need to catch
            MySqlCommand findDetails = SqlConnection.CreateCommand();
            findDetails.CommandText = "select PK_AccountID, Username, Password from Accounts where Username = '" + username + "'";
            MySqlDataReader findDetailsReader;
            string retrievedPassword = "";
            OpenConnection();
            findDetailsReader = findDetails.ExecuteReader();
            // Extract password into string and current user ID into integer
            // (whilst the reader is reading from the database)
            if (findDetailsReader.Read())
            {
                retrievedPassword = findDetailsReader.GetString(2);
                // If the username is retrievable but the password is incorrect
                if (retrievedPassword == password)
                {
                    // Set current user ID
                    CurrentUserID = findDetailsReader.GetInt32(0);
                    findDetailsReader.Close();
                    SqlConnection.Close();
                    // Show menu the main menu
                    MainMenu(ref username);
                }
                // No?
                else
                {
                    findDetailsReader.Close();
                    SqlConnection.Close();
                    ValidateLogIn(3);
                }

            }
            // If the username cannot be read (it is invalid), show error
            else
            {
                findDetailsReader.Close();
                ValidateLogIn(2);
            }
            MainMenu(ref username);
        }

        public static void OpenConnection()
        {
            // Try to open connection. If already open, don't bother
            try
            {
                SqlConnection.Open();
            }
            catch
            {
                // Close connection and then open it again
                SqlConnection.Close();
                SqlConnection.Open();
            }
        }

        public struct ArrangementRow
        {
            public int ID;
            public string Name;
        }

        public struct Pointer
        {
            public int ListItem;
            public int ID;
        }
        
        private static void LoadArrangement(int mode, ref string username)
        {
            #region Retrieving data

            MySqlCommand command = SqlConnection.CreateCommand();
            command.CommandText = "select PK_ArrangementID, ArrangementName from Arrangements where FK_AccountID = " + CurrentUserID;
            MySqlDataReader reader;
            List<ArrangementRow> list = new List<ArrangementRow>();
            ArrangementRow newRow;
            // Read list of arrangements from table
            OpenConnection();
            reader = command.ExecuteReader();
            // Loop until all arrangement names have been retrieved and store each row in the list
            while (reader.Read())
            {
                newRow.ID = reader.GetInt32(0);
                newRow.Name = reader.GetString(1);
                list.Add(newRow);
            }
            SqlConnection.Close();

            #endregion

            // The different arrangements will be displayed as a numbered list. However, each number will need to point to an ID within the arrangement table
            // Therefore, I need to create a list of pointers
            List<Pointer> pointers = new List<Pointer>();
            Pointer newPointer;
            Console.Clear();
            WriteLineCyan("                               Load Arrangement\n");
            // First check if the user has no saved arrangements
            // If that's true, show an error
            if (list.Count == 0)
            {
                WriteLineRed("\nThere are no arrangements saved to this account.\nPress any key to return to the main menu.");
                WriteYellow("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nLogged in as " + username + " ");
                Console.ReadKey();
                MainMenu(ref username);
            }
            // If that's not the case, then run the rest of the code as usual
            else
            {
                // Print each of the arrangement names in the list
                for (int i = 0; i < list.Count(); i++)
                {
                    // Must be +1 to prevent 0-indexing
                    newPointer.ListItem = i + 1;
                    newPointer.ID = list[i].ID;
                    pointers.Add(newPointer);
                    WriteLineGreen((i + 1) + ". " + list[i].Name);
                }
                switch (mode)
                {
                    // Normal mode, where there is no errors
                    case 0:
                        // Prompt user to enter choice
                        WriteCyan("\nEnter number of arrangement to load: ");
                        break;
                    case 1:
                        WriteCyan("\nInvalid choice. Please try again: ");
                        break;
                }
                int choice;
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    // Translate choice number into its corresponding arrangement ID
                    int id = new int();
                    // Flag is needed to check if the choice is valid
                    bool flag = false;
                    for (int i = 0; i < pointers.Count; i++)
                    {
                        if (pointers[i].ListItem == choice)
                        {
                            id = pointers[i].ID;
                            flag = true;
                            break;
                        }
                    }
                    // If the choice is invalid, prompt to choose the arrangement again
                    if (!flag)
                    {
                        LoadArrangement(1, ref username);
                    }
                    // If the choice is valid, create a new arrangement object and load the data
                    else
                    {
                        Arrangement loadingArrangement = new Arrangement();
                        loadingArrangement.LoadArrangement(ref id, ref username);
                    }
                }
                catch
                {
                    // If the user makes an invalid input, run the function again but with showing an error
                    LoadArrangement(1, ref username);
                }
            }
        }

        private static void ChangePassword(ref string username)
        {
            bool flag = false;
            string error = "";
            string currentPassword;
            do
            {
                Console.Clear();
                WriteLineCyan("                                Change Password\n\nPlease enter:\n");
                WriteLineRed(error + "\n");
                WriteMagenta("Your current password: ");
                // Enter current password
                currentPassword = Console.ReadLine();
                // If the password is empty
                if (currentPassword == "")
                {
                    error = "Please don't leave the password blank";
                }
                else
                {
                    // If the password is not correct
                    if (!ValidateCurrentPassword(ref currentPassword))
                    {
                        error = "The password is incorrect. Please try again";
                    }
                    else
                    {
                        // Enter new password
                        WriteMagenta("Your new password: ");
                        newPassword = Console.ReadLine();
                        // Check if empty
                        if (newPassword == "")
                        {
                            error = "Please don't leave the password blank";
                        }
                        else
                        {
                            // Confirm new password
                            WriteMagenta("Confirm your new password: ");
                            confirmNewPassword = Console.ReadLine();
                            if (confirmNewPassword == "")
                            {
                                error = "Please don't leave the confirm password blank";
                            }
                            else if (confirmNewPassword != newPassword)
                            {
                                error = "Your two passwords don't match. Please try again";
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                    }
                }
            } while (flag == false);

            // If no errors, first hash password then send to database
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(newPassword);
            byte[] hashBytes = hash.ComputeHash(plainTextBytes);
            string hashedPassword = Convert.ToBase64String(hashBytes);

            MySqlCommand sendCommand = SqlConnection.CreateCommand();
            sendCommand.CommandText = "update Accounts set Password = \"" + hashedPassword + "\" where PK_AccountID = " + CurrentUserID;
            OpenConnection();
            sendCommand.ExecuteNonQuery();
            SqlConnection.Close();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            WriteLineGreen("\nYour password has been changed");
            stopwatch.Start();
            // Wait for one second before displaying success message
            while (stopwatch.ElapsedMilliseconds < 1000)
            {
                // Wait
            }
            MainMenu(ref username);
        }

        private static bool ValidateCurrentPassword(ref string currentPassword)
        {
            bool flag = false;

            // Hash password
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(currentPassword);
            byte[] hashBytes = hash.ComputeHash(plainTextBytes);
            string hashedCurrentPassword = Convert.ToBase64String(hashBytes);
            
            MySqlCommand command = SqlConnection.CreateCommand();
            command.CommandText = "select Password from Accounts where PK_AccountID = " + CurrentUserID;
            string realPassword = "";
            MySqlDataReader reader;
            OpenConnection();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                realPassword = reader.GetString(0);
            }
            SqlConnection.Close();
            // Check if password is valid
            if (realPassword == hashedCurrentPassword)
            {
                flag = true;
            }

            return flag;
        }

        #endregion
    }
}

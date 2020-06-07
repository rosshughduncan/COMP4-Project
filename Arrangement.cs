using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ProjectGracenoteAlpha1Point1
{
    class Arrangement
    {
        #region Properties

        private List<Clip> clips;
        private string name;
        // tonicNoteNumerical and sharps must be public and static as they need to be accessed from the Clip class
        public static string tonicNoteNumerical;
        public bool sharps;
        public bool major;
        private static int timeSignature;
        private bool saved;
        // Stores the index of the clip currently selected
        private int selectedClipIndex = 0;
        private int ID;

        #endregion

        #region Private variables

        private string rangeOfOctavesInput;
        private string rangeOfNotesInput;
        private string syncopationRandomisationInput;
        private int rangeOfOctaves;
        private int rangeOfNotes;
        private int syncopationRandomisation;

        #endregion

        #region Methods

        // Constructor
        public Arrangement()
        {
            clips = new List<Clip>();
            saved = false;
            ID = 0;
        }
        
        // Access modifier
        public void SetDetails(ref string Name, ref string TonicNote, ref bool Major, ref short TimeSignature)
        {
            name = Name;
            tonicNoteNumerical = TonicNote;
            major = Major;
            timeSignature = TimeSignature;
        }

        // Needs username to display username in footer
        public void DisplayArrangement(ref string username, short error)
        {
            bool flag = true;
            bool flag2 = true;
            string footer = "";
            // If there is an error, display error message
            if (error == 0)
            {
                footer = "Logged in as " + username + " ";
            }
            // If there are no clips available to display, say so
            else if (error == 1)
            {
                footer = "There are no clips to display ";
                flag2 = false;
            }
            // if the user tries to enter a clip but it's already saved
            else if (error == 2)
            {
                footer = "This clip is already saved ";
                flag2 = false;
            }
            else if (error == 3)
            {
                footer = "Insert a clip to save ";
                flag2 = false;
            }
            //// If the user tries to insert a clip but they haven't saved the arrangement first, show this error
            //else if (error == 2)
            //{
            //    footer = "You cannot save a clip without saving the arrangement first ";
            //    flag2 = false;
            //}
            // Run until the user has chosen a valid option to continue
            do
            {
                #region Display
                Console.Clear();
                Program.WriteLineCyan(name + "\n");
                // Clip display section
                // If no clips are within arrangement yet, say so
                if (clips.Count == 0)
                {
                    Program.WriteLineRed("                                < No clips >");
                }
                // If there are clips, show them appropriately
                else
                {
                    Clip currentClip;
                    // Display bar numbers for each clip
                    #region Display bar numbers
                    for (int i = 0; i < clips.Count; i++)
                    {
                        // Display bar number halfway along clip name
                        // If there is no name for the clip, use 4 characters
                        int length;
                        if (clips[i].GetName() == null)
                        {
                            length = 4;
                        }
                        else
                        {
                            length = clips[i].GetName().Length;
                        }
                        int halfLength = length / 2;
                        // If clip name is an odd number of characters, miss out one space
                        if (length % 2 == 0)
                        {
                            PrintSpaces(halfLength);
                        }
                        else
                        {
                            PrintSpaces(halfLength - 1);
                        }
                        Program.WriteGrey((i + 1).ToString());
                        PrintSpaces(halfLength);
                    }
                    Console.Write("\n");
                    #endregion
                    // Now, display the actual clips
                    #region Display clips
                    // The clip selected will be displayed as a different colour to the other clips
                    for (int i = 0; i < clips.Count; i++)
                    {
                        currentClip = clips[i];
                        // If the clip is saved, display as green
                        if (currentClip.IsSaved())
                        {
                            // If the clip is the one currently selected, display as a lighter green than the unselected clips
                            if (i == selectedClipIndex)
                            {
                                Console.BackgroundColor = ConsoleColor.Green;
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                            }
                            Console.Write(currentClip.GetName() + " ");
                        }
                        // If not saved, show as red
                        else
                        {
                            // If the clip is the one currently selected, display as a lighter green than the unselected clips
                            if (i == selectedClipIndex)
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                            }
                            Console.Write(" *   ");
                        }
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    #endregion
                }
                Console.WriteLine();
                Program.WriteLineCyan("\nEnter number for action:\n");
                Program.WriteLineMagenta("1 - Insert clip     2 - Load clip     3 - Save clip     4 - Delete clip\n5 - View clip     6 - Save arrangement     7 - Exit\n");
                Program.WriteLineCyan("Use arrow keys to select clip\n\n\n\n\n\n\n\n\n\n\n\n\n");
                if (flag2)
                {
                    Program.WriteYellow(footer);
                }
                else
                {
                    Program.WriteRed(footer);
                }
                #endregion
                flag = true;
                // Take key input from user to decide the next action
                ConsoleKeyInfo nextAction = Console.ReadKey();
                switch (nextAction.Key)
                {
                    case ConsoleKey.D1:
                        InsertClip(ref username);
                        break;
                    case ConsoleKey.D2:
                        LoadClip(ref username, false);
                        break;
                    case ConsoleKey.D3:
                        SaveClip(ref username);
                        break;
                    case ConsoleKey.D4:
                        DeleteClip(ref username);
                        break;
                    case ConsoleKey.D5:
                        ViewClip(ref username, ref timeSignature);
                        break;
                    case ConsoleKey.D6:
                        SaveArrangement(ref username);
                        break;
                    case ConsoleKey.D7:
                        Exit(ref username);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (selectedClipIndex > 0)
                        {
                            selectedClipIndex--;
                        }
                        flag = false;
                        break;
                    case ConsoleKey.RightArrow:
                        if (selectedClipIndex < clips.Count - 1)
                        {
                            selectedClipIndex++;
                        }
                        flag = false;
                        break;
                    // If no valid option is input, display error
                    default:
                        flag2 = false;
                        footer = "Please choose a valid option";
                        break;
                }
            } while (!flag);
        }

        // Prints spaces for the bar numbers
        private void PrintSpaces(int halfLength)
        {
            for (int j = 1; j <= halfLength; j++)
            {
                Console.Write(" ");
            }
        }

        private void InsertClip(ref string username)
        {
            bool flag = false;
            string error = "";
            do
            {
                Console.Clear();
                Program.WriteLineCyan("                                Insert Clip");
                Console.WriteLine();
                Program.WriteLineRed(error);
                Console.WriteLine();
                Program.WriteLineCyan("Enter details for a new clip below:");
                Console.WriteLine();
                // Input range of octaves
                Program.WriteMagenta("Range of octaves: ");
                rangeOfOctavesInput = Console.ReadLine();
                // Validate range of octaves
                if (rangeOfOctavesInput == "")
                {
                    error = "Please don't leave the range of octaves blank";
                }
                else if (!Int32.TryParse(rangeOfOctavesInput, out rangeOfOctaves))
                {
                    error = "Please enter a valid range of octaves (a whole number)";
                }
                else
                {
                    // Input range of notes
                    Program.WriteMagenta("Range of notes (from 1 to 8): ");
                    rangeOfNotesInput = Console.ReadLine();
                    // Validate range of notes
                    if (rangeOfNotesInput == "")
                    {
                        error = "Please don't leave the range of notes blank";
                    }
                    // Check if range of notes is a number, then check if it is between 1 and 8
                    else if (!Int32.TryParse(rangeOfNotesInput, out rangeOfNotes) || rangeOfNotes < 1 || rangeOfNotes > 8)
                    {
                        error = "Please enter a valid range of notes (a whole number between 1 and 8)";
                    }
                    else
                    {
                        // Input syncopation randomisation
                        Program.WriteMagenta("Syncopation randomisation (from 1 to 10)\n(roughly how much do you want the note values to change?): ");
                        syncopationRandomisationInput = Console.ReadLine();
                        // Validate syncopation randomisation
                        if (syncopationRandomisationInput == "")
                        {
                            error = "Please don't leave the syncopation randomisation blank";
                        }
                        else if (!Int32.TryParse(syncopationRandomisationInput, out syncopationRandomisation) || syncopationRandomisation < 1 || syncopationRandomisation > 10)
                        {
                            error = "Please enter a valid syncopation randomisation value (a whole number between 1 and 10)";
                        }
                        // If all input is valid, create new clip and return to the arranger
                        else
                        {
                            flag = true;
                            Clip newClip = new Clip();
                            newClip.SetDetails(ref rangeOfOctaves, ref rangeOfNotes, ref syncopationRandomisation);
                            newClip.CreateClip(ref sharps, ref major);
                            clips.Add(newClip);
                        }
                    }
                }
            } while (!flag);
            DisplayArrangement(ref username, 0);
        }

        public string GetTonicNoteNumerical()
        {
            return tonicNoteNumerical;
        }

        public static int GetNumberAccidentals(ref bool sharps, ref bool major)
        {
            int numberAccidentals = new int();

            // This section will look up the number of accidentals for each key signature
            // I will be including conditions for both the major key and their equivalent minor keys
            // This is based on the circle of fifths as shown in the Design Section 3.3.1

            // G major / E minor have 1 sharp
            if ((tonicNoteNumerical == "05" && major) || (tonicNoteNumerical == "03" && !major))
            {
                numberAccidentals = 1;
                sharps = true;
            }
            // D major / B minor have 2 sharps
            else if ((tonicNoteNumerical == "02" && major) || (tonicNoteNumerical == "07" && !major))
            {
                numberAccidentals = 2;
                sharps = true;
            }
            // A major / F# minor have 3 sharps
            else if ((tonicNoteNumerical == "06" && major) || (tonicNoteNumerical == "14" && !major))
            {
                numberAccidentals = 3;
                sharps = true;
            }
            // E major / C# minor have 4 sharps
            else if ((tonicNoteNumerical == "03" && major) || (tonicNoteNumerical == "11" & !major))
            {
                numberAccidentals = 4;
                sharps = true;
            }
            // B major / G# minor have 5 sharps
            else if ((tonicNoteNumerical == "07" && major) || (tonicNoteNumerical == "15" & !major))
            {
                numberAccidentals = 5;
                sharps = true;
            }
            // Cb major has 7 flats
            else if (tonicNoteNumerical == "21" & major)
            {
                numberAccidentals = 7;
                sharps = false;
            }
            // Gb and F# major, and Eb and D# minor have 6 sharps
            else if ((tonicNoteNumerical == "25" && major) || (tonicNoteNumerical == "14" && major) || (tonicNoteNumerical == "23" && !major) || (tonicNoteNumerical == "12" && !major))
            {
                numberAccidentals = 6;
                sharps = true;
            }
            // C# major has 7 sharps
            else if (tonicNoteNumerical == "11" && major)
            {
                numberAccidentals = 7;
                sharps = true;
            }
            // Db major / Bb minor have 5 flats
            else if ((tonicNoteNumerical == "22" && major) || (tonicNoteNumerical == "27" && !major))
            {
                numberAccidentals = 5;
                sharps = false;
            }
            // Ab major / F minor have 4 flats
            else if ((tonicNoteNumerical == "26" && major) || (tonicNoteNumerical == "04" && !major))
            {
                numberAccidentals = 4;
                sharps = false;
            }
            // Eb major / C minor have 3 flats
            else if ((tonicNoteNumerical == "23" && major) || (tonicNoteNumerical == "01" && !major))
            {
                numberAccidentals = 3;
                sharps = false;
            }
            // Bb major / G minor have 2 flats
            else if ((tonicNoteNumerical == "27" && major) || (tonicNoteNumerical == "05" && !major))
            {
                numberAccidentals = 2;
                sharps = false;
            }
            // F major / D minor have 1 flat
            else if ((tonicNoteNumerical == "04" && major) || (tonicNoteNumerical == "02" && !major))
            {
                numberAccidentals = 1;
                sharps = false;
            }
            // C major / A minor have no sharps/flats
            // If the user chooses a note that's not supported, don't sharpen or flatten any notes for safety
            else
            {
                numberAccidentals = 0;
            }

            return numberAccidentals;
        }

        public static int GetTimeSignature()
        {
            return timeSignature;
        }

        private void Exit(ref string username)
        {
            Program.MainMenu(ref username);
        }

        private void ViewClip(ref string username, ref int timeSignature)
        {
            // Check if clip is empty
            // If it is, show error
            if (clips.Count == 0)
            {
                DisplayArrangement(ref username, 1);
            }
            // If not, display clips normally
            else
            {
                clips[selectedClipIndex].DisplayClip(ref timeSignature);
                DisplayArrangement(ref username, 0);
            }
        }

        public void LoadArrangement(ref int IDtoSelect, ref string username)
        {
            ID = IDtoSelect;
            // First, retrieve arrangement data from database
            MySqlCommand select = Program.SqlConnection.CreateCommand();
            select.CommandText = "select ArrangementName, ArrangementData, ClipContentsData from Arrangements where PK_ArrangementID = " + IDtoSelect;
            // Store arrangement data and clip contents data in strings
            string arrangementName = "";
            string arrangementData = "";
            string clipContentsData = "";
            // Execute statement
            Program.OpenConnection();
            MySqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                arrangementName = reader.GetString(0);
                arrangementData = reader.GetString(1);
                try
                {
                    clipContentsData = reader.GetString(2);
                }
                catch
                {
                    // Do nothing if field is null
                }
            }
            Program.SqlConnection.Close();

            // Name the arrangement after the name in the record
            name = arrangementName;

            // This section makes reference to the encoding. For details on how the encoding works, see Section 5.2.2 of the Design

            // Decode the data, starting with the arrangement data
            // First, the tonic note
            // The first character in the data should be a 0 (at index 0)
            tonicNoteNumerical += arrangementData[1];
            tonicNoteNumerical += arrangementData[2];
            // This should now give the first two digits of the key tonic note
            // The next character should be a 1 (index 3), followed by the key (index 4)
            // If the character for the key is a C, the key is major. If it is a D, the key is minor
            if (arrangementData[4] == 'C')
            {
                major = true;
            }
            else
            {
                major = false;
            }
            // The next character should be a 2 (index 5) followed by the time signature (index 6)
            // As extraction from array gives a char ASCII value, we subtract it to give the correct time signature
            timeSignature = Convert.ToInt32(arrangementData[6]) - 48;

            // After the basic details, we now need to extract the clips. We must start at index 7
            int index = 7;
            string newKey = "";
            try
            {
                do
                {
                    // If there is an A in the string, this is the start of a clip that's been saved, followed by its compound primary key in the ClipUses table
                    if (arrangementData[index] == 'A')
                    {
                        // Try catch block in case the end of the string is reached
                        try
                        {
                            do
                            {

                                index++;
                                // If the loop has found the start of a new clip, then exit and start on the next clip
                                if (arrangementData[index] == 'A' || arrangementData[index] == 'B')
                                {
                                    // Add this new clip using the key, to look up in the database
                                    Clip newClip = new Clip();
                                    newClip.LoadClipWithKey(ref newKey);
                                    clips.Add(newClip);
                                    break;
                                }
                                // Otherwise, add the key number to the new key variable
                                else
                                {
                                    newKey += arrangementData[index];
                                }
                                // Make sure there is no overflow
                            } while (index < arrangementData.Length);
                        }
                        catch
                        {
                            Clip newClip = new Clip();
                            newClip.LoadClipWithKey(ref newKey);
                            clips.Add(newClip);
                        }
                    }
                    // If it is a B, then this is the start of a clip that's not been saved, followed by the reference code to the clip data in the clip contents data variable
                    else
                    {
                        if (arrangementData[index] != 'E')
                        {
                            index++;
                        }
                        else
                        {
                            // The next character after the B should be an E
                            string id = "";
                            // After the E, there should be the ID for the unsaved clip
                            do
                            {
                                id += arrangementData[index];
                                // If F has been reached, then it is the end of that clip
                                if (arrangementData[index] == 'F')
                                {
                                    // Search for the clip details within the clip contents data variable
                                    SearchUnsavedClip(id, clipContentsData);
                                    break;
                                }
                                index++;
                            } while (true);
                            index++;
                        }
                    }
                } while (index < arrangementData.Length - 1);
            }
            catch
            {
                // Do nothing in case there is no data
            }
            DisplayArrangement(ref username, 0);
        }

        private void SearchUnsavedClip(string id, string clipContentsData)
        {
            int i;
            string key = "";
            bool findingKey = false;
            // Loop through the data to find the right clip
            for (i = 0; i < clipContentsData.Length; i++)
            {
                if (findingKey)
                {
                    key += clipContentsData[i];
                    // If we have reached the end of one key
                    if (clipContentsData[i] == 'F')
                    {
                        findingKey = false;
                        if (key == id)
                        {
                            // If we have found the right key, we exit the loop
                            break;
                        }
                        // Otherwise, we have to start again
                        else
                        {
                            key = "";
                        }
                    }
                }
                else
                {
                    if (clipContentsData[i] == 'E')
                    {
                        key += clipContentsData[i];
                        findingKey = true;
                    }
                }
            }

            // Once the key has been found, extract the data
            i++;
            string data = "";
            do
            {
                try
                {
                    // Keep adding data until another E is reached or if the end of the array is reached
                    if (clipContentsData[i] != 'E')
                    {
                        data += clipContentsData[i];
                        i++;
                    }
                    // Otherwise, send the new clip to be added
                    else
                    {
                        Clip newClip = new Clip();
                        newClip.LoadClipWithData(ref data, true);
                        clips.Add(newClip);
                        break;
                    }
                }
                // If the end of the array has been reached then there will be an index out of bounds error, hence the try catch block
                catch
                {
                    Clip newClip = new Clip();
                    newClip.LoadClipWithData(ref data, true);
                    clips.Add(newClip);
                    break;
                }
            } while (true);
        }

        private void SaveClip(ref string username)
        {
            //if (saved)
            //{
            //    // Insert data into Clips table in database
            //    clips[selectedClipIndex].SaveClip();
            //    // Now insert data into the ClipUses table
            //    InsertClipUses();
            //    DisplayArrangement(ref username, 0);
            //}
            //else
            //{
            //    // If they haven't saved, don't save clip and display error message
            //    DisplayArrangement(ref username, 2);
            //}
            
            // Check if clip is already saved
            try
            {
                if (clips[selectedClipIndex].IsSaved())
                {
                    DisplayArrangement(ref username, 2);
                }
                // Clip cannot be saved again
                else
                {
                    clips[selectedClipIndex].SaveClip();
                    InsertClipUses();
                    DisplayArrangement(ref username, 0);
                }
            }
            catch
            {
                DisplayArrangement(ref username, 3);
            }
        }

        private void InsertClipUses()
        {
            int clipID = clips[selectedClipIndex].GetID();
            MySqlCommand usesCommand = Program.SqlConnection.CreateCommand();
            usesCommand.CommandText = "insert into ClipUses (FK_ArrangementID, FK_ClipID) values (" + ID + ", " + clipID + ")";
            Program.OpenConnection();
            usesCommand.ExecuteNonQuery();
            Program.SqlConnection.Close();
        }

        // NOT FINISHED
        private void LoadClip(ref string username, bool mode)
        {
            #region Retrieving data
            MySqlCommand command = Program.SqlConnection.CreateCommand();
            command.CommandText = "select PK_ClipID, ClipName from Clips where FK_AccountID = " + Program.CurrentUserID;
            MySqlDataReader reader;
            // Borrowing the ArrangementRow struct as used in program
            List<Program.ArrangementRow> list = new List<Program.ArrangementRow>();
            Program.ArrangementRow newRow;
            // Read list of arrangements from table
            Program.OpenConnection();
            reader = command.ExecuteReader();
            // Loop until all arrangement names have been retrieved and store each row in the list
            while (reader.Read())
            {
                newRow.ID = reader.GetInt32(0);
                newRow.Name = reader.GetString(1);
                list.Add(newRow);
            }
            Program.SqlConnection.Close();
            #endregion

            List<Program.Pointer> pointers = new List<Program.Pointer>();
            Program.Pointer newPointer;
            Console.Clear();
            Program.WriteLineCyan("                                  Load Clip\n");
            if (list.Count == 0)
            {
                Program.WriteLineRed("\nThere are no clips saved to this account.\nPress any key to return to the arranger.");
                Program.WriteYellow("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nLogged in as " + username + " ");
                Console.ReadKey();
                DisplayArrangement(ref username, 0);
            }
            else
            {
                // Print each of the arrangement names in the list
                for (int i = 0; i < list.Count(); i++)
                {
                    // Must be +1 to prevent 0-indexing
                    newPointer.ListItem = i + 1;
                    newPointer.ID = list[i].ID;
                    pointers.Add(newPointer);
                    Program.WriteLineGreen((i + 1) + ". " + list[i].Name);
                }
                int choice;
                try
                {
                    if (mode)
                    {
                        Program.WriteCyan("\nInvalid choice. Please try again: ");
                    }
                    else
                    {
                        Program.WriteCyan("\nPlease enter a clip number to load: ");
                    }
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
                    // If the choice is invalid, prompt to choose the clip again
                    if (!flag)
                    {
                        LoadClip(ref username, true);
                    }
                    // If the choice is valid, create a new clip object and load the data from the database
                    else
                    {
                        Clip newClip = new Clip();
                        newClip.LoadClipWithSingleKey(ref id);
                        clips.Add(newClip);
                        DisplayArrangement(ref username, 0);
                    }
                }
                catch
                {
                    // If the user makes an invalid input, run the function again but with showing an error
                    LoadClip(ref username, true);
                }
            }
        }

        private void DeleteClip(ref string username)
        {
            // Delete clip using index
            clips.RemoveAt(selectedClipIndex);
            DisplayArrangement(ref username, 0);
        }

        public void CreateArrangement()
        {
            // Will store encoded arrangement data
            // Encodes main arrangement data first
            string encoded = "0" + tonicNoteNumerical + "1";
            // If the key is major, then a C is added. If it is minor, then a D is added
            if (major)
            {
                encoded += "C";
            }
            else
            {
                encoded += "D";
            }
            encoded += "2" + timeSignature;
            // Create statement to insert data
            MySqlCommand insertCommand = Program.SqlConnection.CreateCommand();
            // Create statement to get the ID of the arrangement
            MySqlCommand getID = Program.SqlConnection.CreateCommand();
            MySqlDataReader IDreader;
            insertCommand.CommandText = "insert into Arrangements (ArrangementName, ArrangementData, FK_AccountID) values (\"" + name + "\", \"" + encoded + "\", " + Program.CurrentUserID + ")";
            getID.CommandText = "select LAST_INSERT_ID()";
            // Insert data
            Program.OpenConnection();
            insertCommand.ExecuteNonQuery();
            IDreader = getID.ExecuteReader();
            while (IDreader.Read())
            {
                ID = IDreader.GetInt32(0);
            }
            Program.SqlConnection.Close();
        }

        // WRITE PROPER COMMENTS
        /// <summary>
        /// 
        /// </summary>
        public void SaveArrangement(ref string username)
        {
            #region Old code that wasn't able to save updated versions
            //// We retrieve the current arrangement data in the database
            //MySqlCommand currentDataCommand = Program.SqlConnection.CreateCommand();
            //currentDataCommand.CommandText = "select ArrangementData from Arrangements where PK_ArrangementID = " + ID;
            //// Store data in string
            //string currentData = "";
            //MySqlDataReader currentDataReader;
            //Program.OpenConnection();
            //currentDataReader = currentDataCommand.ExecuteReader();
            //while (currentDataReader.Read())
            //{
            //    currentData = currentDataReader.GetString(0);
            //}
            //Program.SqlConnection.Close();

            //// Now, we need to cycle through each of the clips contained
            //int unsavedCounter = 0;
            //int clipID;

            //// Encode clip data and insert into the ClipContentsData field with the new key
            //MySqlCommand getClipContentsData = Program.SqlConnection.CreateCommand();
            //// Get the current clip contents data first
            //getClipContentsData.CommandText = "select ClipContentsData from Arrangements where PK_ArrangementID = " + ID;
            //// Read from the database
            //MySqlDataReader clipContentsDataReader;
            //string currentClipContentsData = "";
            //Program.OpenConnection();
            //clipContentsDataReader = getClipContentsData.ExecuteReader();
            //while (clipContentsDataReader.Read())
            //{
            //    // There's a potential that there could be a null field for clip contents data
            //    try
            //    {
            //        currentClipContentsData = clipContentsDataReader.GetString(0);
            //    }
            //    catch
            //    {
            //        // If that's the case, do nothing
            //    }
            //}
            //Program.SqlConnection.Close();
            #endregion

            // Variabes to be inserted into database
            string arrangementData;
            string clipContentsData = "";
            
            arrangementData = "0" + tonicNoteNumerical + "1";
            if (major)
            {
                arrangementData += "C";
            }
            else
            {
                arrangementData += "D";
            }
            arrangementData += "2" + timeSignature;
            
            int clipID;

            foreach (Clip clip in clips)
            {
                // If the clip is saved, then we make a reference to it
                if (clip.IsSaved())
                {
                    //// Insert new record into ClipUses table
                    //MySqlCommand clipUse = Program.SqlConnection.CreateCommand();
                    //MySqlCommand insertIntoArrangement = Program.SqlConnection.CreateCommand();
                    clipID = clip.GetID();
                    //clipUse.CommandText = "insert into ClipUses (FK_ArrangementID, FK_ClipID) values (" + ID + ", " + clipID + ")";

                    // Compound ID formed of arrangement ID and clip ID together, which gets inserted into the clip data, preceded by an A, to show that a saved clip has been included
                    string compoundID = clipID.ToString() + ID.ToString();
                    arrangementData += "A" + compoundID;
                    //insertIntoArrangement.CommandText = "update Arrangements set ArrangementData = \"" + arrangementData + "\" where PK_ArrangementID = " + ID;

                    // Execute database operations
                    //Program.OpenConnection();
                    //clipUse.ExecuteNonQuery();
                    //insertIntoArrangement.ExecuteNonQuery();
                    //Program.SqlConnection.Close();
                }
                // If the clip is not saved, insert it into the ClipContentsData field
                else
                {
                    // Generates a random key
                    // If the key has already been implemented within the arrangement data, then generate a new one, and keep looping until you find a unique key
                    Random random = new Random();
                    int randomNumber;
                    string newKey;
                    bool flag = true;
                    do
                    {
                        randomNumber = random.Next();
                        newKey = "E" + randomNumber + "F";
                        if (!arrangementData.Contains(newKey))
                        {
                            flag = false;
                        }
                    } while (flag);
                        
                        
                    // Append the new data onto the existing data
                    arrangementData += "B" + newKey;
                    clip.EncodeClip();
                    clipContentsData += newKey + clip.GetEncoded();
                        
                    }
                }

            // Create new commands to update the data in the arrangement table
            MySqlCommand updateData = Program.SqlConnection.CreateCommand();
            updateData.CommandText = "update Arrangements set ArrangementData = \"" + arrangementData + "\", ClipContentsData = \"" + clipContentsData + "\" where PK_ArrangementID = " + ID;
            // Execute query
            Program.OpenConnection();
            updateData.ExecuteNonQuery();
            Program.SqlConnection.Close();

            // Display success message and display the arrangement again
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            Console.Clear();
            Program.WriteLineGreen("\n\n\n\n\n\n\n\n\n\n\n                              Arrangement saved");
            stopwatch.Start();
            // Wait for 1 second before showing the arranger
            while (stopwatch.ElapsedMilliseconds < 1000)
            {
                // Do nothing
            }
            stopwatch.Stop();
            DisplayArrangement(ref username, 0);
            }
        }
        #endregion
    }
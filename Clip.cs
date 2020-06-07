using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ProjectGracenoteAlpha1Point1
{
    class Clip
    {
        #region Properties

        private int rangeOfOctaves;
        private int rangeOfNotes;
        private int syncopationRandomisation;
        private List<Note> notes;
        private static short[] accidentals;
        private string name;
        private bool saved;
        private string encoded;
        private static double[] fractionNoteValues = { 1, 0.75, 0.5, 0.25, 0.125, 0.0625 };
        private int ID;

        // CHANGEABLE IN CONSTRUCTOR
        private static short maxNoteValue;
        private static short minNoteValue;

        //public bool IsIncluded;

        #endregion

        #region Methods

        // Constructor
        public Clip()
        {
            accidentals = new short[7];
            // Assign accidental values
            accidentals[0] = 4;
            accidentals[1] = 1;
            accidentals[2] = 5;
            accidentals[3] = 2;
            accidentals[4] = 6;
            accidentals[5] = 3;
            accidentals[6] = 7;
            saved = false;
            notes = new List<Note>();
            encoded = "";
            ID = 0;
            //IsIncluded = false;

            // ONLY CHANGE THESE
            minNoteValue = 0;
            maxNoteValue = 5;
        }

        // Access modifier
        public void SetDetails(ref int RangeOfOctaves, ref int RangeOfNotes, ref int SycopationRandomisation)
        {
            rangeOfOctaves = RangeOfOctaves;
            rangeOfNotes = RangeOfNotes;
            syncopationRandomisation = SycopationRandomisation;
        }

        public bool IsSaved()
        {
            return saved;
        }

        public string GetName()
        {
            return name;
        }

        // This is based on the note generating pseudocode, which can be found in the Design, Section 3.3.2
        public void CreateClip(ref bool sharps, ref bool major)
        {
            int[] octaveRangeAvailable = { 4 - rangeOfOctaves, 4 + rangeOfOctaves };
            int highestNoteAvailable;
            // Copy the tonic note (the main note without any accidentals) from the Arrangement class
            string mainTonicNote = Arrangement.tonicNoteNumerical[1].ToString();
            int TonicNoteNumerical = Convert.ToInt32(mainTonicNote);
            // If the tonic note is a C, make the highest note a B
            if (TonicNoteNumerical == 1)
            {
                highestNoteAvailable = 7;
            }
            // If it's not a C, make the highest note the note below the tonic note
            else
            {
                highestNoteAvailable = TonicNoteNumerical - 1;
            }
            int numberAccidentals = Arrangement.GetNumberAccidentals(ref sharps, ref major);
            Random randomGenerator = new Random();
            //int previousNoteValue = randomGenerator.Next(0, 5);
            int previousNoteValue = randomGenerator.Next(minNoteValue, maxNoteValue);
            int[] accidentalsToApply = new int[numberAccidentals];
            // If the key signature contains sharps, apply sharp accidentals
            if (sharps)
            {
                for (int i = 0; i < numberAccidentals; i++)
                {
                    accidentalsToApply[i] = accidentals[i];
                }
            }
            // Otherwise, if the key contains flats, apply flat accidentals
            else
            {
                short counter = 0;
                for (int i = 6; i >= 7 - numberAccidentals; i--)
                {
                    accidentalsToApply[counter] = accidentals[i];
                    counter++;
                }
            }
            // Calculate the space left for notes
            double spaceLeft = Arrangement.GetTimeSignature();
            double spaceToTakeAway;
            int pitch;
            int octave;
            int noteValue;
            string pitchString;
            Note newNote;
            do
            {
                pitch = randomGenerator.Next(1, 7);
                octave = randomGenerator.Next(1, 10);
                // If the syncopation randomiser decides that the note value needs changing
                if (syncopationRandomiser())
                {
                    // Change the note value
                    //noteValue = randomGenerator.Next(0, 5);
                    noteValue = randomGenerator.Next(minNoteValue, maxNoteValue);
                }
                // If it decides not
                else
                {
                    // Use the note value of the previous note
                    noteValue = previousNoteValue;
                }
                if (spaceLeft >= fractionNoteValues[noteValue])
                {
                    if ((octave >= octaveRangeAvailable[0]) && (octave <= octaveRangeAvailable[1]))
                    {
                        //if ((pitch >= TonicNoteNumerical) && (pitch <= highestNoteAvailable))
                        //{
                            if (accidentalsToApply.Contains(pitch))
                            {
                                if (sharps)
                                {
                                    pitch = pitch + 10;
                                }
                                else
                                {
                                    pitch = pitch + 20;
                                }

                            }
                            // Once note's properties are decided, add it to the notes list    
                            // If the note is natural, then a 0 needs to be added to the end
                            if (pitch - 10 < 0)
                            {
                                pitchString = "0" + pitch.ToString();
                            }
                            else
                            {
                                pitchString = pitch.ToString();
                            }    
                            previousNoteValue = noteValue;
                            newNote = new Note();
                            newNote.SetDetails(pitchString, noteValue, octave);
                            notes.Add(newNote);    
                            // Reduce amount of space left so that new notes can be made
                            spaceToTakeAway = fractionNoteValues[noteValue];
                            spaceLeft = spaceLeft - spaceToTakeAway;
                        //}
                        //else
                        //{
                        //    break;
                        //}
                    }
                //    else
                //    {
                //        break;
                //    }
                }
                //else
                //{
                //    break;
                //}
            } while (spaceLeft > 0); 
        }

        private bool syncopationRandomiser()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, syncopationRandomisation);
            if (randomNumber == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Leads to EncodeClip and SendToDatabase
        /// </summary>
        public void SaveClip()
        {
            // User prompted to enter clip name
            bool flag = false;
            string text = "                       Enter a name for this clip:";
            do
            {
                Console.Clear();
                Program.WriteLineCyan("                                Save Clip\n\n\n\n\n\n\n\n\n\n");
                Program.WriteLineCyan(text+"\n");
                Console.Write("           ");
                string nameInput = Console.ReadLine();
                if (nameInput == "")
                {
                    text = "                     Please don't enter a blank name:";
                }
                else
                {
                    flag = true;
                    name = nameInput;
                    saved = true;
                }
            } while (!flag);

            // Once name is entered properly, encode the clip ready for the database
            EncodeClip();

            // Mark the clip as saved
            saved = true;

            // Send to database
            SendToDatabase();
        }

        public void EncodeClip()
        {
            // Loops through each note in the list in order to encode it
            foreach (Note note in notes)
            {
                // Mark start of note with X
                encoded += "X";
                // Mark note value with A
                encoded += "A";
                // Insert note's note value code
                encoded += note.GetNoteValue();
                // Mark note pitch with B
                encoded += "B";
                // Insert note's pitch code
                encoded += note.GetPitchNumerical();
                // Mark note octave with C
                encoded += "C";
                // Insert note's octave
                encoded += note.GetOctave();
                // Mark end of note with Y
                encoded += "Y";
            }
        }

        public string GetEncoded()
        {
            return encoded;
        }

        public void DisplayClip(ref int timeSignature)
        {
            // Check which is the highest and lowest octaves within the notes list
            // Initially set these variables to the values of the first note. They will be changed later
            int highestOctave = notes[0].GetOctave();
            int lowestOctave = notes[0].GetOctave();
            int currentOctave;

            // Loop through all of the other clips
            for (int i = 1; i < notes.Count; i++)
            {
                currentOctave = notes[i].GetOctave();
                // Compare octave of current note with highest and lowest octaves and set values accordingly
                if (currentOctave > highestOctave)
                {
                    highestOctave = currentOctave;
                }
                else if (currentOctave < lowestOctave)
                {
                    lowestOctave = currentOctave;
                }
            }

            Octave octave = new Octave();
            string toDisplay = octave.GetNoteDisplay(ref highestOctave, ref lowestOctave, ref notes);

            Console.Clear();
            if (this.name == null)
            {
                Program.WriteLineCyan("UNSAVED CLIP");
            }
            else
            {
                Program.WriteLineCyan(this.name);
            }
            Console.WriteLine();
            Console.Write("       ");
            // Print bar numbers
            for (int i = 1; i <= timeSignature; i++)
            {
                Program.WriteGrey("<------" + i + "------->");
            }
            Console.WriteLine();
            foreach (char i in toDisplay)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                switch (i)
                {
                    // If the note is natural, display as blue
                    case '#':
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write(" ");
                        break;
                    // If the note is sharp, display as green
                    case '/':
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write(" ");
                        break;
                    // If the note is flat, display as red
                    case '*':
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(" ");
                        break;
                    // Otherwise, print whatever is there
                    default:
                        Console.Write(i);
                        break;
                }
            }
            Program.WriteCyan("\nNatural notes are ");
            Program.WriteBlue("blue");
            Program.WriteCyan(", sharp notes are ");
            Program.WriteGreen("green");
            Program.WriteCyan(" and flat notes are ");
            Program.WriteRed("red");
            Program.WriteCyan(".\n");
            Program.WriteMagenta("\nPress any key to return to the arranger ");
            Console.ReadKey();
        }

        public void LoadClipWithData(ref string data, bool mode)
        {
            // If the clip is an unsaved one, we mark the clip as unsaved
            if (mode)
            {
                saved = false;
            }
            // Then, we continue with extracting the data
            // Flags to indicate if each part of the note is found
            bool startFound = false;
            bool noteValueFound = false;
            bool pitchFound = false;
            bool octaveFound = false;
            // Properties for the new note
            int noteValue = new int();
            string pitch = "";
            string octaveString = "";
            int octave = new int();
            // Searches for 
            for (int i = 0; i <= data.Length; i++)
            {
                // We should consider each possible combination

                /// NEW NOTE ENCODING - ignore those in the Design section
                /// X - start of note
                /// A - note value
                /// B - pitch
                /// C - octave
                /// Y - end of note

                // If the start has not been found
                if (!startFound)
                {
                    // If the character is an X, then the start has been found
                    if (data[i] == 'X')
                    {
                        startFound = true;
                    }
                }
                // If the start has been found but not the note value
                else if (startFound && !noteValueFound)
                {
                    // If we have found an A, then we have found the note value
                    if (data[i] == 'A')
                    {
                        // We move to the next index, which should be a single integer representing the note value code
                        i++;
                        // Once again, we need to convert out of the ASCII code into the actual note value code
                        noteValue = data[i] - 48;
                        // We now mark the note value as found
                        noteValueFound = true;
                    }
                }
                // If the start and note value have been found but not the pitch
                else if (startFound && noteValueFound && !pitchFound)
                {
                    // If we have found an B, then we have the two-digit pitch
                    if (data[i] == 'B')
                    {
                        // We move forward to find the two digits of the pitch code
                        i++;
                        pitch += data[i];
                        i++;
                        pitch += data[i];
                        // We now mark the pitch as found
                        pitchFound = true;
                    }
                }
                // If the start, note value and pitch have been found but not the octave
                else if (startFound && noteValueFound && pitchFound && !octaveFound)
                {
                    // If we have found a C, then we have found the octave
                    if (data[i] == 'C')
                    {
                        // Unlike the other properties, the octave can be any length, so we need to find F
                        do
                        {
                            i++;
                            if (data[i] == 'Y')
                            {
                                // If we have found the Y, then stop working
                                break;
                            }
                            else
                            {
                                // Otherwise, start to store the octave value, first as a string
                                octaveString += data[i];
                            }
                        } while (true);
                        // Then convert value to an integer
                        octave = Convert.ToInt32(octaveString);
                        // Flag that we have found the octave
                        octaveFound = true;
                    }
                }
                // If we have reached the end of the note, then set all flags to false
                else
                {
                    startFound = false;
                    noteValueFound = false;
                    pitchFound = false;
                    octaveFound = false;
                    // Create new note and add to the note list
                    Note newNote = new Note();
                    newNote.SetDetails(pitch, noteValue, octave);
                    notes.Add(newNote);
                    // Mark properties in this function as empty
                    noteValue = new int();
                    pitch = "";
                    octaveString = "";
                    octave = new int();
                    if (i < data.Length)
                    {
                        i--;
                    }
                }
            }
        }

        public void LoadClip(ref int selectedID)
        {
            do
            {
                // Retrieve clip data from 
                MySqlCommand select = Program.SqlConnection.CreateCommand();
                ID = selectedID;
                select.CommandText = "select ClipName, NoteData from Clips where PK_ClipID = " + ID;
                MySqlDataReader reader;
                string noteData = "";
                Program.OpenConnection();
                reader = select.ExecuteReader();
                while (reader.Read())
                {
                    name = reader.GetString(0);
                    noteData = reader.GetString(1);
                }
                Program.SqlConnection.Close();
                LoadClipWithData(ref noteData, false);
            } while (true);
        }

        public void LoadClipWithKey(ref string key)
        {
            saved = true;
            // Check which field to load data from by checking primary key
            MySqlCommand command = Program.SqlConnection.CreateCommand();
            command.CommandText = "select FK_ClipID from ClipUses where concat(FK_ClipID, FK_ArrangementID) = " + key;
            MySqlDataReader clipIDReader;
            int clipID = new int();
            Program.OpenConnection();
            clipIDReader = command.ExecuteReader();
            while (clipIDReader.Read())
            {
                clipID = clipIDReader.GetInt32(0);
            }
            Program.SqlConnection.Close();

            // Now load the clip name and data from the clips table, using the key
            string clipName = "";
            string noteData = "";
            MySqlCommand getData = Program.SqlConnection.CreateCommand();
            getData.CommandText = "select ClipName, NoteData from Clips where PK_ClipID = " + clipID;
            MySqlDataReader dataReader;
            Program.OpenConnection();
            dataReader = getData.ExecuteReader();
            while (dataReader.Read())
            {
                clipName = dataReader.GetString(0);
                noteData = dataReader.GetString(1);
            }

            // Set the name of the clip
            name = clipName;

            // Send data to be decoded in other function
            LoadClipWithData(ref noteData, false);
        }

        public void LoadClipWithSingleKey(ref int clipID)
        {
            // Now load the clip name and data from the clips table, using the key
            string clipName = "";
            string noteData = "";
            MySqlCommand getData = Program.SqlConnection.CreateCommand();
            getData.CommandText = "select ClipName, NoteData from Clips where PK_ClipID = " + clipID;
            MySqlDataReader dataReader;
            Program.OpenConnection();
            dataReader = getData.ExecuteReader();
            while (dataReader.Read())
            {
                clipName = dataReader.GetString(0);
                noteData = dataReader.GetString(1);
            }

            // Set the name of the clip
            name = clipName;

            // Send data to be decoded in other function
            LoadClipWithData(ref noteData, false);

            saved = true;
        }

        private void SendToDatabase()
        {
            // When clip is not already saved, insert into new record
            // Otherwise, update record
            MySqlCommand insertClips = Program.SqlConnection.CreateCommand();
            insertClips.CommandText = "insert into Clips (ClipName, NoteData, FK_AccountID) values (\"" + name + "\", \"" + encoded + "\", " + Program.CurrentUserID + ")";
            if (ID == 0)
            {
                // Get the PK_ClipID of the clip once it's been inserted
                MySqlCommand getID = Program.SqlConnection.CreateCommand();
                MySqlDataReader IDreader;
                getID.CommandText = "select LAST_INSERT_ID()";
                Program.OpenConnection();
                insertClips.ExecuteNonQuery();
                IDreader = getID.ExecuteReader();
                while (IDreader.Read())
                {
                    ID = IDreader.GetInt32(0);
                }
                Program.SqlConnection.Close();
            }
            else
            {
                insertClips.CommandText = "update Clips set NoteData = \"" + encoded + "\" where PK_ClipID = " + ID;
                Program.OpenConnection();
                insertClips.ExecuteNonQuery();
                Program.SqlConnection.Close();
            }
        }

        public int GetID()
        {
            return ID;
        }

        #endregion
    }
}

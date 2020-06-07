using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGracenoteAlpha1Point1
{
    class Octave
    {
        private struct OctaveOfNotes
        {
            // Contains holders for notes of one octave
            public string[] Notes;
        }

        // Constructor
        public Octave() { }

        private struct Pointer
        {
            public int Index;
            public int Octave;
        }

        // Method to generate notes to display back to Clip class
        public string GetNoteDisplay(ref int highestOctave, ref int lowestOctave, ref List<Note> notes)
        {
            #region Pre-printing
            string toDisplay = "";

            // List to hold all of the octaves required
            List<OctaveOfNotes> listOfOctaves = new List<OctaveOfNotes>();

            int numberOctaves = (highestOctave - lowestOctave) + 1;

            // Generate correct number of octaves
            for (int i = 0; i < numberOctaves; i++)
            {
                OctaveOfNotes newOctave = new OctaveOfNotes();
                newOctave.Notes = new string[7];
                listOfOctaves.Add(newOctave);
            }

            // Assign the correct octave numbers to each octave created
            int count = 0;
            Pointer[] pointers = new Pointer[numberOctaves];
            // Octaves will be listed in descending order
            for (int i = highestOctave; i >= lowestOctave; i--)
            {
                // Give each note row its corresponding note
                listOfOctaves[count].Notes[0] = "G" + i + "     ";
                listOfOctaves[count].Notes[1] = "F" + i + "     ";
                listOfOctaves[count].Notes[2] = "E" + i + "     ";
                listOfOctaves[count].Notes[3] = "D" + i + "     ";
                listOfOctaves[count].Notes[4] = "C" + i + "     ";
                listOfOctaves[count].Notes[5] = "B" + i + "     ";
                listOfOctaves[count].Notes[6] = "A" + i + "     ";

                // Give a pointer to this octave
                pointers[count].Index = count;
                pointers[count].Octave = i;

                // Increment counter for next octave
                count++;
            }
            #endregion

            #region Assigning each note
            // Loop through each of the notes and assign them to the correct octave
            foreach (Note currentNote in notes)
            {
                // Variables used for assigning the notes
                int octaveListIndex = new int();
                int noteInOctave = new int();
                int numberSpacesToUse = new int();

                #region Find octave index
                int octave = currentNote.GetOctave();
                // Find which octave array it needs to be assigned to in the list          
                for (int i = 0; i < pointers.Length; i++)
                {
                    if (pointers[i].Octave == octave)
                    {
                        octaveListIndex = pointers[i].Index;
                    }
                }
                #endregion

                #region Find pitch index
                // Find which pitch within the note needs to be assigned to
                string pitch = currentNote.GetPitchNumerical();
                // Extract accidental and main pitch
                string accidental = pitch[0].ToString();
                // Change accidental code to #, / and * to avoid confusion with octave numbers
                switch (accidental)
                {
                    case "0":
                        accidental = "#";
                        break;
                    case "1":
                        accidental = "/";
                        break;
                    case "2":
                        accidental = "*";
                        break;
                }
                string mainNote = pitch[1].ToString();
                // Choosing which index in one of the octaves needs to be assigned to
                switch (mainNote)
                {
                    // If the note is a C
                    case "1":
                        // Cs are stored in index 4 in each of the octaves
                        noteInOctave = 4;
                        break;
                    // If the note is a D
                    case "2":
                        // Ds are stored in index 3 in each of the octaves
                        noteInOctave = 3;
                        break;
                    // If the note is an E
                    case "3":
                        noteInOctave = 2;
                        break;
                    // If F
                    case "4":
                        noteInOctave = 1;
                        break;
                    // If G
                    case "5":
                        noteInOctave = 0;
                        break;
                    // If A
                    case "6":
                        noteInOctave = 6;
                        break;
                    // If B
                    case "7":
                        noteInOctave = 5;
                        break;
                }
                #endregion

                #region Find number of spaces
                // Decide how many characters to print based on how long the note value is
                int noteValue = currentNote.GetNoteValue();
                // Extract note value based on its corresponding code
                switch (noteValue)
                {
                    // If the note is 1 beat
                    case 0:
                        // A note that is 1 beat will take up 16 spaces
                        numberSpacesToUse = 16;
                        break;
                    // If the note is 3/4 beat, it will take up 12 spaces
                    case 1:
                        numberSpacesToUse = 12;
                        break;
                    // If the note is 1/2 beat, it will take up 8 spaces
                    case 2:
                        numberSpacesToUse = 8;
                        break;
                    // 1/4 beat
                    case 3:
                        numberSpacesToUse = 4;
                        break;
                    // 1/8 beat
                    case 4:
                        numberSpacesToUse = 2;
                        break;
                    // 1/16 beat
                    case 5:
                        numberSpacesToUse = 1;
                        break;
                }
                #endregion

                #region Assigning characters
                // Print out accidental code for number of spaces needed
                for (int i = 0; i < numberSpacesToUse; i++)
                {
                    // For each row where our note is contained, its accidental code (#, / or *) will be assigned
                    // For each row where our note is not contained, spaces will be added
                    for (int j = 0; j < listOfOctaves.Count; j++)
                    {
                        // Find index where the note is located
                        if (j == octaveListIndex)
                        {
                            for (int k = 0; k < 7; k++)
                            {
                                if (k == noteInOctave)
                                {
                                    listOfOctaves[j].Notes[k] += accidental;
                                }
                                else
                                {
                                    listOfOctaves[j].Notes[k] += " ";
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < 7; k++)
                            {
                                listOfOctaves[j].Notes[k] += " ";
                            }
                        }
                    }
                }
                #endregion


            }
            #endregion

            #region Printing
            // Loop through each of the rows and add to toDisplay
            foreach (OctaveOfNotes currentOctave in listOfOctaves)
            {
                for (int i = 0; i < 7; i++)
                {
                    // Inserts each row separated by line
                    toDisplay += currentOctave.Notes[i] + "\n";
                }
            }
            #endregion

            return toDisplay;
        }     
    }
}

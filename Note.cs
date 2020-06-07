using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGracenoteAlpha1Point1
{
    class Note
    {
        #region Properties

        private string pitchNumerical;
        private int noteValue;
        private int octave;

        #endregion

        #region Methods

        // Constructor
        public Note() { }
        
        // Access modifier
        public void SetDetails(string Pitch, int NoteValue, int Octave)
        {
            pitchNumerical = Pitch;
            noteValue = NoteValue;
            octave = Octave;
        }

        public string GetPitchNumerical()
        {
            return pitchNumerical;
        }

        public int GetNoteValue()
        {
            return noteValue;
        }

        public int GetOctave()
        {
            return octave;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ProjectGracenoteAlpha1Point1
{
    class NewAccount
    {
        #region Properties

        private string username;
        private string password;

        #endregion

        #region Methods

        #region Finished

        // Access modifier
        public void SetDetails(ref string Username, ref string Password)
        {
            username = Username;
            password = Password;
        }

        public bool SaveDetails()
        {
            bool flag = true;
            // Using connection in Program, send to database
            MySqlCommand sender = new MySqlCommand("insert into Accounts (Username, Password) values (\"" + username + "\", \"" + password + "\")", Program.SqlConnection);
            // If there are any problems with the connection, show error (error is shown in Program)
            try
            {
                Program.SqlConnection.Open();
                sender.ExecuteNonQuery();
                Program.SqlConnection.Close();
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        #endregion

        #endregion
    }
}

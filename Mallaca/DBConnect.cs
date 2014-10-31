using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mallaca.Usertypes;
using MySql.Data.MySqlClient;
using System.Globalization;
using Newtonsoft.Json.Linq;
namespace Mallaca
{
    public struct SessionData
    {
        public int userId;
        public int sessionId;
        public DateTime date;

        public SessionData(int uId, int sId, DateTime d)
        {
            userId = uId;
            sessionId = sId;
            date = d;
        }

        public override string ToString()
        {
            return String.Format("Session {0}, {1}", sessionId, date.ToShortDateString() + " " + date.ToShortTimeString());
        }
    }
}

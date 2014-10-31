using System;

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

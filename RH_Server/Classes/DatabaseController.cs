using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using Mallaca;
using Mallaca.Usertypes;
using RH_Server.Database;
using User = RH_Server.Database.User;
using EntityUser = Mallaca.Usertypes.User;
using Measurement = RH_Server.Database.Measurement;
using EntityMeasurement = Mallaca.Measurement;

namespace RH_Server.Classes
{
    public class DatabaseController
    {
        private readonly DataClassesDataContext _dataContext;

        public DatabaseController()
        {
            _dataContext = new DataClassesDataContext();
        }

        public EntityUser GetUser(int userId)
        {
            return _dataContext.Users.Where(user => user.Id == userId)
                .Select(user => DbUserToEntityUser(user))
                .FirstOrDefault();
        }

        public EntityUser GetUser(String username)
        {
            return _dataContext.Users.Where(user => user.Username == username)
                .Select(user => DbUserToEntityUser(user))
                .FirstOrDefault();
        }

        public List<EntityUser> GetAllUsers()
        {
            return _dataContext.Users.Select(user => DbUserToEntityUser(user)).ToList();
        }

        public List<EntityMeasurement> GetMeasurementsByUser(int userId, int sessionId)
        {
            var mByUserId = _dataContext.Measurements.Where(measurement => measurement.User_ID == userId);

            var measurementByUser =
                from u in _dataContext.Users
                join m in mByUserId
                    on u.Id equals m.User_ID into eGroup
                from z in eGroup.DefaultIfEmpty()
                where z.User_ID == userId && z.Session_ID == sessionId
                select z;

            return measurementByUser.ToList().Select(DbMeasurementToMeasurement).ToList();
        }

        public List<EntityMeasurement> GetMeasurementsByUser(String username, int sessionId)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.Username == username);
            return user == null ? new List<EntityMeasurement>() : GetMeasurementsByUser(user.Id, sessionId);
        }

        public List<EntityMeasurement> GetAllMeasurements()
        {
            return _dataContext.Measurements.Select(m => DbMeasurementToMeasurement(m)).ToList();
        }

        public List<SessionData> GetTrainingSessions()
        {
            var x = _dataContext.Measurements.GroupBy(m => new
            {
                m.Session_ID, m.User_ID,
                Datetime = m.Datatime
            }).Select(z => new SessionData(z.Key.User_ID, z.Key.Session_ID, z.Key.Datetime));
            return x.ToList();
        }

        public Tuple<bool, UserType> ValidateUser(String username, String password, bool passIsHashedWithSha256 = true)
        {
            if (!passIsHashedWithSha256)
                password = Hashing.CreateSHA256(password);
            var users = _dataContext.Users.Where(user => user.Username == username && user.Password == password).ToList();

            if (!users.Any())
                return new Tuple<bool, UserType>(false, UserType.None);

            var usertypeInt = users.First().Usertype;
            
            var usertype = (UserType)usertypeInt;
            return new Tuple<bool, UserType>(true, usertype);
        }

        public List<Specialist> GetClientsBySpecialists()
        {
            var grp = from users in _dataContext.SpecialistHasClients
                group users by users.SpecialistID;

            var list = new List<Specialist>();

            foreach (var item in grp)
            {
                var specId = item.Key;
                var specialist = DbUserToEntityUser(
                    _dataContext.Users.FirstOrDefault(u => u.Id == specId)) as Specialist;
                if (specialist == null)
                    continue;
                foreach (var client in item.Select(specialistHasClient => DbUserToEntityUser
                    (_dataContext.Users.FirstOrDefault(u => u.Id == specialistHasClient.ClientID)) as Client)
                    .Where(client => client != null))
                {
                    specialist.Clients.Add(client);
                }
                list.Add(specialist);
            }

            return list;
        }

        public int GetNewTrainingSessionId(int userId)
        {
            //"SELECT COUNT(DISTINCT session_id) FROM {0}.measurement WHERE user_id = {1}"
            var distinctCount = _dataContext.Measurements
                .Where(measurement => measurement.User_ID == userId)
                .GroupBy(m => m.User_ID)
                .Select(m => m.First())
                .Count();

            return distinctCount;
        }

        public bool SaveMeasurement(IEnumerable<EntityMeasurement> m, int sessionId, int userId)
        {
            foreach (var measurement in m)
            {
                _dataContext.Measurements.InsertOnSubmit(
                    MeasurementToDbMeasurement(measurement, sessionId, userId)
                    );
            }

            try
            {
                _dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveMeasurement(EntityMeasurement m, int sessionId, int userId)
        {
            _dataContext.Measurements.InsertOnSubmit(MeasurementToDbMeasurement(m, sessionId, userId));

            try
            {
                _dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveUser(EntityUser u)
        {
            return false;
        }
        private static EntityUser DbUserToEntityUser(User user)
        {
            var usertype = (UserType) user.Usertype;
            switch (usertype)
            {
                case UserType.User:
                    return new EntityUser(user.Id, user.Username, user.Password, user.Firstname,
                            user.DateOfBirth, user.Lastname, 
                            user.Gender.ToString(CultureInfo.InvariantCulture));
                case UserType.Client:
                    return new Client(user.Id, user.Username, user.Password, user.Firstname,
                            user.DateOfBirth, user.Lastname, 
                            user.Gender.ToString(CultureInfo.InvariantCulture), user.Usertype);
                case UserType.Specialist:
                    return new Specialist(user.Id, user.Username, user.Password, user.Firstname,
                            user.DateOfBirth, user.Lastname, 
                            user.Gender.ToString(CultureInfo.InvariantCulture), user.Usertype);
            }
            return null;
        }

        private static User UserToDbUser(EntityUser user)
        {
            var u = new User
            {
                DateOfBirth = user.DateOfBirth,
                Firstname = user.Name,
                Gender = user.Gender[0],
                Lastname = user.Surname,
                Password = user.PasswordToBeSaved,
                Username = user.Username,
                Usertype = (short) user.UserType,
            };
            return u;
        }

        private static Mallaca.Measurement DbMeasurementToMeasurement(Measurement m)
        {
            var mallacaMeasurement = new Mallaca.Measurement
            {
                ACT_POWER = m.Power,
                DATE = m.Datatime,
                DISTANCE = (int) m.Distance,
                ENERGY = m.Energy,
                POWER = m.Power,
                PULSE = m.Pulse,
                RPM = m.RPM,
                SPEED = (int) m.Speed,
                TIME = String.Format("{0}:{1}", m.Time.Minutes, m.Time.Seconds)
            };
            return mallacaMeasurement;
        }

        private static Measurement MeasurementToDbMeasurement(EntityMeasurement m, int sessionId, int userId)
        {
            var splitStrings = m.TIME.Split(':');

            var dbMeasurement = new Measurement
            {
                Datatime = m.DATE,
                Distance = m.DISTANCE,
                Energy = m.ENERGY,
                Power = m.POWER,
                Pulse = m.PULSE,
                RPM = m.RPM,
                Session_ID = sessionId,
                User_ID = userId,
                Speed = m.SPEED,
                Time = new TimeSpan(0, int.Parse(splitStrings[0]), int.Parse(splitStrings[1])),
            };
            return dbMeasurement;
        }

    }
}

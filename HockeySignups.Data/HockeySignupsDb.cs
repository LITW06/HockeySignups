using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HockeySignups.Data
{
    public class HockeySignupsDb
    {
        private readonly string _connectionString;

        public HockeySignupsDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddEvent(Event e)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Events (Date, MaxPeople) VALUES (@Date, @MaxPeople); SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@Date", e.Date);
                cmd.Parameters.AddWithValue("@MaxPeople", e.MaxPeople);
                connection.Open();
                e.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public Event GetEventById(int eventId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Events WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", eventId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                reader.Read();
                return new Event
                {
                    Date = (DateTime)reader["Date"],
                    Id = (int)reader["Id"],
                    MaxPeople = (int)reader["MaxPeople"]
                };
            }
        }

        public Event GetLatestEvent()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT TOP 1 * FROM Events 
                                  WHERE Date > GetDate() 
                                  ORDER BY DATE";
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new Event
                {
                    Date = (DateTime)reader["Date"],
                    Id = (int)reader["Id"],
                    MaxPeople = (int)reader["MaxPeople"]
                };
            }
        }

        public IEnumerable<Event> GetEvents()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Events ORDER BY Date DESC";
                connection.Open();
                var reader = cmd.ExecuteReader();
                List<Event> events = new List<Event>();
                while (reader.Read())
                {
                    events.Add(new Event
                    {
                        Date = (DateTime)reader["Date"],
                        Id = (int)reader["Id"],
                        MaxPeople = (int)reader["MaxPeople"]
                    });
                }

                return events;
            }

        }

        public IEnumerable<EventSignup> GetEventSignups(int eventId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                List<EventSignup> signups = new List<EventSignup>();
                cmd.CommandText = "SELECT * FROM EventSignups WHERE EventId = @eventId";
                cmd.Parameters.AddWithValue("@eventId", eventId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    EventSignup es = new EventSignup
                    {
                        Id = (int)reader["Id"],
                        Email = (string)reader["Email"],
                        EventId = (int)reader["EventId"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"]
                    };
                    signups.Add(es);
                }

                return signups;
            }
        }

        public void AddEventSignup(EventSignup es)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO EventSignups (Email, FirstName, LastName, EventId) VALUES (@email, @firstName, @lastName, @eventId)";
                cmd.Parameters.AddWithValue("@email", es.Email);
                cmd.Parameters.AddWithValue("@firstName", es.FirstName);
                cmd.Parameters.AddWithValue("@lastName", es.LastName);
                cmd.Parameters.AddWithValue("@eventId", es.EventId);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<NotificationSignup> GetNotificationSignups()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                List<NotificationSignup> result = new List<NotificationSignup>();
                cmd.CommandText = "SELECT * FROM NotificationSignups";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NotificationSignup ns = new NotificationSignup
                    {
                        Id = (int)reader["Id"],
                        Email = (string)reader["Email"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"]
                    };
                    result.Add(ns);
                }
                return result;
            }
        }

        public EventStatus GetEventStatus(Event e)
        {
            if (e == null || e.Date < DateTime.Today)
            {
                return EventStatus.InThePast;
            }

            int pplAmount = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM EventSignups WHERE EventId = @eventId";
                cmd.Parameters.AddWithValue("@eventId", e.Id);
                connection.Open();
                pplAmount = (int)cmd.ExecuteScalar();
            }

            if (pplAmount < e.MaxPeople)
            {
                return EventStatus.Open;
            }

            return EventStatus.Full;
        }

        public void AddNotificationSignup(NotificationSignup ns)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO NotificationSignups (Email, FirstName, LastName) VALUES (@email, @firstName, @lastName)";
                cmd.Parameters.AddWithValue("@email", ns.Email);
                cmd.Parameters.AddWithValue("@firstName", ns.FirstName);
                cmd.Parameters.AddWithValue("@lastName", ns.LastName);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<EventPeopleCount> GetEventsWithCount()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                List<EventPeopleCount> result = new List<EventPeopleCount>();
                cmd.CommandText = "GetEventsWithCount";
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new EventPeopleCount
                    {
                        Date = (DateTime)reader["Date"],
                        MaxPeople = (int)reader["MaxPeople"],
                        PeopleCount = (int)reader["PeopleCount"],
                        Id = (int)reader["Id"]
                    });
                }
                return result;
            }


        }

        public int GetSignupCountForEvent(int eventId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM EventSignups WHERE EventId = @eventId";
                cmd.Parameters.AddWithValue("@eventId", eventId);
                connection.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
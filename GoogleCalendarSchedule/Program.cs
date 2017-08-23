using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCalendarSchedule
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "Jason's Schedule Calendar App";

        static CalendarService service;
        static UserCredential credential;

        static readonly string MasterCalendarId = "vms.edu_770mvu4qrf16kanie3j1e9k0bc@group.calendar.google.com";

        static Dictionary<String, List<DateTime>> RotationDates = new Dictionary<string, List<DateTime>>();

        static void Main(string[] args)
        {
            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar_creds.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(MasterCalendarId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            foreach(String day in new String[]{ "A", "B", "C", "D", "E", "F"})
                RotationDates.Add(day, new List<DateTime>());


            foreach (var eventItem in events.Items)
            {
                Console.WriteLine("Examining:  {0}", eventItem.Summary);
                foreach (String day in new String[] { "A", "B", "C", "D", "E", "F" })
                {
                    if (eventItem.Summary.Contains(String.Format("\"{0}\" Day", day)))
                    {
                        RotationDates[day].Add(DateTime.Parse(eventItem.Start.Date));
                        Console.WriteLine("{0} is an {1} day.", eventItem.Start.Date, day);
                    }
                }
            }

            using (ScheduleDatabaseEntities db = new ScheduleDatabaseEntities())
            {
                int evtId = db.Events.Count() <= 0 ? 1 : db.Events.OrderBy(e => e.Id).ToList().Last().Id + 1;
                foreach (ClassSchedule schedule in db.ClassSchedules.ToList())
                {
                    foreach (DateTime date in RotationDates["A"])
                    {
                        // only create an event, if it hasn't already been logged.


                        if (schedule.Events.Where(e => e.Date.Equals(date.Date)).Count() <= 0 && schedule.A.Hours > 0)
                        {
                            DateTime st = new DateTime(date.Year, date.Month, date.Day, schedule.A.Hours, schedule.A.Minutes, 0);
                            DateTime et = st.AddMinutes(50);
                            Google.Apis.Calendar.v3.Data.Event evt = new Google.Apis.Calendar.v3.Data.Event()
                            {
                                Start = new EventDateTime() { DateTime = st },
                                End = new EventDateTime() { DateTime = et },
                                Summary = schedule.ClassName                                
                            };

                            EventReminder reminder = new EventReminder() { Method = "sms", Minutes = 10 };
                            evt.Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData() { Overrides = new List<EventReminder>() { reminder }, UseDefault = false };

                            EventsResource.InsertRequest insReq = new EventsResource.InsertRequest(service, evt, "jcox@vms.edu");

                            Google.Apis.Calendar.v3.Data.Event newEvt = insReq.Execute();

                            Event dbEvent = new Event() { ClassScheduleId = schedule.Id, Date = date, CalendarEventURI = newEvt.Id, Id = evtId++ };
                            db.Events.Add(dbEvent);
                            db.SaveChanges();
                        }
                    }

                    foreach (DateTime date in RotationDates["B"])
                    {
                        // only create an event, if it hasn't already been logged.
                        if (schedule.Events.Where(e => e.Date.Equals(date.Date)).Count() <= 0 && schedule.B.Hours > 0)
                        {
                            DateTime st = new DateTime(date.Year, date.Month, date.Day, schedule.B.Hours, schedule.B.Minutes, 0);
                            DateTime et = st.AddMinutes(50);
                            Google.Apis.Calendar.v3.Data.Event evt = new Google.Apis.Calendar.v3.Data.Event()
                            {
                                Start = new EventDateTime() { DateTime = st },
                                End = new EventDateTime() { DateTime = et },
                                Summary = schedule.ClassName
                            };

                            EventReminder reminder = new EventReminder() { Method = "sms", Minutes = 10 };
                            evt.Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData() { Overrides = new List<EventReminder>() { reminder }, UseDefault = false };

                            EventsResource.InsertRequest insReq = new EventsResource.InsertRequest(service, evt, "primary");

                            Google.Apis.Calendar.v3.Data.Event newEvt = insReq.Execute();

                            Event dbEvent = new Event() { ClassScheduleId = schedule.Id, Date = date, CalendarEventURI = newEvt.Id, Id = evtId++ };
                            db.Events.Add(dbEvent);
                            db.SaveChanges();
                        }
                    }

                    foreach (DateTime date in RotationDates["C"])
                    {
                        // only create an event, if it hasn't already been logged.
                        if (schedule.Events.Where(e => e.Date.Equals(date.Date)).Count() <= 0 && schedule.C.Hours > 0)
                        {
                            DateTime st = new DateTime(date.Year, date.Month, date.Day, schedule.C.Hours, schedule.C.Minutes, 0);
                            DateTime et = st.AddMinutes(50);
                            Google.Apis.Calendar.v3.Data.Event evt = new Google.Apis.Calendar.v3.Data.Event()
                            {
                                Start = new EventDateTime() { DateTime = st },
                                End = new EventDateTime() { DateTime = et },
                                Summary = schedule.ClassName
                            };

                            EventReminder reminder = new EventReminder() { Method = "sms", Minutes = 10 };
                            evt.Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData() { Overrides = new List<EventReminder>() { reminder }, UseDefault = false };

                            EventsResource.InsertRequest insReq = new EventsResource.InsertRequest(service, evt, "primary");

                            Google.Apis.Calendar.v3.Data.Event newEvt = insReq.Execute();

                            Event dbEvent = new Event() { ClassScheduleId = schedule.Id, Date = date, CalendarEventURI = newEvt.Id, Id = evtId++ };
                            db.Events.Add(dbEvent);
                            db.SaveChanges();
                        }
                    }

                    foreach (DateTime date in RotationDates["D"])
                    {
                        // only create an event, if it hasn't already been logged.
                        if (schedule.Events.Where(e => e.Date.Equals(date.Date)).Count() <= 0 && schedule.D.Hours > 0)
                        {
                            DateTime st = new DateTime(date.Year, date.Month, date.Day, schedule.D.Hours, schedule.D.Minutes, 0);
                            DateTime et = st.AddMinutes(50);
                            Google.Apis.Calendar.v3.Data.Event evt = new Google.Apis.Calendar.v3.Data.Event()
                            {
                                Start = new EventDateTime() { DateTime = st },
                                End = new EventDateTime() { DateTime = et },
                                Summary = schedule.ClassName
                            };

                            EventReminder reminder = new EventReminder() { Method = "sms", Minutes = 10 };
                            evt.Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData() { Overrides = new List<EventReminder>() { reminder }, UseDefault = false };

                            EventsResource.InsertRequest insReq = new EventsResource.InsertRequest(service, evt, "primary");

                            Google.Apis.Calendar.v3.Data.Event newEvt = insReq.Execute();

                            Event dbEvent = new Event() { ClassScheduleId = schedule.Id, Date = date, CalendarEventURI = newEvt.Id, Id = evtId++ };
                            db.Events.Add(dbEvent);
                            db.SaveChanges();
                        }
                    }

                    foreach (DateTime date in RotationDates["E"])
                    {
                        // only create an event, if it hasn't already been logged.
                        if (schedule.Events.Where(e => e.Date.Equals(date.Date)).Count() <= 0 && schedule.E.Hours > 0)
                        {
                            DateTime st = new DateTime(date.Year, date.Month, date.Day, schedule.E.Hours, schedule.E.Minutes, 0);
                            DateTime et = st.AddMinutes(50);
                            Google.Apis.Calendar.v3.Data.Event evt = new Google.Apis.Calendar.v3.Data.Event()
                            {
                                Start = new EventDateTime() { DateTime = st },
                                End = new EventDateTime() { DateTime = et },
                                Summary = schedule.ClassName
                            };

                            EventReminder reminder = new EventReminder() { Method = "sms", Minutes = 10 };
                            evt.Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData() { Overrides = new List<EventReminder>() { reminder }, UseDefault = false };

                            EventsResource.InsertRequest insReq = new EventsResource.InsertRequest(service, evt, "primary");

                            Google.Apis.Calendar.v3.Data.Event newEvt = insReq.Execute();

                            Event dbEvent = new Event() { ClassScheduleId = schedule.Id, Date = date, CalendarEventURI = newEvt.Id, Id = evtId++ };
                            db.Events.Add(dbEvent);
                            db.SaveChanges();
                        }
                    }

                    foreach (DateTime date in RotationDates["F"])
                    {
                        // only create an event, if it hasn't already been logged.
                        if (schedule.Events.Where(e => e.Date.Equals(date.Date)).Count() <= 0 && schedule.F.Hours > 0)
                        {
                            DateTime st = new DateTime(date.Year, date.Month, date.Day, schedule.F.Hours, schedule.F.Minutes, 0);
                            DateTime et = st.AddMinutes(50);
                            Google.Apis.Calendar.v3.Data.Event evt = new Google.Apis.Calendar.v3.Data.Event()
                            {
                                Start = new EventDateTime() { DateTime = st },
                                End = new EventDateTime() { DateTime = et },
                                Summary = schedule.ClassName
                            };

                            EventReminder reminder = new EventReminder() { Method = "sms", Minutes = 10 };
                            evt.Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData() { Overrides = new List<EventReminder>() { reminder }, UseDefault = false };

                            EventsResource.InsertRequest insReq = new EventsResource.InsertRequest(service, evt, "primary");

                            Google.Apis.Calendar.v3.Data.Event newEvt = insReq.Execute();

                            Event dbEvent = new Event() { ClassScheduleId = schedule.Id, Date = date, CalendarEventURI = newEvt.Id, Id = evtId++ };
                            db.Events.Add(dbEvent);
                            db.SaveChanges();
                        }
                    }
                }
            }

            Console.WriteLine("I'm Done!");
            Console.Read();
        }
    }
}
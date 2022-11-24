using System;
using System.Collections.Generic;
using System.Linq;
using static TicketsConsole.Program;


// To execute C#, please define "static void Main" on a class
// named Solution.

namespace TicketsConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var events = new List<Event>{
                new Event(1, "Phantom of the Opera", "New York", new DateTime(2023,12,23)),
                new Event(2, "Metallica", "Los Angeles", new DateTime(2023,12,02)),
                new Event(3, "Metallica", "New York", new DateTime(2023,12,06)),
                new Event(4, "Metallica", "Boston", new DateTime(2023,10,23)),
                new Event(5, "LadyGaGa", "New York", new DateTime(2023,09,20)),
                new Event(6, "LadyGaGa", "Boston", new DateTime(2023,08,01)),
                new Event(7, "LadyGaGa", "Chicago", new DateTime(2023,07,04)),
                new Event(8, "LadyGaGa", "San Francisco", new DateTime(2023,07,07)),
                new Event(9, "LadyGaGa", "Washington", new DateTime(2023,05,22)),
                new Event(10, "Metallica", "Chicago", new DateTime(2023,01,01)),
                new Event(11, "Phantom of the Opera", "San Francisco", new DateTime(2023,07,04)),
                new Event(12, "Phantom of the Opera", "Chicago", new DateTime(2024,05,15))
            };

            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new DateTime(1995, 05, 10)
            };
            MarketingEngine markEng = new MarketingEngine(customer, events);
        }
    }

    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }

        public Event(int id, string name, string city, DateTime date)
        {
            this.Id = id;
            this.Name = name;
            this.City = city;
            this.Date = date;
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class MarketingEngine
    {
        public MarketingEngine(Customer customer, List<Event> lstEvents)
        {
            Console.WriteLine("Events in the same city as the customer are displayed as follows");
            SendCustomerNotifications(customer, lstEvents);
            Console.WriteLine(@"Events occuring close to customer's next birthday are displayed as follows");
            SendEventCloseToNextBirthDayYear(customer, lstEvents);
            Console.WriteLine("Five closest events to customer are displayed as follows");
            GetFiveClosestEvents(customer, lstEvents);
            Console.WriteLine("Five closest events for question 5 improved by implementing dictionary for caching to customer are displayed as follows");
            GetFiveClosestEventsForQuestion5(customer, lstEvents);
            Console.WriteLine("The Below Code is done for Question 6 i.e sorting with price");
            Dictionary<Event, decimal> dicEventsWithPrice = new();
            var rand = new Random();
            decimal price = 1.0M;
            foreach (Event ev in lstEvents)
            {
                if (!dicEventsWithPrice.ContainsKey(ev))
                {
                    price = Convert.ToDecimal(rand.NextDouble() * 40);
                    while (price < 0.0M)
                    {
                        price = Convert.ToDecimal(rand.NextDouble() * 40);
                    }
                    dicEventsWithPrice.Add(ev, price);
                }
                Console.WriteLine("Event Name = {0} and Event price = {1}", ev.Name, price);
            }
            GetFiveClosestEventsWithPrice(customer, dicEventsWithPrice);
            Console.WriteLine("Exception handling code has been added for question 7");
            GetFiveClosestEventsForQuestion7(customer, new List<Event>());

        }

        public void
            SendCustomerNotifications(Customer customer, List<Event> lstEvents)
        {
            //Option 1 using LINQ to SQL
            var customerEvents = from ev in lstEvents
                                 where ev.City == customer.City
                                 select ev;
            foreach (var e in customerEvents)
            {
                Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");
            }
            /*It can also be done as below*/
            foreach (Event e in lstEvents)
            {
                if (e.City == customer.City)
                    Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");
            }
        }

        public void SendEventCloseToNextBirthDayYear(Customer customer, List<Event> lstEvents)
        {
            DateTime nextBirthday = customer.BirthDate.AddYears(1);
            Dictionary<int, int> dicNext = new();
            foreach (Event e in lstEvents)
            {
                if (!dicNext.ContainsKey(e.Id))
                {
                    int daysDiff = 0;
                    if (e.Date >= nextBirthday)
                    {
                        daysDiff = (e.Date - nextBirthday).Days;
                    }
                    else
                    {
                        daysDiff = (nextBirthday - e.Date).Days;
                    }
                    dicNext.Add(e.Id, daysDiff);
                }
            }
            int min = 0;
            int evID = 1;
            if (dicNext.Count > 0)
            {
                min = dicNext[dicNext.Keys.First()];
            }

            foreach (int ids in dicNext.Keys)
            {
                if (dicNext[ids] <= min)
                {
                    min = dicNext[ids];
                    evID = ids;
                }
            }

            Console.WriteLine($"{customer.Name} from {customer.City} event {lstEvents.Where(x => x.Id == evID).FirstOrDefault().Name} at {lstEvents.Where(x => x.Id == evID).FirstOrDefault().Date}");
        }

        public static List<Event> GetFiveClosestEvents(Customer customer, List<Event> lstEvent)
        {
            Dictionary<Event, int> dicFiveClosestEvents = new();
            List<Event> fiveNearestevents = new List<Event>();
            foreach (Event ev in lstEvent)
            {
                if (!dicFiveClosestEvents.ContainsKey(ev))
                {
                    int dis = GetDistance(customer.City, ev.City);
                    dicFiveClosestEvents.Add(ev, dis);
                }
            }
            var fiveEvents = from x in dicFiveClosestEvents
                             orderby x.Value
                             select x;
            Console.WriteLine("Events sorted in ascending order");
            foreach (var eve in fiveEvents)
            {
                Console.WriteLine("Nearest Event Name = {0} and Event distance = {1}", eve.Key.Name, eve.Value);
            }
            int index = 0;
            foreach (var y in fiveEvents)
            {
                if (index < 5)
                {
                    fiveNearestevents.Add(y.Key);
                    index++;
                }
                if (index == 5) break;
            }
            Console.WriteLine("Five nearest events sorted in ascending order");
            foreach (var r in fiveNearestevents)
            {
                Console.WriteLine("Nearest Event Name = {0} and Event City = {1}", r.Name, r.City);
            }
            return fiveNearestevents;
        }

        public static int GetDistance(string fromCity, string toCity)
        {
            int distance = 0;
            int maxLength = Math.Max(fromCity.Length, toCity.Length);
            int minLength = Math.Min(fromCity.Length, toCity.Length);
            int i = 0;

            for (; i < minLength; i++)//for to add up difference between characters
            {
                distance += Math.Abs(fromCity[i] - toCity[i]);
            }

            for (int j = i; j < maxLength; j++)//for the remaining characters get all ASCII values of characters in string array
            {
                distance += fromCity.Length > toCity.Length ? Convert.ToInt32(fromCity[j]) : Convert.ToInt32(toCity[j]);
            }
            return distance;
        }

        public static List<Event> GetFiveClosestEventsForQuestion5(Customer customer, List<Event> lstEvent)
        {
            Dictionary<Event, int> dicFiveClosestEvents = new();
            List<Event> fiveNearestevents = new List<Event>();
            Dictionary<string, int> dicDistance = new Dictionary<string, int>();
            foreach (Event ev in lstEvent)
            {
                if (!dicFiveClosestEvents.ContainsKey(ev))
                {
                    int dis = 0;
                    if (!dicDistance.ContainsKey(customer.City + '-' + ev.City))
                    {
                        dis = GetDistance(customer.City, ev.City);
                        dicDistance.Add(customer.City + '-' + ev.City, dis);
                    }
                    else
                    {
                        dis = dicDistance[customer.City + '-' + ev.City];
                    }
                    dicFiveClosestEvents.Add(ev, dis);
                }
            }

            foreach (string str in dicDistance.Keys)
            {
                Console.WriteLine("from to city {0} distance {1}", str, dicDistance[str]);
            }

            var fiveEvents = from x in dicFiveClosestEvents
                             orderby x.Value
                             select x;
            Console.WriteLine("Events sorted in ascending order");
            foreach (var eve in fiveEvents)
            {
                Console.WriteLine("Nearest Event Name = {0} and Event distance = {1}", eve.Key.Name, eve.Value);
            }
            int index = 0;
            foreach (var y in fiveEvents)
            {
                if (index < 5)
                {
                    fiveNearestevents.Add(y.Key);
                    index++;
                }
                if (index == 5) break;
            }
            Console.WriteLine("Five nearest events sorted in ascending order");
            foreach (var r in fiveNearestevents)
            {
                Console.WriteLine("Nearest Event Name = {0} and Event City = {1}", r.Name, r.City);
            }
            return fiveNearestevents;
        }

        public static List<Event> GetFiveClosestEventsWithPrice(Customer customer, Dictionary<Event, decimal> dicEventprice)
        {
            Dictionary<Event, int> dicFiveClosestEvents = new();
            List<Event> fiveNearestevents = new List<Event>();
            foreach (Event ev in dicEventprice.Keys)
            {
                if (!dicFiveClosestEvents.ContainsKey(ev))
                {
                    int dis = GetDistance(customer.City, ev.City);
                    dicFiveClosestEvents.Add(ev, dis);
                }
            }
            var fiveEvents = from x in dicFiveClosestEvents
                             orderby x.Value
                             select x;

            foreach (var eve in fiveEvents)
            {
                Console.WriteLine("Nearest Event Name = {0} Event distance = {1}", eve.Key.Name, eve.Value);
            }
            int index = 0;


            Dictionary<Event, decimal> dicEP = new();
            foreach (var ep in fiveEvents)
            {
                if (dicEventprice.ContainsKey(ep.Key))
                {
                    if (!dicEP.ContainsKey(ep.Key))
                    {
                        dicEP.Add(ep.Key, dicEventprice[ep.Key]);
                    }
                }
            }

            var fiveChepestEvents = from x in dicEP
                                    orderby x.Value
                                    select x;

            foreach (var y in fiveChepestEvents)
            {
                if (index < 5)
                {
                    fiveNearestevents.Add(y.Key);
                    index++;
                }
                if (index == 5) break;
            }
            foreach (var r in fiveNearestevents)
            {
                Console.WriteLine("Nearest and Cheapest Event Name = {0} Event City = {1}", r.Name, r.City);
            }
            return fiveNearestevents;
        }

        public static List<Event> GetFiveClosestEventsForQuestion7(Customer customer, List<Event> lstEvent)
        {
            if (lstEvent.Count == 0)
            {
                Console.WriteLine("No Events have been sent! Method will exit with Forbidden status code 403");
                return null;
            }
            Dictionary<Event, int> dicFiveClosestEvents = new();
            List<Event> fiveNearestevents = new List<Event>();
            Dictionary<string, int> dicDistance = new Dictionary<string, int>();
            try
            {
                foreach (Event ev in lstEvent)
                {
                    if (!dicFiveClosestEvents.ContainsKey(ev))
                    {
                        int dis = 0;
                        if (!dicDistance.ContainsKey(customer.City + '-' + ev.City))
                        {
                            dis = GetDistance(customer.City, ev.City);
                            dicDistance.Add(customer.City + '-' + ev.City, dis);
                        }
                        else
                        {
                            dis = dicDistance[customer.City + '-' + ev.City];
                        }
                        dicFiveClosestEvents.Add(ev, dis);
                    }
                }

                if (dicFiveClosestEvents.Count > 0 || dicDistance.Count > 0)
                {
                    Console.WriteLine("Unable to find events near to customer method will exit with Notfound status code 404");
                    return null;
                }

                foreach (string str in dicDistance.Keys)
                {
                    Console.WriteLine("from to city {0} distance {1}", str, dicDistance[str]);
                }

                var fiveEvents = from x in dicFiveClosestEvents
                                 orderby x.Value
                                 select x;
                Console.WriteLine("Events sorted in ascending order");
                foreach (var eve in fiveEvents)
                {
                    Console.WriteLine("Nearest Event Name = {0} and Event distance = {1}", eve.Key.Name, eve.Value);
                }
                int index = 0;
                foreach (var y in fiveEvents)
                {
                    if (index < 5)
                    {
                        fiveNearestevents.Add(y.Key);
                        index++;
                    }
                    if (index == 5) break;
                }
                Console.WriteLine("Five nearest events sorted in ascending order");
                foreach (var r in fiveNearestevents)
                {
                    Console.WriteLine("Nearest Event Name = {0} and Event City = {1}", r.Name, r.City);
                }
            }
            catch
            {
                Console.WriteLine("Error while processing your request method will exit with InternalServerError status code 500");
            }
            return fiveNearestevents;
        }

    }

}

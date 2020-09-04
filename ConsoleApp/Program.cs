using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            //AddSamurai();
            //GetSamurais("After Add:");
            //InsertMultipleSamurais();
            //QueryFilters();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurai();
            //RetriveAndDeleteASamurai();
            //InsertBattle();
            //QueryAndUpdateBattle_Disconnected();
            //InsertNewSamuraiWithAQuote();
            //AddQuoteToExistingSamuraiWhileTracked();
            //AddQuoteToExistingSamuraiNotTracked(2);
            //AddQuoteToExistingSamuraiNotTracked_Easy(2);
            //ProjectSomeProperties();
            //ProjectSamuraisWithQuotes();
            //ExplicitLoadQuotes();
            //FilteringWithRelatedData();
            //ModifyingRelatedDataWhenNotTracked();
            //EnlistSamuraiIntoBattle();
            //RemoveJoinBetweenSamuraiAndBattle();
            //GetSamuraiWithBattles();
            //AddNewSamuraiWithHorse();
            //AddNewHorseToSamuraiUsingId();
            //AddNewHorseToSamuraiObject();
            AddNewHorseToDisconnectedSamuraiObject();

            Console.Write("Press any key...");
            Console.ReadKey();

        }

        private static void InsertMultipleSamurais()
        {
            var samurai = new Samurai { Name = "Sampson" };
            var samurai2 = new Samurai { Name = "Tasha" };
            var samurai3 = new Samurai { Name = "Number 3" };
            var samurai4 = new Samurai { Name = "Number 4" };

            _context.Samurais.AddRange(samurai, samurai2, samurai3, samurai4);
            _context.SaveChanges();
        }

        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "Kikuchio" };
            var clan = new Clan { ClanName = "Imperial Clan" };
            _context.AddRange(samurai, clan);
            _context.SaveChanges();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Sampson" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void GetSamuraisSimpler()
        {
            var samurais = _context.Samurais.ToList();
        }

        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }

        private static void QueryFilters()
        {
            //var samurais = _context.Samurais.Where(s => s.Name == "Sampson").ToList();
            //var samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "J%")).ToList();
            //var samurais = _context.Samurais.Find(2);
            var name = "Sampson";
            var last = _context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurai()
        {
            var samurais = _context.Samurais.Skip(1).Take(3).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }

        private static void RetriveAndDeleteASamurai()
        {
            var samurai = _context.Samurais.Find(11);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void InsertBattle()
        {
            _context.Battles.Add(new Battle
            {
                Name = "Battle of Okehazama",
                StartDate = new DateTime(1560, 05, 01),
                EndDate = new DateTime(1560, 05, 15)
            });
            _context.SaveChanges();
        }

        private static void QueryAndUpdateBattle_Disconnected()
        {
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
            battle.EndDate = new DateTime(1560, 06, 30);

            using (var newContextInstance = new SamuraiContext())
            {
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "I've come to save you" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });

            using (var newContext = new SamuraiContext())
            {
                //newContext.Samurais.Update(samurai); - to robi niepotrzebny update lepiej użyć Atach
                newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }
        }

        private static void AddQuoteToExistingSamuraiNotTracked_Easy(int samuraiId)
        {
            var quote = new Quote
            {
                Text = "Now that I saved you, will you feed me dinner again?",
                SamuraiId = samuraiId //użycie klucza obcego
            };

            using (var newContext = new SamuraiContext())
            {
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }

        private static void EagerLoadSamuraiWithQuote()
        {
            var samuraiWithQuotes = _context.Samurais.Where(s => s.Name.Contains("Julie"))
                                                        .Include(s => s.Quotes);

            //tak nie może być bo FirstOrDefault czy Find zwraca typ Samurai a metoda Include jest metodą DBSeta
            //var samuraiWithQuotes = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Julie"))
            //                              .Include(s => s.Quotes);
            //
            //var samuraiSixteen = _context.Samurais.Find(16).Include(s => s.Quotes);

            //Jak chcemy pobrać więcej DBSetów to tak:
            //_context.Samurais.Include(s => s.Quotes)
            //                  .ThenInclude(q => q.Translations)
            //
            //_context.Samurais.Include(s => s.Quotes)
            //                  .Include(q => q.Clan)
        }

        private static void ProjectSomeProperties()
        {
            //new { s.Id, s.Name } to typ anonimowy dostępny tylko w tej metodzie
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();

            //jak chcemy mieć dostęp spoza metody możemy utworzyć typ albo jak tutaj strukturę
            var idsAndNames = _context.Samurais.Select(s => new IdAndNames(s.Id, s.Name)).ToList();
        }

        public struct IdAndNames
        {
            public int Id;
            public string Name;
            public IdAndNames(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        private static void ProjectSamuraisWithQuotes()
        {
            var somePropertiesWithQuotes = _context.Samurais
                .Select(s => new { s.Id, s.Name, s.Quotes })
                .ToList();
            //można też np. tak
            //.Select(s => new { s.Id, s.Name, s.Quotes.Count }).ToList();

            var somePropertiesWithQuotes2 = _context.Samurais
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
                })
                .ToList();
        }

        private static void ExplicitLoadQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Julie"));
            //Load() można wywołać tylko dla pojedynczych obiektów, nie da się dla listy samurajów
            //dla właściwości która jest kolekcją
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            //dla właściwości która jest tylko referencją a nie kolekcją
            _context.Entry(samurai).Reference(s => s.Horse).Load();
        }

        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                                    .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                                    .ToList();
        }

        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            samurai.Quotes[0].Text = " Did you hear that?";
            _context.Quotes.Remove(samurai.Quotes[2]);
            _context.SaveChanges();
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            var quote = samurai.Quotes[0];
            quote.Text += " Did you hear that again?";

            using (var newContext = new SamuraiContext())
            {
                //newContext.Quotes.Update(quote);
                //Jeśli chcemy by EF Core nie updatował wszystkich cytatów z grafu obiektów, to trzeba użyć Entry jak niżej
                //ustawić State na Modified i wtedy SaveChanges zapisze zmiany
                newContext.Entry(quote).State = EntityState.Modified;
                newContext.SaveChanges();
            }
        }

        //chyba dotąd odpalałem w pracy

        private static void JoinBattleAndSamurai()
        {
            //Samurai i bitwa już istnieją i mamy ich IDs
            var sbJoin = new SamuraiBattle { SamuraiId = 1, BattleId = 3 };
            _context.Add(sbJoin);
            _context.SaveChanges();
        }

        private static void EnlistSamuraiIntoBattle()
        {
            var battle = _context.Battles.Find(1);
            battle.SamuraiBattles.Add(new SamuraiBattle { SamuraiId = 6 });
            _context.SaveChanges();
        }

        private static void RemoveJoinBetweenSamuraiAndBattle()
        {
            //Normalnie powinniśmy najpierw pobrać obiekt, ale tu jest tak prosto, że jest bezpiecznie go utworzyć
            var join = new SamuraiBattle { BattleId = 1, SamuraiId = 1 };
            _context.Remove(join);
            _context.SaveChanges();
        }

        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattle = _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle)
                .FirstOrDefault(samurai => samurai.Id == 6);

            var samuraiWithBattlesCleaner = _context.Samurais.Where(s => s.Id == 6)
                .Select(s => new
                {
                    Samurai = s,
                    Battles = s.SamuraiBattles.Select(sb => sb.Battle)
                })
                .FirstOrDefault();
        }

        private static void AddNewSamuraiWithHorse()
        {
            var samurai = new Samurai { Name = "Jina Ujichika" };
            samurai.Horse = new Horse { Name = "Silver" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddNewHorseToSamuraiUsingId()
        {
            var horse = new Horse { Name = "Scout", SamuraiId = 2 };
            _context.Add(horse);
            _context.SaveChanges();
        }

        private static void AddNewHorseToSamuraiObject()
        {
            var samurai = _context.Samurais.Find(3);
            samurai.Horse = new Horse { Name = "Black Beauty" };
            _context.SaveChanges();
        }

        private static void AddNewHorseToDisconnectedSamuraiObject()
        {
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 6);
            samurai.Horse = new Horse { Name = "Mr. Ed" };
            using (var newContext = new SamuraiContext())
            {
                newContext.Attach(samurai);
                newContext.SaveChanges();
            }
        }
    }
}

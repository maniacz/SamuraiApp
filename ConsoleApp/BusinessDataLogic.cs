using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    public class BusinessDataLogic
    {
        private SamuraiContext _context;

        public BusinessDataLogic(SamuraiContext context)
        {
            _context = context;
        }

        public BusinessDataLogic()
        {
            _context = new SamuraiContext();
        }

        public int AddMultipleSamurais(string[] nameList)
        {
            var samuraiList = new List<Samurai>();
            foreach (var name in nameList)
            {
                samuraiList.Add(new Samurai { Name = name });
            }
            _context.Samurais.AddRange(samuraiList);

            var dbResult = _context.SaveChanges();
            return dbResult;
        }

        public int InsertNewSamurai(Samurai samurai)
        {
            _context.Samurais.Add(samurai);
            var dbResult = _context.SaveChanges();
            return dbResult;
        }

        public Samurai GetSamuraiWithQuotes(int samuraiId)
        {
            var samuraiWithQuotes = _context.Samurais.Where(s => s.Id == samuraiId)
                                                     .Include(s => s.Quotes)
                                                     .FirstOrDefault();

            return samuraiWithQuotes;
        }
    }
}

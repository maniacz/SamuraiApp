using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using TestsForConsoleApp;
using Microsoft.EntityFrameworkCore;
using ConsoleApp;

namespace TestsForConsoleApp
{
    [TestClass]
    public class BizDataLogicTests
    {
        [TestMethod]
        public void AddMultipleSamuraisReturnsCorrectNumberOfInsertedRows()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("AddMultipleSamurais");
            using (var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                var nameList = new string[] { "Kikuchiyo", "Kyuzo", "Rikchi" };
                var result = bizlogic.AddMultipleSamurais(nameList);
                Assert.AreEqual(nameList.Length, result);
            }
        }

        [TestMethod]
        public void CanInsertSingleSamurai()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("InsertNewSamurai");

            using (var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                bizlogic.InsertNewSamurai(new Samurai());
            }

            using (var context2 = new SamuraiContext(builder.Options))
            {
                Assert.AreEqual(1, context2.Samurais.CountAsync().Result);
            }
        }

        [TestMethod]
        public void CanInsertSamuraiwithQuotes()
        {

            var samuraiGraph = new Samurai
            {
                Name = "Kyūzō",
                Quotes = new List<Quote> {
                  new Quote { Text = "Watch out for my sharp sword!" },
                  new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" }
                }
            };
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("SamuraiWithQuotes");
            using (var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                var result = bizlogic.InsertNewSamurai(samuraiGraph);
            };
            using (var context2 = new SamuraiContext(builder.Options))
            {
                var samuraiWithQuotes = context2.Samurais.Include(s => s.Quotes).FirstOrDefaultAsync().Result;
                Assert.AreEqual(2, samuraiWithQuotes.Quotes.Count);
            }

        }
        [TestMethod, TestCategory("SamuraiWithQuotes")]
        public void CanGetSamuraiwithQuotes()
        {
            int samuraiId;
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("SamuraiWithQuotes");
            using (var seedcontext = new SamuraiContext(builder.Options))
            {
                var samuraiGraph = new Samurai
                {
                    Name = "Kyūzō",
                    Quotes = new List<Quote> {
                        new Quote { Text = "Watch out for my sharp sword!" },
                        new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" }
                      }
                };
                seedcontext.Samurais.Add(samuraiGraph);
                seedcontext.SaveChanges();
                samuraiId = samuraiGraph.Id;
            }
            using (var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                var result = bizlogic.GetSamuraiWithQuotes(samuraiId);
                Assert.AreEqual(2, result.Quotes.Count);
            };
        }
    }
}

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Pentathanerd.When.Models;

namespace Pentathanerd.When.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        { 
            return View();
        }

        public ActionResult HardReset(string resetKey)
        {
            if (resetKey == "hardToGuessResetKey123")
                GamePlayHub.HardReset();

            return RedirectToAction("Index");
        }

        public ActionResult ClearLeaderboard(string resetKey)
        {
            if (resetKey == "hardToGuessResetLeaderboardKey123")
                GamePlayHub.ClearLeaderboard();

            return RedirectToAction("Leaderboard");
        }

        public ActionResult Leaderboard()
        {
            var leaderboard = GamePlayHub.GetLeaderboard();

            var models = ToLeaderboardModels(leaderboard).OrderByDescending(x => x.Score).ToList();

            return View(models);
        }

        private List<LeaderboardModel> ToLeaderboardModels(ConcurrentDictionary<string, int> dictionary)
        {
            var retValue = new List<LeaderboardModel>();

            foreach (var i in dictionary)
            {
                retValue.Add(new LeaderboardModel
                {
                    Score = i.Value,
                    TeamName = i.Key
                });
            }

            return retValue;
        }
    }
}
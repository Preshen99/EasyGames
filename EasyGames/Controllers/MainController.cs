using EasyGames.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EasyGames.Controllers
{
    public class MainController : Controller
    {
        private EasyGamesDBEntities db = new EasyGamesDBEntities();

        // GET: Main
        public ActionResult Index(string indexing, string searching, Transaction transaction)
        {
            ViewBag.Name = String.IsNullOrEmpty(indexing) ? "name_desc" : "";
            ViewBag.Surname = String.IsNullOrEmpty(indexing) ? "surname_desc" : "";

            var clients = from c in db.Clients select c;

            if (!String.IsNullOrEmpty(searching))
            {
                clients = clients.Where(c => c.Name.Contains(searching) || c.Surname.Contains(searching));
            }

            switch (indexing)
            {
                case "name_desc":
                    clients = clients.OrderByDescending(c => c.Name);
                    break;
                case "surname_desc":
                    clients = clients.OrderByDescending(c => c.Surname);
                    break;
                default:
                    clients = clients.OrderBy(c=>c.Name);
                    break;
            }

            return View(clients.ToList());
        }

        public ActionResult ViewTransactions(int? id, string searching)
        {
            var transactions = db.Transactions.Include(t => t.Client).Include(t => t.TransactionType);

            if (!String.IsNullOrEmpty(searching))
            {
                transactions = transactions.Where(c => c.TransactionType.TransactionTypeName.Contains(searching) || c.Comment.Contains(searching));
            }

            @ViewBag.id = id;

            return View(transactions.ToList());
        }

        public ActionResult EditComment(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Name", transaction.ClientID);
            ViewBag.TransactionTypeID = new SelectList(db.TransactionTypes, "TransactionTypeID", "TransactionTypeName", transaction.TransactionTypeID);
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "TransactionID,Amount,TransactionTypeID,ClientID,Comment")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientID", "Name", transaction.ClientID);
            ViewBag.TransactionTypeID = new SelectList(db.TransactionTypes, "TransactionTypeID", "TransactionTypeName", transaction.TransactionTypeID);
            return View(transaction);
        }

        public ActionResult AddTransaction()
        {
            return View();
        }
    }
}
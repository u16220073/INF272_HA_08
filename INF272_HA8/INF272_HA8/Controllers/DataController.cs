using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INF272_HA8.Models;
using System.Data;
using System.Data.SqlClient;

namespace INF272_HA8.Controllers
{
    public class DataController : Controller
    {
        // GET: Data
        static List<Franchise> tempList = new List<Franchise>();
        //SqlConkknection DBconnection = new SqlConnection("Data Source=.\\MSSQL14; Initial Catalog=MyShop; Integrated Security = True");
        SqlConnection DBconnection = new SqlConnection("Data Source=ROLH-017\\SYSARCH; Initial Catalog=MyShop; Integrated Security = True");

        public ActionResult Index()
        {
            DataTable dbList = new DataTable();
            DBconnection.Open();
            SqlDataAdapter results = new SqlDataAdapter("select * from Franchise", DBconnection);
            results.Fill(dbList);

            //Convert DataTable to List Using Linq
            //src=> https://www.c-sharpcorner.com/UploadFile/ee01e6/different-way-to-convert-datatable-to-list
            tempList = (from DataRow dr in dbList.Rows
                                     select new Franchise()
                                     {
                                         FranchiseID = Convert.ToInt32(dr["FranchiseID"]),
                                         FranchiseArea = dr["FranchiseArea"].ToString(),
                                         FranchiseName = dr["FranchiseName"].ToString(),
                                         FranchiseOwner = dr["FranchiseOwner"].ToString(),
                                         FranchiseNumberClients = dr["FranchiseNumberClients"].ToString()
                                     }).ToList();
            DBconnection.Close(); //skeptical
            return View(tempList);
        }

        // GET: Data/Create
        public ActionResult Create()
        {
            ViewBag.Msg = " ";
            return View();
        }
        
        // POST: Data/Create
        [HttpPost]
        public ActionResult Create(Franchise newFranchise)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlCommand newTuple = new SqlCommand("Insert into Franchise values(' " + newFranchise.FranchiseArea + " ', ' " + newFranchise.FranchiseName
 + " ',' " + newFranchise.FranchiseOwner + " ',' " + newFranchise.FranchiseNumberClients + " ') ", DBconnection);
                    DBconnection.Open();
                    int rows = newTuple.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch(Exception err)
            {
                ViewBag.Msg = "Error: " + err.Message.Replace("\r\n", " ");
                return View();
            }
            finally
            { DBconnection.Close(); }
        }

        // GET: Data/Edit/5
        public ActionResult Edit(int id)
        {
            Franchise curFranchise = new Franchise();
            DataTable cfFromDB = new DataTable();

            DBconnection.Open();
            SqlDataAdapter results = new SqlDataAdapter("select * from Franchise where FranchiseID=' "+id+" ' ", DBconnection);
            results.Fill(cfFromDB);
            try
            {
                if (cfFromDB.Rows.Count == 1)
                {
                    curFranchise.FranchiseArea = cfFromDB.Rows[0][1].ToString();
                    curFranchise.FranchiseName = cfFromDB.Rows[0][2].ToString();
                    curFranchise.FranchiseOwner = cfFromDB.Rows[0][3].ToString();
                    curFranchise.FranchiseNumberClients = cfFromDB.Rows[0][4].ToString();

                    DBconnection.Close();
                    return View(curFranchise);
                }
                else throw new Exception("Duplicate IDs! #something is very wrong");
            }
            catch (Exception err)
            {
                ViewBag.Msg = "Error: " + err.Message.Replace("\r\n", " ");
                return View();
            }
        }

        // POST: Data/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Franchise editedFranchise)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlCommand newTuple = new SqlCommand("update Franchise set FranchiseArea=' " + editedFranchise.FranchiseArea + " ', FranchiseName=' " + editedFranchise.FranchiseName
 + " ',FranchiseOwner=' " + editedFranchise.FranchiseOwner + " ',FranchiseNumberClients=' " + editedFranchise.FranchiseNumberClients + " ' where FranchiseID=' "+id+"'", DBconnection);
                    DBconnection.Open();
                    int rows = newTuple.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ViewBag.Msg = "Error: " + err.Message.Replace("\r\n", " ");
                return View();
            }
            finally
            { DBconnection.Close(); }
        }

        // GET: Data/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlCommand newTuple = new SqlCommand("delete from Franchise where FranchiseID=' " + id + "'", DBconnection);
                    DBconnection.Open();
                    int rows = newTuple.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ViewBag.Msg = "Error: " + err.Message.Replace("\r\n", " ");
                return RedirectToAction("Index");
            }
            finally
            { DBconnection.Close(); }
        }

        ///NOW FOR SECTION 2
        public ActionResult Main()
        {
            MyShopEntities db = new MyShopEntities();
            return View(db.Franchises.ToList() );
        }
        public ActionResult CreateETF()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateETF(Franchise newFrancET)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MyShopEntities db = new MyShopEntities();
                    db.Franchises.Add(newFrancET);
                    ViewBag.Msg = "New Franchise successfully added!";
                    db.SaveChanges();
                }
                return RedirectToAction("Main");

            }
            catch (Exception err)
            {
                ViewBag.Msg = "ERROR: " + err.Message;
                return View();
            }
        }

        public ActionResult EditETF(int id)
        {
            MyShopEntities db = new MyShopEntities();
            return View(db.Franchises.Where(x => x.FranchiseID == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult EditETF(int id, Franchise edtdFrancETF)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MyShopEntities db = new MyShopEntities();
                    db.Entry(edtdFrancETF).State = System.Data.Entity.EntityState.Modified;

                    ViewBag.Msg = "New Franchise successfully Updated!";
                    db.SaveChanges();
                }
                return RedirectToAction("Main");

            }
            catch (Exception err)
            {
                return View();
            }
            //return View();
        }

        [HttpGet]
        public ActionResult DeleteETF(int id)
        {
            MyShopEntities db = new MyShopEntities();
            Franchise tarFrenchise = db.Franchises.Where(x => x.FranchiseID == id).FirstOrDefault();
            db.Franchises.Remove(tarFrenchise);
            db.SaveChanges();
            return RedirectToAction("Main");
        }

    }
}

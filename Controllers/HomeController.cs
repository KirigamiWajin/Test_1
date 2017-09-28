using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;
using System.Configuration;
using System.Data.Sql;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
      /* SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
        OleDbConnection Econ;*/
        public UserContext db = new UserContext();

        public ActionResult Index()
        {
            
                return View();
        }

           [HttpPost]

         public ActionResult Index(HttpPostedFileBase file )
         {
            
            string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filepath = "/excelfolder/" + filename;
            file.SaveAs(Path.Combine(Server.MapPath("/excelfolder"), filename));
            DataTable csvData = new DataTable();
            using (TextFieldParser tfp = new TextFieldParser(Server.MapPath("/excelfolder/" + filename)))
            {
                DataColumn datecolumnNum = new DataColumn("FIO");
                datecolumnNum.AllowDBNull = true;
                csvData.Columns.Add(datecolumnNum);
                DataColumn datecolumnSer = new DataColumn("Birthday");
                datecolumnSer.AllowDBNull = true;
                csvData.Columns.Add(datecolumnSer);
                DataColumn datecolumnser1 = new DataColumn("Email");
                datecolumnser1.AllowDBNull = true;
                csvData.Columns.Add(datecolumnser1);
                DataColumn datecolumnser2 = new DataColumn("phone");
                datecolumnser2.AllowDBNull = true;
                csvData.Columns.Add(datecolumnser2);
                tfp.SetDelimiters(";");
                tfp.HasFieldsEnclosedInQuotes = true;
                    while (!tfp.EndOfData)
                    {
                        string[] fields = tfp.ReadFields();
                        csvData.Rows.Add(fields);
                    }
            }
            csvData.Rows.Remove(csvData.Rows[0]);
            try
            {
                var users = csvData.AsEnumerable().Select(row => new User
                {
                    FIO = row.Field<string>("FIO").Replace("  ", string.Empty).Trim(),
                    Birthday = row.Field<string>("Birthday"),
                    Email = row.Field<string>("Email").Replace(" ", string.Empty).Trim(),
                    phone = Regex.Replace(row.Field<string>("phone"), @"[^\d]", "")
                });
                using (UserContext context = new UserContext())
                {
                    context.Users.AddRange(users);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                
            }
            

            return View();
         }


       public ActionResult loaddata()
        {
            using (UserContext db = new UserContext())
            {
               
                var data = db.Users.OrderBy(a => a.FIO).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UserView(int id)
        {
            
            var user = db.Users.Find(id);
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]

        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            User user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]

        public ActionResult EditUser(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
using jQueryAjaxMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryAjaxMVC.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAll()
        {
            return View(GetAllEmployee());
        }

        IEnumerable<Employee> GetAllEmployee()
        {
            using (jQueryAjaxDBEntities db = new jQueryAjaxDBEntities())
            {
                return db.Employee.ToList<Employee>();
            }
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Employee emp = new Employee();

            if (id != 0)
            {
                using (jQueryAjaxDBEntities db = new jQueryAjaxDBEntities())
                {
                    emp = db.Employee.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
                }
            }

            return View(emp);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Employee emp)
        {
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);

                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.ImagePath = "~/AppFiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (jQueryAjaxDBEntities db = new jQueryAjaxDBEntities())
                {
                    if (emp.EmployeeID == 0)
                    {
                        db.Employee.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();
                    }                    
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Sikeres hozzáadás." }, JsonRequestBehavior.AllowGet);

            }
            catch(Exception e)
            {
                return Json(new { success = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (jQueryAjaxDBEntities db = new jQueryAjaxDBEntities())
                {
                    Employee emp = db.Employee.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
                    db.Employee.Remove(emp);
                    db.SaveChanges();
                }

                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Sikeres törlés." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
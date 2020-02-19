using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            List<ContactModel> contacts = new List<ContactModel>();
            using (MyContactBookEntities4 dc = new MyContactBookEntities4())
            {
                var v = (from a in dc.Contacts 
                         join b in dc.Emails on a.ContactID equals b.ContactID 
                         join c in dc.Phones on a.ContactID equals c.ContactID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             Firstname = a.Firstname,
                             Lastname = a.Lastname,
                             Email = b.email1,
                             Phone = c.phone1,
                             Address = a.Address
                         }).ToList();
                contacts = v;
            }
            return View(contacts);
        }

        public ActionResult Home()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ContactModel c, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (MyContactBookEntities4 dc = new MyContactBookEntities4())
                {
                    Contact contact = new Contact();
                    contact.Firstname = c.Firstname;
                    contact.Lastname = c.Lastname;
                    contact.Address = c.Address;
                    dc.Contacts.Add(contact);
                    dc.SaveChanges();
                    
                    Email email = new Email();
                    email.email1 = c.Email;
                    email.ContactID = contact.ContactID;
                    contact.emailid = email.emailid;
                    dc.Emails.Add(email);
                    dc.SaveChanges();

                    if (c.Email1 != null)
                    {
                        Email email1 = new Email();
                        email1.email1 = c.Email1;
                        email1.ContactID = contact.ContactID;
                        contact.emailid = email.emailid;
                        dc.Emails.Add(email1);
                        dc.SaveChanges();
                    }

                    Phone phone = new Phone();
                    phone.phone1 = c.Phone;
                    phone.ContactID = contact.ContactID;
                    contact.phoneid = phone.phoneid;
                    dc.Phones.Add(phone);
                    dc.SaveChanges();

                    if (c.Phone1 != 0)
                    {
                        Phone phone1 = new Phone();
                        phone1.phone1 = c.Phone1;
                        phone1.ContactID = contact.ContactID;
                        contact.phoneid = phone.phoneid;
                        dc.Phones.Add(phone1);
                        dc.SaveChanges();
                    }
 }
                return RedirectToAction("Index");
            }
            else
            {
                return View(c);
            }
        }

        public ActionResult View(int id)
        {
            ContactModel c = null;
           c = GetContact(id);
            return View(c);
        }
        private ContactModel GetContact(int contactID)
        {
            ContactModel contact = new ContactModel();
            using (MyContactBookEntities4 dc = new MyContactBookEntities4())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Emails on a.ContactID equals b.ContactID
                         join c in dc.Phones on a.ContactID equals c.ContactID
                         where a.ContactID.Equals(contactID)
                         select new
                         {
                             a.Address,
                             a.Firstname,
                             a.Lastname,
                             b.email1,
                             c.phone1
                         }).FirstOrDefault();
                if (v != null)
               { 
                    contact.Address = v.Address;
                    contact.Firstname = v.Firstname;
                    contact.Lastname = v.Lastname;
                    contact.Email = v.email1 ;
                    contact.Phone = v.phone1;
                }   
            }
            return contact;
        }

        public ActionResult Edit(int id)
        {
            ContactModel c = null;
            c = GetContact(id);
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactModel c,int id, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (MyContactBookEntities4 dc = new MyContactBookEntities4())
                {
                    Email e = new Email();
                    e = dc.Emails.Where(a => a.ContactID == id).FirstOrDefault();
                    if (e != null)
                    {
                        e.email1 = c.Email;
                    }
                    dc.SaveChanges();
                         
                     Phone p = new Phone();
                    p = dc.Phones.Where(a => a.ContactID == id).FirstOrDefault();
                    if (p != null)
                    {
                        p.phone1 = c.Phone;
                    }

                    dc.SaveChanges();
                     
                    Contact co = new Contact();
                    co = dc.Contacts.Where(a => a.ContactID==id).FirstOrDefault();
                    if (co != null)
                    {
                        co.Firstname = c.Firstname;
                        co.Lastname = c.Lastname;
                        co.Firstname = c.Firstname;
                    }
                 
                        dc.SaveChanges();
                    }
                return RedirectToAction("Index");
            }
            else
            {
                return View(c.Email);
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            ContactModel c = null;
            c = GetContact(id);
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            using (MyContactBookEntities4 dc = new MyContactBookEntities4())
            {
               var email = dc.Emails.Where(a => a.ContactID==id).FirstOrDefault();
                if (email != null)
                {
                    dc.Emails.Remove(email);
                    dc.SaveChanges();
                   return RedirectToAction("Index");
                }
                var phone = dc.Phones.Where(a => a.ContactID==id).FirstOrDefault();
                if (phone != null)
                {
                    dc.Phones.Remove(phone);
                    dc.SaveChanges();
                     return RedirectToAction("Index");
                }
                var contact = dc.Contacts.Where(a => a.ContactID.Equals(id)).FirstOrDefault();
                if (contact != null)
                {
                    dc.Contacts.Remove(contact);
                    dc.SaveChanges();
                     return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound("Contact Not Found!");
                }
              //  return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Export()
        {
            List<ContactModel> allContacts = new List<ContactModel>();
            using (MyContactBookEntities4 dc = new MyContactBookEntities4())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Emails on a.ContactID equals b.ContactID
                         join c in dc.Phones on a.ContactID equals c.ContactID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             Firstname = a.Firstname,
                             Lastname = a.Lastname,
                             Email = b.email1,
                             Phone = c.phone1,
                             Address = a.Address
                         }).ToList();
                allContacts = v;
            }
            return View(allContacts);
        }

        [HttpPost]
        [ActionName("Export")]
        public FileResult ExportData()
        {
            List<ContactModel> allContacts = new List<ContactModel>();
            using (MyContactBookEntities4 dc = new MyContactBookEntities4())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Emails on a.ContactID equals b.ContactID
                         join c in dc.Phones on a.ContactID equals c.ContactID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             Firstname = a.Firstname,
                             Lastname = a.Lastname,
                             Email = b.email1,
                             Phone = c.phone1,
                             Address = a.Address
                         }).ToList();
                allContacts = v;
            }

            var grid = new WebGrid(source: allContacts, canPage: false, canSort: false);
            string exportData = grid.GetHtml(
                            columns: grid.Columns(
                                        grid.Column("ContactID", "Contact ID"),
                                        grid.Column("FirstName", "First Name"),
                                        grid.Column("LastName", "Last Name"),
                                        grid.Column("Email", "Email"),
                                        grid.Column("Phone", "Phone"),
                                        grid.Column("Address", "Address")
                                    )
                                ).ToHtmlString();
            return File(new System.Text.UTF8Encoding().GetBytes(exportData),
                    "application/vnd.ms-excel",
                    "Contacts.xls");

        }

        public ActionResult Search(string searching, string sln, string se, string sp, string sa)
        {
             List<ContactModel> contacts = new List<ContactModel>();
            using (MyContactBookEntities4 dc = new MyContactBookEntities4())
            {
                var v = (from a in dc.Contacts 
                         join b in dc.Emails on a.ContactID equals b.ContactID 
                         join c in dc.Phones on a.ContactID equals c.ContactID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             Firstname = a.Firstname,
                             Lastname = a.Lastname,
                             Email = b.email1,
                             Phone = c.phone1,
                             Address = a.Address
                         });
                if (!string.IsNullOrEmpty(searching))
                {
                    v = v.Where(a => a.Firstname.Contains(searching) || searching == null);
                }
                else if (!string.IsNullOrEmpty(sln))
                {
                    v = v.Where(a => a.Lastname.Contains(sln) || sln == null);
                }
                else if (!string.IsNullOrEmpty(se))
                {
                    v = v.Where(a => a.Email.Contains(se) || se == null);
                }
                else if (!string.IsNullOrEmpty(sp))
                {
                    v = v.Where(a => a.Phone.ToString().Contains(sp) || sp == null);
                }
                else if (!string.IsNullOrEmpty(sa))
                {
                    v = v.Where(a => a.Address.Contains(sa) || sa == null);
                }
                contacts = v.ToList();
            }
            return View(contacts);

        }

        [HttpPost]
        public FileResult ExportS(string searching, string sln, string se, string sp, string sa)
        {
            List<ContactModel> contacts = new List<ContactModel>();
            using (MyContactBookEntities4 dc = new MyContactBookEntities4())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Emails on a.ContactID equals b.ContactID
                         join c in dc.Phones on a.ContactID equals c.ContactID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             Firstname = a.Firstname,
                             Lastname = a.Lastname,
                             Email = b.email1,
                             Phone = c.phone1,
                             Address = a.Address
                         });
                if (!string.IsNullOrEmpty(searching))
                {
                    v = v.Where(a => a.Firstname.Contains(searching) || searching == null);
                }
                else if (!string.IsNullOrEmpty(sln))
                {
                    v = v.Where(a => a.Lastname.Contains(sln) || sln == null);
                }
                else if (!string.IsNullOrEmpty(se))
                {
                    v = v.Where(a => a.Email.Contains(se) || se == null);
                }
                else if (!string.IsNullOrEmpty(sp))
                {
                    v = v.Where(a => a.Phone.ToString().Contains(sp) || sp == null);
                }
                else if (!string.IsNullOrEmpty(sa))
                {
                    v = v.Where(a => a.Address.Contains(sa) || sa == null);
                }

                var grid = new WebGrid(source: v, canPage: false, canSort: false);
                string exportData = grid.GetHtml(
                                columns: grid.Columns(
                                            grid.Column("FirstName", "First Name"),
                                            grid.Column("LastName", "Last Name"),
                                            grid.Column("Email", "Email"),
                                            grid.Column("Phone", "Phone"),
                                            grid.Column("Address", "Address")
                                        )
                                    ).ToHtmlString();
                return File(new System.Text.UTF8Encoding().GetBytes(exportData),
                        "application/vnd.ms-excel",
                        "Contacts.xls");

            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult EditUser()
        {
            Table tablemodel = new Table();
            using (MyContactBookEntities3 list = new MyContactBookEntities3())
            {
                tablemodel.Rolecollection = list.Roles.ToList<Role>();
            }

            return View(tablemodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(Table c, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (MyContactBookEntities3 dc = new MyContactBookEntities3())
                {
                    var v = dc.Tables.Where(a => a.Id.Equals(c.Id)).FirstOrDefault();
                    if (v != null)
                    {
                        v.RoleId = c.RoleId;
                    }
                    dc.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(c);
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult AddUser()
        {
            Table tablemodel = new Table();
            using (MyContactBookEntities3 list = new MyContactBookEntities3())
            {
                tablemodel.Rolecollection = list.Roles.ToList<Role>();
            }

            return View(tablemodel);
        }

        private static string CreatePasswordHash(string password)
        {
            string passwrodSalt = String.Concat(password);
            string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(passwrodSalt, "SHA1");
            return hashedPwd;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(Table c, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (MyContactBookEntities3 dc = new MyContactBookEntities3())
                {
                    c.Password = CreatePasswordHash(c.Password);
                    dc.Tables.Add(c);
                    dc.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(c);
            }
        }
    }
}

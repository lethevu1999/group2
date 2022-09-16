using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    public class DocGiaController : Controller
    {
        QLTVEntities db = new QLTVEntities();
        static string ErrorTrung = "false";
        public List<tblDocGia> listDocGia()
        {
            return db.tblDocGias.ToList();
        }

        public bool KiemTraTrung(string madocgia, int? xoa = 0)
        {
            int kt = 0;
            if (xoa == 0)
            {
                kt = db.tblDocGias.Where(x => x.madocgia == madocgia).Count();
            }
            else
            {
                int sothe = db.tblDocGias.Where(x => x.madocgia == madocgia).Select(x => x.sothe).FirstOrDefault();
                kt = db.tblMuonTras.Where(x => x.sothe == sothe).Count();
            }
            if (kt > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else
            {
                ViewBag.ErrorTrung = ErrorTrung;
                ErrorTrung = "false";
                return View(listDocGia());
            }    
            
        }

        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else
            {
                tblDocGia DocGia = new tblDocGia();
                return View(DocGia);
            }    
        }

        [HttpPost]

        public ActionResult Create(tblDocGia DocGia)
        {
            if (ModelState.IsValid)
            {
                if (KiemTraTrung(DocGia.madocgia))
                {
                    ViewBag.ErrorTrung = "true";
                    return View();
                }
                else if (DocGia.ngaysinh > DateTime.Now.Date)
                {
                    ViewBag.ErrorTrung = "false";
                    ViewBag.ErrorDate = "true";
                    return View();
                }    
                else
                {
                    ViewBag.ErrorDate = "false";
                    tblTheThuVien TheThuVien = new tblTheThuVien();
                    TheThuVien.ngaybatdau = DateTime.Now;
                    TheThuVien.ngayhethan = DateTime.Now.AddMonths(1);
                    db.tblTheThuViens.Add(TheThuVien);
                    DocGia.sothe = db.tblTheThuViens.Max(x => x.sothe);
                    db.tblDocGias.Add(DocGia);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }    
            else
            {
                return View();
            }
        }

        public ActionResult Edit(string madocgia)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else
            {
                tblDocGia DocGia = new tblDocGia();
                DocGia = db.tblDocGias.Where(x => x.madocgia == madocgia).FirstOrDefault();
                return View(DocGia);
            }    
        }

        [HttpPost]

        public ActionResult Edit(tblDocGia DocGia)
        {
            if (ModelState.IsValid)
            {
                if (DocGia.ngaysinh > DateTime.Now.Date)
                {
                    ViewBag.ErrorDate = "true";
                    return View();
                }
                else
                {
                    ViewBag.ErrorDate = "false";
                    tblDocGia preDocGia = new tblDocGia();
                    preDocGia = db.tblDocGias.Where(x => x.madocgia == DocGia.madocgia).FirstOrDefault();
                    preDocGia.hotendg = DocGia.hotendg;
                    preDocGia.ngaysinh = DocGia.ngaysinh;
                    preDocGia.gioitinh = DocGia.gioitinh;
                    preDocGia.sdt = DocGia.sdt;
                    preDocGia.email = DocGia.email;
                    preDocGia.diachi = DocGia.diachi;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }    
            }
            else
            {
                return View();
            }
        }

        public ActionResult Delete(string madocgia)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else
            {
                tblDocGia DocGia = db.tblDocGias.Where(x => x.madocgia == madocgia).FirstOrDefault();
                if (KiemTraTrung(madocgia, 1))
                {
                    ErrorTrung = "true";
                }
                else
                {
                    ErrorTrung = "false";
                    db.tblDocGias.Remove(db.tblDocGias.Where(x => x.madocgia == madocgia).FirstOrDefault());
                    db.tblTheThuViens.Remove(db.tblTheThuViens.Where(x => x.sothe == DocGia.sothe).FirstOrDefault());
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }
    }
}
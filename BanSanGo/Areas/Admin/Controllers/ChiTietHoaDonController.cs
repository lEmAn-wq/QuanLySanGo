using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BanSanGo.Models;

namespace BanSanGo.Areas.Admin.Controllers
{
    public class ChiTietHoaDonController : Controller
    {
        private readonly QuanLySanGoEntities db = new QuanLySanGoEntities();

        // GET: Admin/ChiTietHoaDon
        public ActionResult Index(int id)
        {
            ViewBag.id = id;
            var chiTietHoaDons = db.ChiTietHoaDons
                                    .Include(c => c.HoaDon)
                                    .Include(c => c.SanGo)
                                    .Where(c => c.MaHD == id)
                                    .ToList();
            return View(chiTietHoaDons);
        }

        // GET: Admin/ChiTietHoaDon/Create
        public ActionResult Create(int id)
        {
            var sanGoes = db.SanGoes.ToList();
            ViewBag.MaHD = id;
            ViewBag.MaSanGo = new SelectList(sanGoes, "MaSanGo", "TenSanGo");

            var giaSanGo = sanGoes.ToDictionary(s => s.MaSanGo.ToString(), s => s.GiaBan);
            ViewBag.GiaSanGo = giaSanGo;

            return View();
        }

        // POST: Admin/ChiTietHoaDon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaCTHD,MaHD,MaSanGo,SoLuongMua,GiaMua")] ChiTietHoaDon chiTietHoaDon)
        {
            if (ModelState.IsValid)
            {
                db.ChiTietHoaDons.Add(chiTietHoaDon);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = chiTietHoaDon.MaHD });
            }

            var sanGoes = db.SanGoes.ToList();
            ViewBag.MaHD = chiTietHoaDon.MaHD;
            ViewBag.MaSanGo = new SelectList(sanGoes, "MaSanGo", "TenSanGo", chiTietHoaDon.MaSanGo);
            var giaSanGo = sanGoes.ToDictionary(s => s.MaSanGo.ToString(), s => s.GiaBan);
            ViewBag.GiaSanGo = giaSanGo;
            return View(chiTietHoaDon);
        }

        // GET: Admin/ChiTietHoaDon/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietHoaDon chiTietHoaDon = db.ChiTietHoaDons.Find(id);
            if (chiTietHoaDon == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaHD = chiTietHoaDon.MaHD;
            ViewBag.MaSanGo = new SelectList(db.SanGoes, "MaSanGo", "TenSanGo", chiTietHoaDon.MaSanGo);
            return View(chiTietHoaDon);
        }

        // POST: Admin/ChiTietHoaDon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaCTHD,MaHD,MaSanGo,SoLuongMua,GiaMua")] ChiTietHoaDon chiTietHoaDon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var sanGo = db.SanGoes.Find(chiTietHoaDon.MaSanGo);
                    if (sanGo == null)
                    {
                        ModelState.AddModelError("", "Sàn gỗ không tồn tại.");
                        return View(chiTietHoaDon);
                    }

                    var soLuongChiTietHienTai = db.ChiTietHoaDons.AsNoTracking().FirstOrDefault(c => c.MaCTHD == chiTietHoaDon.MaCTHD)?.SoLuongMua ?? 0;
                    var soLuongMuaChenhLech = chiTietHoaDon.SoLuongMua - soLuongChiTietHienTai;

                    if (sanGo.SoLuong < soLuongMuaChenhLech)
                    {
                        ModelState.AddModelError("", "Số lượng gỗ không đủ để thực hiện chỉnh sửa.");
                        ViewBag.MaSanGo = new SelectList(db.SanGoes, "MaSanGo", "TenSanGo", chiTietHoaDon.MaSanGo);
                        ViewBag.MaHD = chiTietHoaDon.MaHD;
                        return View(chiTietHoaDon);
                    }

                    db.Entry(chiTietHoaDon).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = chiTietHoaDon.MaHD });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ex.Entries.Single().Reload();
                    ModelState.AddModelError("", "Dữ liệu đã thay đổi kể từ khi bạn lấy dữ liệu. Vui lòng thử lại.");
                }
            }

            ViewBag.MaSanGo = new SelectList(db.SanGoes, "MaSanGo", "TenSanGo", chiTietHoaDon.MaSanGo);
            ViewBag.MaHD = chiTietHoaDon.MaHD;
            return View(chiTietHoaDon);
        }

        // GET: Admin/ChiTietHoaDon/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietHoaDon chiTietHoaDon = db.ChiTietHoaDons.Find(id);
            if (chiTietHoaDon == null)
            {
                return HttpNotFound();
            }
            return View(chiTietHoaDon);
        }

        // POST: Admin/ChiTietHoaDon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChiTietHoaDon chiTietHoaDon = db.ChiTietHoaDons.Find(id);
            db.ChiTietHoaDons.Remove(chiTietHoaDon);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = chiTietHoaDon.MaHD });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

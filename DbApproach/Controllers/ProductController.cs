using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbApproach.Models;

namespace DbApproach.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(string search, string sortColumn = "ProductName", string IconClass = "bi-sort-alpha-down", string page = "1")
        {
            int currPage = Convert.ToInt32(page);
            int dataPerPage = 5;
            int offset = (currPage - 1) * dataPerPage;
            ViewBag.url = Request.Url;

            var db = new dbEntities();
            var brand = db.Brands.ToList();
            var cat = db.Categories.ToList();
            ViewBag.brand = brand;
            ViewBag.cat = cat;

            List<Product> products;
            if (search != null)
            {
                products = db.Products.Where(tmp => tmp.ProductName.Contains(search)).ToList();
            }
            else
            {
                products = db.Products.ToList();

            }

            ViewBag.SortColumn = sortColumn;
            ViewBag.IconClass = IconClass;

            if (ViewBag.SortColumn == "ProductName")
            {
                if (ViewBag.IconClass == "bi-sort-alpha-up")
                {
                    products = products.OrderBy(tmp => tmp.ProductName).ToList();
                }
                else
                {
                    products = products.OrderByDescending(tmp => tmp.ProductName).ToList();

                }
            }
            products = products.Skip(offset).Take(dataPerPage).ToList();

            return View(products);
        }

        public ActionResult Input()
        {
            var db = new dbEntities();
            var brand = db.Brands.ToList();
            var cat = db.Categories.ToList();
            ViewBag.brand = brand;
            ViewBag.cat = cat;
            return View();
        }

        [HttpPost]
        public ActionResult Input(Product product)
        {
            var db = new dbEntities();
            if (product.Active != 1)
            {
                product.Active = 0;
            }
            if (Request.Files.Count >= 1)
            {
                var file = Request.Files[0];
                var imgByte = new Byte[file.ContentLength - 1];
                file.InputStream.Read(imgByte, 0, file.ContentLength);
                var imgStr = Convert.ToBase64String(imgByte, 0, imgByte.Length);
                product.Image = imgStr;
            }
            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            var db = new dbEntities();
            var brand = db.Brands.ToList();
            var cat = db.Categories.ToList();
            ViewBag.brand = brand;
            ViewBag.cat = cat;
            if (id != null)
            {
                var product = db.Products.Where(tmp => tmp.ProductId.Equals(id)).FirstOrDefault();
                return View(product);
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public ActionResult Edit(Product input)
        {
            var db = new dbEntities();
            Product existProd = db.Products.Where(tmp => tmp.ProductId.Equals(input.ProductId)).FirstOrDefault();
            existProd.ProductName = input.ProductName;
            existProd.DateOfPurchase = input.DateOfPurchase;
            existProd.AvailableStatus = input.AvailableStatus;
            existProd.CategoryId = input.CategoryId;
            existProd.BrandId = input.BrandId;
            existProd.Price = input.Price;
            if (input.Active != 1)
            {
                existProd.Active = 0;
            }
            else
            {
                existProd.Active = input.Active;
            }
            db.SaveChanges();

            return RedirectToAction("index");
        }

        public ActionResult Delete(string id)
        {
            var db = new dbEntities();
            Product existProd = db.Products.Where(tmp => tmp.ProductId.Equals(id)).FirstOrDefault();
            db.Products.Remove(existProd);
            db.SaveChanges();

            return RedirectToAction("index");
        }
    }
}


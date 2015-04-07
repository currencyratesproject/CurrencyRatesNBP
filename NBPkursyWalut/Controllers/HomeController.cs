using NBPkursyWalut.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using NBPkursyWalut.Models.Buissness;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Globalization;

namespace NBPkursyWalut.Controllers
{
    public class HomeController : Controller
    {



        public async Task<ActionResult> Index()
        {

            try
            {
                bool isEmpty = true;

                EntityDbContext db = new EntityDbContext();
            
                if (db.Postions.FirstOrDefault() != null) { isEmpty = false; }

                #region if database has not any record of Positions
                if (isEmpty)
                {
                    //Download data from specified urls
                    CurrencyDownload currencyDownload = new CurrencyDownload();

                    List<Dir> dirs = await currencyDownload.DirTxt(new Uri("http://www.nbp.pl/kursy/xml/dir.txt"), 070101, 150101);


                    List<RatesTableDay> ratesTableDay = new List<RatesTableDay>();

                    foreach (var dir in dirs)
                    {
                        ratesTableDay.Add(await currencyDownload.GetCurrency(new Uri("http://www.nbp.pl/kursy/xml/" + dir.DirNumber + ".xml")));
                    }


                    //Adding downloaded data to database
                    using (EntityDbContext dbContext = new EntityDbContext())
                    {
                        if (dbContext.Dirs.Count() == 0)
                        {
                            dbContext.Dirs.AddRange(dirs);
                            dbContext.SaveChanges();
                        }


                        for (int i = 0; i < ratesTableDay.Count; i++)
                        {
                            foreach (var element in ratesTableDay[i].Positions)
                            {

                                dbContext.Postions.Add(new Position
                                {
                                    Date = Convert.ToDateTime(ratesTableDay[i].PublicationDate),
                                    CurrencyName = element.CurrencyName,
                                    CurrencyConversion = element.CurrencyConversion,
                                    CurrencyCode = element.CurrencyCode,
                                    Average = element.Average
                                });
                            }



                        }
                        dbContext.SaveChanges();

                    }


                }

                #endregion


                var uniqueCurrencyNames = db.Postions.Select(x => x.CurrencyName).Distinct().ToList();

                List<Position> rates = new List<Position>();

                foreach (var nameCurrency in uniqueCurrencyNames)
                {
                    ViewData[nameCurrency + "Max"] = db.Postions.Where(x => x.CurrencyName == nameCurrency).Max(x => x.Average).ToString("F4");
                    ViewData[nameCurrency + "Min"] = db.Postions.Where(x => x.CurrencyName == nameCurrency).Min(x => x.Average).ToString("F4");
                    ViewData[nameCurrency + "Avg"] = db.Postions.Where(x => x.CurrencyName == nameCurrency).OrderBy(x => x.Average).Average(x => x.Average).ToString("F4");
                    rates.Add(db.Postions.Where(x => x.CurrencyName == nameCurrency).First());
                }


                return View(rates.OrderBy(x => x.CurrencyName).ToList());

            }
            catch (Exception)
            {
                return RedirectToAction("Exception", "Error");

            }


        }




        public ActionResult Currency()
        {
            var code = RouteData.Values["id"];

            List<Position> positions = new List<Position>();

            try
            {

                if (code != null)
                {
                    using (EntityDbContext db = new EntityDbContext())
                    {
                        positions = db.Postions.Where(x => x.CurrencyCode == code).ToList();

                        if (positions.Count > 0)
                        {
                            ViewBag.Code = code;

                            var dates = db.Postions.Where(x => x.CurrencyCode == code).Select(x => x.Date).Distinct().ToList();
                            List<String> datesString = new List<String>();

                            foreach (var item in dates)
                            {
                                datesString.Add(item.Date.ToString("yyyy-MM-dd"));
                            }


                            ViewBag.DatesDsc = datesString.OrderByDescending(x => x).ToList();
                            ViewBag.DatesAsc = datesString.OrderBy(x => x).ToList();



                            return View(positions.OrderByDescending(x => x.Date).ToList());
                        }


                    }
                }

                return RedirectToAction("PageNotFound", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("Exception", "Error");
            }

        }










        [HttpPost]
        public ActionResult Currency(string from, string to)
        {

            var code = RouteData.Values["id"];
            try
            {
                #region Getting all dates from db for dropdown-lists
                using (EntityDbContext dbContext = new EntityDbContext())
                {

                    var dates = dbContext.Postions.Where(x => x.CurrencyCode == code).Select(x => x.Date).Distinct().ToList();
                    List<String> datesString = new List<String>();

                    foreach (var item in dates)
                    {
                        datesString.Add(item.Date.ToString("yyyy-MM-dd"));
                    }


                    ViewBag.DatesDsc = datesString.OrderByDescending(x => x).ToList();
                    ViewBag.DatesAsc = datesString.OrderBy(x => x).ToList();
                }
                #endregion

                DateTime fromDate = new DateTime();
                DateTime toDate = new DateTime();

                DateTime.TryParse(from, out fromDate);
                DateTime.TryParse(to, out toDate);

                if (fromDate.Date >= toDate.Date)
                {
                    ModelState.AddModelError("To", "Date FROM can't be higher or equal to date TO");
                    ViewBag.Code = null;
                    return PartialView("_Table");
                }


                List<Position> positions = new List<Position>();


                if (code != null)
                {
                    using (EntityDbContext db = new EntityDbContext())
                    {
                        var temp = db.Postions.Where(x => x.CurrencyCode == code).ToList();


                        if (temp.Count > 0)
                        {
                            foreach (var item in temp)
                            {
                                if (item.Date.Date >= fromDate && item.Date.Date <= toDate.Date)
                                    positions.Add(item);
                            }


                            if (positions.Count > 0)
                            {


                                ViewBag.Code = code;

                                return PartialView("_Table", positions.OrderByDescending(x => x.Date).ToList());

                            }


                        }

                    }


                }

                return RedirectToAction("PageNotFound", "Error");


            }
            catch (Exception)
            {

                return RedirectToAction("Exception", "Error");
            }


        }









        public JsonResult GetAverages(string from, string to)
        {
            var code = RouteData.Values["id"];


            List<double> averages = new List<double>();
            List<String> dates = new List<String>();
            try
            {


                if (code != null)
                {

                    using (EntityDbContext db = new EntityDbContext())
                    {

                        //If date range has been changed
                        if (from != null && to != null)
                        {
                            var temp = db.Postions.Where(x => x.CurrencyCode == code).ToList();

                            DateTime dateFrom = new DateTime();
                            DateTime dateTo = new DateTime();

                            DateTime.TryParse(from, out dateFrom);
                            DateTime.TryParse(to, out dateTo);


                            for (int i = 0; i < temp.Count; i++)
                            {
                                if (temp[i].Date.Date >= dateFrom.Date && temp[i].Date.Date <= dateTo.Date)
                                {
                                    averages.Add(temp[i].Average);
                                    dates.Add(temp[i].Date.ToString("yyyy-MM-dd"));
                                }

                            }
                        }
                        //On document load where all dates are required
                        else
                        {
                            averages = db.Postions.Where(x => x.CurrencyCode == code).Select(x => x.Average).ToList();
                            if (averages.Count > 0)
                            {
                                var tempData = db.Postions.Where(x => x.CurrencyCode == code).Select(x => x.Date).ToList();
                                for (int i = 0; i < tempData.Count; i++)
                                {
                                    dates.Add(tempData[i].ToString("yyyy-MM-dd"));
                                }
                            }

                        }

                        return Json(new { data = averages, x = dates }, JsonRequestBehavior.AllowGet);



                    }
                }
                //if in some case, there was no id in route, it sends back null
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Error = "Błąd połączenia" }, JsonRequestBehavior.AllowGet);
            }



        }

        public PartialViewResult _Table()
        {
            return PartialView();
        }




    }



}






















































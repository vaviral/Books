using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoogleLogin.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;

namespace GoogleLogin.Controllers
{
    public class BooksController : Controller
    {
        private string API_Key;
        string FilePath;

        public BooksController()
        {
            FilePath = ConfigurationManager.AppSettings["FavouritesFilepath"].ToString();
            API_Key = ConfigurationManager.AppSettings["GoogleApiKey"].ToString();
            //Log = LogManager.GetLogger(typeof(BooksController));
        }
        private List<T> Read<T>()
        {
            try
            {
                var serializer = new JsonSerializer();
                List<T> List = new List<T>();
                using (var file = System.IO.File.OpenText(FilePath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        List = serializer.Deserialize<List<T>>(reader);
                    }
                }
                return List;
            }
            catch
            {
                throw;
            }
        }
        private int Write<T>(List<T> List)
        {
            try
            {
                var serializer = new JsonSerializer();
                using (StreamWriter file = System.IO.File.CreateText(FilePath))
                {
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        serializer.Serialize(writer, List);
                    }
                }
                return 1;
            }
            catch
            {
                throw;
            }
        }
        private Result GenerateResponse(int Output, string ErrorMessage)
        {
            Result response = new Result();
            switch (Output)
            {
                case 0:
                    response.Status = 0;
                    response.Message = "Failed";
                    if (ErrorMessage != null)
                        response.ErrorMessage = ErrorMessage;
                    break;
                case 1:
                    response.Status = 1;
                    response.Message = "Succeeded";
                    break;
            }
            return response;
        }
        private BookDetails SearchBook(string ISBN)
        {
            using (var client = new HttpClient())
            {
                string Url = "https://www.googleapis.com/books/v1/volumes?q=" + ISBN.ToLower() + "&key=" + API_Key;
                client.BaseAddress = new Uri(Url);
                var result = client.GetAsync(Url).Result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<GoogleBooksApiResult>(result);
                //Log.Info("Status= " + response.Status + "Message=" + response.Message);
                return response.Items[0];
            }
        }
        private int AddToFav(string ISBN,Favourites model,List<Favourites> List)
        {
            BookDetails Details = SearchBook(ISBN);
            if (List == null)
            {
                model.UserId = Session["Username"].ToString();
                model.LinkForThumbnail.Add(Details.VolumeInfo.ImageLinks.SmallThumbnail);
                model.Publisher.Add(Details.VolumeInfo.Publisher);
                model.NameOfBook.Add(Details.VolumeInfo.Title);
                model.IsbnOfBook.Add("ISBN:" + Details.VolumeInfo.IndustryIdentifiers[0].Identifier);
                model.Authors.Add(Details.VolumeInfo.Authors.ToList());
                model.Category.Add(Details.VolumeInfo.Categories.ToList());
                List = new List<Favourites>();
                List.Add(model);
                return Write(List);
            }
            else if (List.Exists(e => e.UserId.Equals(Session["Username"].ToString()) && !e.IsbnOfBook.Contains(ISBN)))
            {
                foreach(var item in List)
                {
                    if(item.UserId.Equals(Session["Username"].ToString()) && !item.IsbnOfBook.Contains(ISBN))
                    {
                        item.LinkForThumbnail.Add(Details.VolumeInfo.ImageLinks.SmallThumbnail);
                        item.Publisher.Add(Details.VolumeInfo.Publisher);
                        item.NameOfBook.Add(Details.VolumeInfo.Title);
                        item.IsbnOfBook.Add(ISBN);
                        item.Authors.Add(Details.VolumeInfo.Authors.ToList());
                        item.Category.Add(Details.VolumeInfo.Categories.ToList());
                        return Write(List);
                    }
                }
            }
            return 2;
        }
        private bool IfFavouritesIsNull()
        {
            if (Read<Favourites>() == null)
                return true;
            else
                return false;
        }
        private Favourites GenerateModel()
        {
            Favourites model = new Favourites();
            model.LinkForThumbnail = new List<string>();
            model.Publisher = new List<string>();
            model.NameOfBook = new List<string>();
            model.IsbnOfBook = new List<string>();
            model.Authors = new List<List<string>>();
            model.Category = new List<List<string>>();
            return model;
        }
        private bool CheckIfFavourite(string ISBN)
        {
            try
            {
                var serializer = new JsonSerializer();
                List<Favourites> List = new List<Favourites>();
                using (var file = System.IO.File.OpenText(FilePath))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    List = serializer.Deserialize<List<Favourites>>(reader);
                }
                if (List == null)
                {
                    return false;
                }
                else
                {
                    foreach (var items in List)
                    {
                        if (items.UserId == Session["Username"].ToString())
                        {
                            if (items.IsbnOfBook.Contains(ISBN))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult SearchBooks(string BookName)
        {
            if (Session["Username"] != null)
            {
                using (var client = new HttpClient())
                {
                    string Url = "https://www.googleapis.com/books/v1/volumes?q=" + BookName + "&key=" + API_Key;
                    client.BaseAddress = new Uri(Url);
                    var result = client.GetAsync(Url).Result.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<GoogleBooksApiResult>(result);
                    //Log.Info("Status= " + response.Status + "Message=" + response.Message);
                    return View(response);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult ViewBook(string ISBN)
        {
            if (Session["Username"] != null)
            {
                ViewBag.ISBN = ISBN;
                ViewBag.IsFavourite = CheckIfFavourite(ISBN);
                return View(SearchBook(ISBN));
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult AddToFavourites(string ISBN, string BookName)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    if (IfFavouritesIsNull())
                    {
                        return Json(GenerateResponse(AddToFav(ISBN, GenerateModel(), null),null), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(GenerateResponse(AddToFav(ISBN, GenerateModel(), Read<Favourites>()),null), JsonRequestBehavior.AllowGet);
                    }
                }
                catch(Exception e)
                {
                    return Json(GenerateResponse(0,"Error Occured"), JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult DeleteFromFavourites(string ISBN, string BookName)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    if (CheckIfFavourite(ISBN))
                    {
                        List<Favourites> List = Read<Favourites>();
                        foreach (var items in List)
                        {
                            if (items.UserId.Equals(Session["Username"].ToString()))
                            {
                                foreach (var item in items.IsbnOfBook)
                                {
                                    if (item.Equals(ISBN))
                                    {
                                        BookDetails Details = SearchBook(ISBN);
                                        items.Authors.Remove(Details.VolumeInfo.Authors.ToList());
                                        items.Category.Remove(Details.VolumeInfo.Categories.ToList());
                                        items.IsbnOfBook.Remove(item);
                                        items.NameOfBook.Remove(BookName);
                                        items.Publisher.Remove(Details.VolumeInfo.Publisher);
                                        items.LinkForThumbnail.Remove(Details.VolumeInfo.ImageLinks.SmallThumbnail);
                                        return Json(GenerateResponse(Write(List), null), JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                        }
                        return Json(GenerateResponse(0,null), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(GenerateResponse(2, "Its not your favourite book"), JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception e)
                {
                    return Json(GenerateResponse(2, "Error Occured"), JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ManageFavourites()
        {
            if (Session["Username"] != null)
            {
                string Email = Session["Username"].ToString();
                Favourites response = GenerateModel();
                List<Favourites> List_Retrieved = Read<Favourites>();
                if (List_Retrieved != null)
                {
                    foreach (var item in List_Retrieved)
                    {
                        if (item.UserId.Equals(Email))
                        {
                            for (int i = 0; i < item.NameOfBook.Count(); i++)
                            {
                                response.NameOfBook.Add(item.NameOfBook[i]);
                                response.IsbnOfBook.Add(item.IsbnOfBook[i]);
                                response.Authors.Add(item.Authors[i]);
                                response.Category.Add(item.Category[i]);
                                response.Publisher.Add(item.Publisher[i]);
                                response.LinkForThumbnail.Add(item.LinkForThumbnail[i]);
                            }
                            break;
                        }
                    }
                    //ViewBag.ListOfFavouriteBooks = List_To_Return;
                    return View(response);
                }
                else
                    return View(response);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using project_download.Data;
using System.IO;

namespace project_download.Controllers;

public class FileUploadController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult DeleteFile(string fileName, string password)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", fileName);
        var file = FileDataManager.GetFile(fileName);
        if(BCrypt.Net.BCrypt.Verify(password, file.PasswordHash))
        {
            return RedirectToAction("DeleteFile");
        } 

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            //return Content($"file <{fileName}> has deleted.");
            return RedirectToAction("Downloads");
        }
        else
        {
            return RedirectToAction("DeleteFile");
            //return Content($"IMPORTANT:\nfile <{fileName}> hasn't deleted.");
        }
    }
    [HttpGet]
    public IActionResult DeleteFile(string fileName){
        return View("DeleteFile", fileName);
    }
    public IActionResult Upload(IFormFile file, string password)
    {
        // альтернативный вариант загрузки через HttpContext
        //var files = HttpContext.Request.Form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return Content("Файл не выбран");
        }

        string curDir = Directory.GetCurrentDirectory();
        var uploadsFolder = Path.Combine(curDir, "UploadedFiles");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var filePath = Path.Combine(uploadsFolder, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        // работа с паролем
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        // FileRepository.Files.Add(new Models.FileData()
        // {
        //     FileName = file.FileName,
        //     PasswordHash = passwordHash
        // });
        FileDataManager.AddFile(new Models.FileData()
        {
            FileName = file.FileName,
            PasswordHash = passwordHash
        });

        // return Content($"file <{file.FileName}> has downloaded! {passwordHash}");
        return RedirectToAction("Downloads");
    }

    [HttpPost]
    public IActionResult DownloadFile(string fileName, string password)
    {
        // достаем dileData по имени файла
        // var fileData = FileRepository.Files.FirstOrDefault(x => x.FileName == fileName);
        var fileData = FileDataManager.GetFile(fileName);

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (fileData == null)
        {
            return Content("Файл не найден");
        }

        // check password
        if (BCrypt.Net.BCrypt.Verify(password, fileData.PasswordHash) == false)
        {
            return Content("Неверный пароль");
        }

        byte[] bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "application/octet-stream", fileName);
    }
    public IActionResult DownLoadFile(string fileName)
    {
        // ViewName(ViewName, object)
        return View("DownLoadFile", fileName);
    }
    public IActionResult DownLoads()
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        if (!Directory.Exists(uploadsFolder))
        {
            return Content("Error. Directory not found. Directory doesn't exists");
        }
        var files = Directory.GetFiles(uploadsFolder).
        Select(file => new FileInfo(file)).
        Select(file => new
        {
            Name = file.Name,
            Length = $"{(file.Length / 1024f / 1024f):F2} Мб"
        }).ToList();

        return View(files);
    }
}
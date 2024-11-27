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
    public IActionResult DeleteFile(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", fileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            return Content($"file <{fileName}> has deleted.");
        }
        else
        {
            return Content($"IMPORTANT:\nfile <{fileName}> hasn't deleted.");
        }

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
        FileRepository.Files.Add(new Models.FileData(){
            FileName=file.FileName,
            PasswordHash = passwordHash
        });

        return Content($"file <{file.FileName}> has downloaded! {passwordHash}");
    }

    [HttpPost]
    public IActionResult DownloadFile(string fileName, string password)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return Content("Файл не найден");
        }
        else
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", fileName);
        }
    }
    public IActionResult DownLoadFile(string fileName){

        // ViewName(ViewName, object)
        return View("DownLoadFile", fileName);
    }
    public IActionResult DownLoads()
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        if(!Directory.Exists(uploadsFolder)){
            return Content("Error. Directory not found. Directory doesn't exists");
        }
        var files = Directory.GetFiles(uploadsFolder).
        Select(file => new FileInfo(file)).
        Select(file => new{
            Name = file.Name,
            Length = $"{(file.Length / 1024f / 1024f):F2} Мб"
        }).ToList();

        return View(files);
    }
}
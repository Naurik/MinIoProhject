using Microsoft.AspNetCore.Mvc;

namespace MinIoClient.Model
{
    public abstract class FileResult : IActionResult
    {
        //
        // Сводка:
        //     Initializes a new instance of the System.Web.Mvc.FileResult class.
        //
        // Параметры:
        //   contentType:
        //     The type of the content.
        //
        // Исключения:
        //   T:System.ArgumentException:
        //     The contentType parameter is null or empty.
        protected FileResult(string contentType) { }

        //
        // Сводка:
        //     Gets the content type to use for the response.
        //
        // Возврат:
        //     The type of the content.
        public string ContentType { get; }
        //
        // Сводка:
        //     Gets or sets the content-disposition header so that a file-download dialog box
        //     is displayed in the browser with the specified file name.
        //
        // Возврат:
        //     The name of the file.
        public string FileDownloadName { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}

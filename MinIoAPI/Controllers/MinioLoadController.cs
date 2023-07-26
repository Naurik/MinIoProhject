using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel;
using MinIoClient.Model;
using System.Net;
using System.Text;

namespace MinIoClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinioLoadController : ControllerBase
    {
        public MinioLoadController() { }

        //Save Files in MinIo
        #region SaveFile
        [HttpPost]
        [Route("loadfiles")]
        public async Task<IActionResult> SaveFileInMinio(MinioModel model)
        {
            try
            {
                await FileLoadInMinio(model);

                return Ok();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static async Task FileLoadInMinio(MinioModel model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                return true;
            };
            var endpoint = "storage-api.kuis.kz:9000";
            var accessKey = "SKZD0lXUhlUrv2Yp6YVG";
            var secretKey = "zQUUb0pFZFnkOQakrKV0t5IgGdi2Gf0rKomdQN39";

            try
            {
                using var minio = new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .WithSSL(false)
                    .Build();
                await LoadRun(minio, model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static async Task LoadRun(IMinioClient minio, MinioModel model)
        {
            // Make a new bucket called mymusic.
            var bucketName = model.BucketName;
            // Upload the zip file
            var objectName = model.ObjectName;
            // The following is a source file that needs to be created in
            // your local filesystem.
            var filePath = model.FilePath;
            var contentType = model.ContentType;

            try
            {
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minio.BucketExistsAsync(bktExistArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mkBktArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minio.MakeBucketAsync(mkBktArgs).ConfigureAwait(false);
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType(contentType);
                await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        //UpLoad Files from MinIo
        #region UploadFile
        [HttpPost]
        [Route("uploadfiles")]
        public async Task<FileResponse> GetFilesFromMinio(MinioModel model)
        {
            try
            {
                var result = await FileUploadFromMinio(model);
                System.IO.File.Delete(result.FilePath);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static async Task<FileResponse> FileUploadFromMinio(MinioModel model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                return true;
            };
            var endpoint = "storage-api.kuis.kz:9000";
            var accessKey = "SKZD0lXUhlUrv2Yp6YVG";
            var secretKey = "zQUUb0pFZFnkOQakrKV0t5IgGdi2Gf0rKomdQN39";

            try
            {
                using var minio = new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .WithSSL(false)
                    .Build();
                return await UploadRun(minio, model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static async Task<FileResponse> UploadRun(IMinioClient minio, MinioModel model)
        {
            // Make a new bucket called mymusic.
            var bucketName = model.BucketName;
            // Upload the zip file
            var objectName = model.ObjectName;

            FileResponse fileResponse = new FileResponse(); ;
            try
            {
                Console.WriteLine("Running API: GetObjectAsync");
                // Check whether the object exists using StatObjectAsync(). If the object is not found,
                // StatObjectAsync() will throw an exception.
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);
                await minio.StatObjectAsync(statObjectArgs).ConfigureAwait(false);

                // Get object content starting at byte position 1024 and length of 4096
                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithOffsetAndLength(1024L, 4096L)
                    .WithCallbackStream(async (stream, cancellationToken) =>
                    {
                        var fileStream = System.IO.File.Create(objectName);
                        await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                        await fileStream.DisposeAsync().ConfigureAwait(false);
                        var writtenInfo = new FileInfo(objectName);
                        var file_read_size = writtenInfo.Length;
                        // Uncomment to print the file on output console
                        stream.CopyTo(Console.OpenStandardOutput());
                        fileResponse.FilePath = fileStream.Name;

                        stream.Dispose();
                    });
                var reponse =  await minio.GetObjectAsync(getObjectArgs).ConfigureAwait(false);

                using (FileStream fstream = System.IO.File.OpenRead(fileResponse.FilePath))
                {
                    // выделяем массив для считывания данных из файла
                    byte[] buffer = new byte[fstream.Length];
                    // считываем данные
                    fstream.ReadAsync(buffer, 0, buffer.Length);
                    // декодируем байты в строку
                    string textFromFile = Encoding.Default.GetString(buffer);

                    fileResponse.FileName = model.ObjectName;

                    fileResponse.FileDATA = new byte[buffer.Length];
                }

                return fileResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
    }
}

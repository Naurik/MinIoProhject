using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel;
using MinIoClient.Model;
using System.Net;
using System.Net.Security;

namespace MinIoClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinioLoadController : ControllerBase
    {
        public MinioLoadController() { }

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

        [HttpPost]
        [Route("uploadfiles")]
        public async Task<ObjectStat> GetFilesFromMinio(MinioModel model)
        {
            try
            {
               return await FileUploadFromMinio(model);
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
            var accessKey = "mIph30XEe5SlNekUmvru";
            var secretKey = "y2nlz2q5ruUB4e2iVDSl7mDey6PKJquSxq3Ewxub";

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

        private static async Task<ObjectStat> FileUploadFromMinio(MinioModel model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                return true;
            };
            var endpoint = "storage-api.kuis.kz:9000";
            var accessKey = "mIph30XEe5SlNekUmvru";
            var secretKey = "y2nlz2q5ruUB4e2iVDSl7mDey6PKJquSxq3Ewxub";

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

        private static async Task LoadRun(MinioClient minio, MinioModel model)
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

        private static async Task<ObjectStat> UploadRun(MinioClient minio, MinioModel model)
        {
            // Make a new bucket called mymusic.
            var bucketName = model.BucketName;
            // Upload the zip file
            var objectName = model.ObjectName;

            try
            {
                var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
                var stat = await minio.GetObjectAsync(args).ConfigureAwait(false);
                return stat;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

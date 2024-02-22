using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace RemoteStorage.DAL
{
    static class Repository
    {
        private const string containerName = "blobcontainer";
        private static readonly string cs =
            ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        private static readonly Lazy<BlobContainerClient> container =
            new Lazy<BlobContainerClient>(() =>
                new BlobServiceClient(cs).GetBlobContainerClient(containerName));

        public static BlobContainerClient Container = container.Value;
    }
}

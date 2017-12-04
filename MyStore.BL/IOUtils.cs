using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyStore.BL
{
    public static class IOUtils
    {
        #region Image To Byte Array
        public async static Task<byte[]> ImageFileToByteArray(string imagePath)
        {
            var imageFile = await StorageFile.GetFileFromPathAsync(imagePath);
            return await ImageFileToByteArray(imageFile);
        }

        public async static Task<byte[]> ImageFileToByteArray(StorageFile imageFile)
        {
            using (var inputStream = await imageFile.OpenSequentialReadAsync())
            {
                var stream = inputStream.AsStreamForRead();
                var buffer = await ReadStream(stream);
                return buffer;
            }
        }
        #endregion // Image To Byte Array

        #region Byte Array To Image
        public async static Task<BitmapImage> ByteArrayToBitmapImage(byte[] byteArray)
        {
            var image = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
            {
                writer.WriteBytes(byteArray);
                await writer.StoreAsync();
                await image.SetSourceAsync(stream);
            }

            return image;
        }
        #endregion // Byte Array To Image

        #region Helper Methods
        public async static Task<byte[]> ReadStream(Stream stream)
        {
            var byteArray = new byte[stream.Length];

            int bytesCount = 1;
            int totalBytesCount = 0;
            while (totalBytesCount < byteArray.Length && bytesCount > 0)
            {
                bytesCount = await stream.ReadAsync(byteArray, 0, byteArray.Length);
                totalBytesCount += bytesCount;
            }

            return byteArray;
        }
        #endregion // Helper Methods
    }
}

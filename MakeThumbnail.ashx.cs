using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using BCC.Modules.BCCForms.Components;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;

namespace BCC.Modules.BCCForms
{
    /// <summary>
    ///   Summary description for MakeThumbnail
    /// </summary>
    public class MakeThumbnail : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var intMaxWidth = 0;
            var intMaxHeight = 0;
            Bitmap sourceImage = null;
            Bitmap newImage = null;
            var request = context.Request;
            Stream sourceImageStream = null;

            try
            {
                // Get max. width / height, if any. Clamped so a request can't allocate a huge bitmap.
                intMaxWidth = ClampDimension(request.QueryString["w"].AsInt());
                intMaxHeight = ClampDimension(request.QueryString["h"].AsInt());

                // Get source image path (portal-relative)
                var strFilepath = request.QueryString["image"];
                var ps = PortalController.Instance.GetCurrentSettings();

                // Resolve the file and enforce folder view permissions BEFORE touching the cache,
                // otherwise a thumbnail cached for one user would be served to everyone.
                var file = string.IsNullOrEmpty(strFilepath) ? null : FileManager.Instance.GetFile(ps.PortalId, strFilepath);
                if (file == null || !AllowedExtensions.Contains((file.Extension ?? string.Empty).ToLowerInvariant()))
                {
                    context.Response.StatusCode = 404;
                    return;
                }
                var folder = FolderManager.Instance.GetFolder(file.FolderId) as FolderInfo;
                if (folder == null || !FolderPermissionController.CanViewFolder(folder))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                // Check cache for thumbnail (keyed by file id, so renamed/moved files don't collide)
                var cacheKey = "BCCForms_TN" + file.FileId + "_" + intMaxWidth + "x" + intMaxHeight;
                var image = DataCache.GetCache(cacheKey);
                ImageFormat iFormat;
                if (image == null)
                {
                    // Get source Image
                    sourceImageStream = FileManager.Instance.GetFileContent(file);
                    sourceImage = new Bitmap(sourceImageStream);

                    iFormat = ImageFormat.Jpeg;
                    var intSourceWidth = sourceImage.Width;
                    var intSourceHeight = sourceImage.Height;
                    if ((intMaxWidth > 0 && intMaxWidth < intSourceWidth) ||
                        (intMaxHeight > 0 && intMaxHeight < intSourceHeight))
                    {
                        // Resize image:
                        double aspect = sourceImage.PhysicalDimension.Width / sourceImage.PhysicalDimension.Height;

                        int newWidth;
                        int newHeight;
                        if (intMaxWidth == 0)
                        {
                            newWidth = (int)(intMaxHeight * aspect);
                            newHeight = intMaxHeight;
                        }
                        else if (intMaxHeight == 0)
                        {
                            newWidth = intMaxWidth;
                            newHeight = (int)(intMaxWidth / aspect);
                        }
                        else if ((intSourceWidth / intMaxWidth) >= (intSourceHeight / intMaxHeight))
                        {
                            newWidth = intMaxWidth;
                            newHeight = (int)(intMaxWidth / aspect);
                        }
                        else
                        {
                            newWidth = (int)(intMaxHeight * aspect);
                            newHeight = intMaxHeight;
                        }

                        newImage = new Bitmap(newWidth, newHeight);
                        using (var g = Graphics.FromImage(newImage))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                            g.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
                        }

                        //Cache thumbnail clone (Disposing a cached image will destroy its cache too)
                        DataCache.SetCache(cacheKey, newImage.Clone());
                    }
                    else //use original width (no maxwidth given or image is narrow enough:
                    {
                        newImage = sourceImage;
                    }
                }
                else
                {
                    // Get (cloned) cached thumbnail
                    newImage = (Bitmap)((Bitmap)image).Clone();
                    iFormat = ImageFormat.Jpeg;
                }

                // Send image to the browser.
                context.Response.ContentType = GetContentType(iFormat);
                newImage.Save(context.Response.OutputStream, iFormat);
            }
            finally
            {
                // Clean up
                if (newImage != null) newImage.Dispose();
                if (sourceImage != null) sourceImage.Dispose();
                if (sourceImageStream != null) sourceImageStream.Dispose();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        const int MaxDimension = 2000;

        static readonly string[] AllowedExtensions = { "jpg", "jpeg", "png", "gif", "bmp" };

        static int ClampDimension(int value)
        {
            if (value < 0) return 0;
            return value > MaxDimension ? MaxDimension : value;
        }

        static string GetContentType(ImageFormat iFormat)
        {
            string contentType;

            if (iFormat.Guid.Equals(ImageFormat.Jpeg.Guid))
            {
                contentType = "image/jpeg";
            }
            else if (iFormat.Guid.Equals(ImageFormat.Gif.Guid))
            {
                contentType = "image/gif";
            }
            else if (iFormat.Guid.Equals(ImageFormat.Png.Guid))
            {
                contentType = "image/png";
            }
            else if (iFormat.Guid.Equals(ImageFormat.Tiff.Guid))
            {
                contentType = "image/tiff";
            }
            else if (iFormat.Guid.Equals(ImageFormat.Bmp.Guid))
            {
                contentType = "image/x-ms-bmp";
            }
            else
            {
                contentType = "image/jpeg";
            }
            return contentType;
        }
    }
}
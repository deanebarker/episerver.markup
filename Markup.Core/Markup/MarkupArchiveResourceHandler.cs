﻿using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Markup.Models;
using System;
using System.IO;
using System.Web;
using System.Web.Routing;

namespace Markup
{
    public class MarkupArchiveResourceHandler : IHttpHandler, IRouteHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // Get local variables
            if(context.Request.QueryString["id"] == null || context.Request.QueryString["file"] == null)
            {
                throw NotFound();
            }
            var id = context.Request.QueryString["id"];
            var file = context.Request.QueryString["file"];

            // Get the block
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            MarkupArchiveFile markupArchiveFile;
            try
            {
                markupArchiveFile = repo.Get<MarkupArchiveFile>(new ContentReference(id));
            }
            catch(Exception e)
            {
                throw NotFound();
            }

            // Get the content of the file
            var content = markupArchiveFile.GetFileContent(file);
            if(content == null)
            {
                throw NotFound();
            }

            // Output
            context.Response.ContentType = MimeMapping.GetMimeMapping(file);
            context.Response.BinaryWrite(content);
            context.ApplicationInstance.CompleteRequest();
        }

        private HttpException NotFound()
        {
            return new HttpException(404, "Not Found");
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }
    }
}